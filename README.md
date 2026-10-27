# RadarUtilityLibrary
大陆（Continental）毫米波雷达ARS40X功能库

## 修改记录

### 2026-07-03 — `RadarBlacklist.Contains` 方法逻辑修正

**修改文件：** `ArsLibrary/Model/RadarBlacklist.cs`

**问题：** 原 `Contains` 方法使用 AND 逻辑（所有启用的限制条件都必须在范围内才返回 `true`），且存在短路逻辑导致后续限制类型不被逐一检测，与设计意图不符。

**修正内容：**
- 将核心逻辑从 AND 改为 OR：**只要任意一种限制条件被启用，且当前状态处于该限制的范围内，就返回 `true`**
- 消除 `goto` 短路逻辑，每种限制类型独立判断、逐一检测
- 修正单机姿态限制中对 `null` 参数的处理：原代码要求所有4个参数都必须有值（`HasValue && Between`），修正后对 `null` 参数视为"不限制该维度"（`!HasValue || Between`），与 XML 文档注释一致

**使用说明：**
- 方法签名：`bool Contains(Point3D point, double? walkPos = null, double? pitchAngle = null, double? yawAngle = null, double? stretchLen = null)`
- 检测的限制类型（按顺序）：RCS值、雷达坐标系坐标、单机坐标系坐标、角度、单机姿态
- 若 `ClaimerPostureLimited` 启用但对应的可空参数（`walkPos`/`pitchAngle`/`yawAngle`/`stretchLen`）传入 `null`，则该维度自动视为通过

---

## RHD 雷达 Object 目标结构体字段说明

> 类定义位置：`ArsLibrary/Model/Rhd/ObjectData.cs`，继承自 `ArsLibrary/Model/Rhd/Data`。
> 用于描述 RHD（HURYS 基数据）协议中单个 Object 目标的结构化数据。

### 一、`ObjectData` 自有属性

| 属性名 | 数据类型 | 单位 | 分辨率/取值范围 | 物理意义与使用场景 |
|--------|----------|------|-----------------|---------------------|
| `Type` | `ObjectType`（枚举） | — | `Unknown=0`/`MotorVehicle=1`/`Pedestrian=2`/`NonMotorVehicle=3` | 目标类型。协议 v2.1.3 为保留字段，默认 `Unknown`；v2.1.2 用于区分机动车、行人、非机动车等目标类别 |
| `Length` | `double` | 米 | 分辨率 0.5 | 目标长度（纵向尺寸） |
| `Width` | `double` | 米 | 分辨率 0.5 | 目标宽度（横向尺寸） |
| `Height` | `double` | 米 | 分辨率 0.5 | 目标高度（竖直尺寸） |
| `Speed` | `double` | 米/秒 | 分辨率 0.02 | 目标运动方向速度（标量，沿航向） |
| `Ax` | `double` | 米/秒² | 分辨率 0.01 | X 轴加速度；协议 v2.1.3 为保留字段，默认 0 |
| `Ay` | `double` | 米/秒² | 分辨率 0.01 | Y 轴加速度；协议 v2.1.3 为保留字段，默认 0 |
| `Az` | `double` | 米/秒² | 分辨率 0.01 | Z 轴加速度；协议 v2.1.3 为保留字段，默认 0 |
| `Acceleration` | `double` | 米/秒² | 分辨率 0.01 | 运动方向加速度（标量）；协议 v2.1.3 为保留字段，默认 0 |
| `CourseAngle` | `double` | 度 | 分辨率 0.1 | 目标航向角，从正北方向顺时针计算 |
| `Longitude` | `double` | 度 | 分辨率 1e-8 | 经度，东经为正 |
| `Latitude` | `double` | 度 | 分辨率 1e-8 | 纬度，北纬为正 |
| `Altitude` | `double` | 米 | 分辨率 0.1 | 海拔高度 |
| `Confidence` | `ProbOfExist`（枚举） | — | 见下表 | 置信度（存在概率）；协议 v2.1.3 为保留字段，默认 `Lte100`（<=100%） |
| `MeasState` | `MeasState`（枚举） | — | 见下表 | 检测状态；仅协议 v2.1.2 存在，v2.1.3 默认 `Measured`（已测量） |

### 二、`ProbOfExist` 存在概率枚举（`ArsLibrary/Core/ArsEnums.cs`）

| 枚举成员 | 数值 | 含义 |
|----------|------|------|
| `InvalidValue` | -1 | 无效值（给出的值本身没有意义） |
| `Invalid` | 0 | 无效 |
| `Lt025` | 1 | 小于 25% |
| `Lt050` | 2 | 小于 50% |
| `Lt075` | 3 | 小于 75% |
| `Lt090` | 4 | 小于 90% |
| `Lt099` | 5 | 小于 99% |
| `Lt999` | 6 | 小于 99.9% |
| `Lte100` | 7 | 小于等于 100% |

### 三、`MeasState` 检测状态枚举（`ArsLibrary/Core/ArsEnums.cs`）

| 枚举成员 | 数值 | 含义 |
|----------|------|------|
| `InvalidValue` | -1 | 无效值（给出的值本身没有意义） |
| `Deleted` | 0 | 被删除，ID 消失前的最后一轮数据传输中出现 |
| `New` | 1 | 新出现，ID 创建后的第一轮数据传输中出现 |
| `Measured` | 2 | 已测量，目标的出现被实际测量证实 |
| `Predicted` | 3 | 预测的，目标的出现无法被实际测量证实 |
| `DeletedForMerge` | 4 | 为合并删除，为与另一个目标合并而被删除 |
| `NewFromMerge` | 5 | 合并为新的，合并后产生的新目标 |

### 四、`ObjectType` 目标类型枚举（`ArsLibrary/Core/Rhd/RhdEnums.cs`）

| 枚举成员 | 数值 | 含义 |
|----------|------|------|
| `Unknown` | 0 | 未知 |
| `MotorVehicle` | 1 | 机动车 |
| `Pedestrian` | 2 | 行人 |
| `NonMotorVehicle` | 3 | 非机动车 |

### 五、继承自基类 `Data` 的属性

> `ObjectData` 继承自 `ArsLibrary/Model/Rhd/Data`，以下为基类中定义的原生/扩展属性。

#### 原生属性

| 属性名 | 数据类型 | 单位 | 分辨率 | 物理意义 |
|--------|----------|------|--------|----------|
| `Id` | `ushort` | — | 1-65535 | 目标循环 ID |
| `X` | `double` | 米 | 0.05 | X 轴坐标，正方向为雷达法线方向 |
| `Y` | `double` | 米 | 0.05 | Y 轴坐标，正方向为垂直 X 轴向左 |
| `Z` | `double` | 米 | 0.05 | Z 轴坐标，垂直地面向上 |
| `Vx` | `double` | 米/秒 | 0.02 | X 轴速度，远离雷达方向为正 |
| `Vy` | `double` | 米/秒 | 0.02 | Y 轴速度，向左移动为正 |
| `Vz` | `double` | 米/秒 | 0.02 | Z 轴速度，向上移动为正 |
| `Rcs` | `double` | dBm² | Cluster 0.5 / Object 0.1 | 雷达散射截面积 |

#### 扩展属性

| 属性名 | 数据类型 | 单位 | 物理意义 |
|--------|----------|------|----------|
| `CX` | `double` | 米 | 转换后单机坐标系 X 轴坐标，正方向为臂架/溜筒右侧 |
| `CY` | `double` | 米 | 转换后单机坐标系 Y 轴坐标，正方向为臂架/溜筒前方 |
| `CZ` | `double` | 米 | 转换后单机坐标系 Z 轴坐标，竖直向上 |
| `DistanceToBorder` | `double` | 米 | 距检测边界的距离，与检测方式（点/线/面）及雷达朝向有关 |
| `Angle` | `double` | 度 | 根据纵向与横向坐标计算的方位角 |
| `Color` | `Color` | — | 当前点的颜色（依据 Z 轴值/RCS 值确定） |
| `PushfCounter` | `int` | 次 | 所在帧累积未清除的结算（push finalization）次数 |

> **注意：** 上述所有属性的 setter 均为 `internal`，仅可在程序集内部赋值，外部调用方只能读取（只读访问），数据由协议解析过程填充。

---

## 聚类（DBSCAN）防碰能力（ArsLibrary.Cluster）

> 类定义位置：`ArsLibrary/Cluster/`（9 个文件），命名空间 `ArsLibrary.Cluster`，编译产物 `ArsLibrary.dll`。
> 提供与雷达型号、报文格式无关的中性点云模型与「滤波 → 聚类 → 簇特征 → 跨帧跟踪」算法，供各雷达业务项目复用。

### 一、命名空间与物理路径

| 项目 | 说明 |
|------|------|
| 仓库相对路径 | `ArsLibrary/Cluster/` |
| 文件 | `RadarPoint.cs`、`RadarCluster.cs`、`IClusterResultSource.cs`、`PointCloudFilters.cs`、`Dbscan.cs`、`TrackManager.cs`、`Geometry2D.cs`、`ClusterConst.cs`、`ClusterBoxShape.cs` |
| 命名空间 | `ArsLibrary.Cluster` |
| 目标框架 | .NET Framework 4.5（未使用 C# 8+ 语法，便于与 net45 业务项目共用） |

设计原则：**与雷达型号、报文格式、坐标系解耦**。ARS408 侧使用 `DistLong/DistLat`、RHD 侧使用 `X/Y/Z`，两套坐标**不同源、不可混用**；由调用方在转换为 `RadarPoint` 时决定语义，库内只认中性 `RadarPoint`。

### 二、文件与类型清单

| 文件 | 类型 | 说明 |
|------|------|------|
| `RadarPoint.cs` | `RadarPoint` | 聚类中性点（7 维特征 + 聚类/跟踪标签） |
| `RadarCluster.cs` | `RadarCluster` / `ClusterBox` | 簇结果实体 / 本体（臂架）盒 |
| `IClusterResultSource.cs` | `IClusterResultSource` | 绘制侧解耦接口（5 个成员） |
| `PointCloudFilters.cs` | `PointCloudFilters` | SOR 离群点去除 + 归一化加权 |
| `Dbscan.cs` | `Dbscan` | 拐点法自适应 eps + DBSCAN + 两类过滤 |
| `TrackManager.cs` | `TrackManager` / `TrackState` | 跨帧跟踪（贪心最近邻） |
| `Geometry2D.cs` | `Geometry2D` | 凸包（Andrew monotone chain） |
| `ClusterConst.cs` | `ClusterConst` | `[Cluster]` 段常量 + `Refresh(IniFileHelper)` |
| `ClusterBoxShape.cs` | `ClusterBoxShape` | 框形状枚举（0 长方形 / 1 凸包） |

### 三、API 速查

#### 3.1 RadarPoint

| 成员 | 数据类型 | 默认值 | 说明 |
|------|----------|--------|------|
| `X` / `Y` / `Z` | `double` | 0 | 纵向 / 横向 / 高度，米（ARS408 侧 `Z` 恒为 0） |
| `Vx` / `Vy` / `Vz` | `double` | 0 | 纵 / 横 / 垂向相对速度，米/秒（ARS408 侧 `Vz` 恒为 0） |
| `Rcs` | `double` | 0 | 反射强度，dB |
| `ClusterId` | `int` | -1 | 簇标签，-1 表示噪声 |
| `TrackId` | `int` | -1 | 跨帧跟踪 ID，-1 表示未分配 |
| `SourceIndex` | `int` | -1 | 原始点在传入快照中的索引（回查用） |
| `DistanceToBorder` | `double` | 0 | 距检测区边界的距离，用于计算簇的报警距离 L |
| `Radius` | `double` | 0 | 距雷达原点的距离 |
| `GetFeature(int col)` | `double` | — | 按列号取特征值：`0-X 1-Y 2-Z 3-Vx 4-Vy 5-Vz 6-Rcs`，越界返回 0 |

两套坐标映射（**不同源，不可混用**）：

| RadarPoint | ARS408 `SensorGeneral` | RHD `ArsLibrary.Model.Rhd.Data` |
|------------|------------------------|----------------------------------|
| `X` / `Y` / `Z` | `DistLong` / `DistLat` / 0 | `X` / `Y` / `Z` |
| `Vx` / `Vy` / `Vz` | `VrelLong` / `VrelLat` / 0 | `Vx` / `Vy` / `Vz` |
| `Rcs` | `RCS` | `Rcs` |

#### 3.2 RadarCluster / ClusterBox

| 成员 | 数据类型 | 说明 |
|------|----------|------|
| `Label` | `int` | DBSCAN 原始簇标签 |
| `TrackId` | `int`（默认 -1） | 跨帧跟踪 ID |
| `PointCount` | `int` | 簇内点数 |
| `XMin/XMax`、`YMin/YMax`、`ZMin/ZMax` | `double` | 包围盒（纵 / 横 / 高） |
| `HeartZ` | `double` | 高度中心（Z 均值），用于高度筛选 |
| `L` | `double` | 报警距离：`min(DistanceToBorder) − LOffset` |
| `AverageDistanceToBorder` / `AverageRadius` | `double` | 平均边界距离 / 平均半径（备用） |
| `W` / `H` | `double` | 宽度 `\|(XMin+XMax)/2\|` / 高度 `ZMax` |
| `RcsMin` / `RcsAverage` | `double` | 最小 / 平均反射强度 |
| `IsValid` | `bool` | 有效簇（点数达标 且 高度筛选通过） |
| `Points` | `List<RadarPoint>` | 簇内点 |
| `RadarCluster()` / `RadarCluster(int label, List<RadarPoint> points)` | 构造器 | 带参构造同时算好包围盒 / HeartZ / 均值 / W / H |
| `Evaluate(double lOffset, int minPoints, bool heartZFilterEnabled, double heartZLow, double heartZHigh)` | `void` | 计算 `L` 并判定 `IsValid` |
| `ToString()` | `string` | `簇{0}(跟踪{1}) 点数{2} L={3:F2} W={4:F2} H={5:F2} Rcs={6:F1} 有效{7}` |

`ClusterBox`：`XMin/XMax/YMin/YMax/ZMin/ZMax` + `bool Contains(RadarPoint point)`（null 返回 false），用于本体 / 臂架盒的保存与剔除判定。

#### 3.3 IClusterResultSource

| 成员 | 类型 | 说明 |
|------|------|------|
| `ClusterEnabled` | `bool { get; }` | 全局开关与雷达自身开关同时为真 |
| `Clusters` | `List<RadarCluster> { get; }` | 最近一次聚类得到的簇列表 |
| `BestCluster` | `RadarCluster { get; }` | 最危险（L 最小）的有效簇，无有效目标时为 null |
| `ClusterPoints` | `List<RadarPoint> { get; }` | 最近一次参与聚类的点，含噪声点（`ClusterId = -1`） |
| `ClusterDistanceSuppressed` | `bool { get; }` | 最近一次结果是否因跳变防抖被抑制而未输出 |

> 备注：业务侧两个帧消息处理类的泛型参数不同、不存在统一非泛型父类，故以本接口解耦；所需成员在实现类中已以公共属性存在，实现接口无需新增成员。

#### 3.4 PointCloudFilters

| 方法 | 签名 | 说明 |
|------|------|------|
| `StatisticalOutlierRemoval` | `List<RadarPoint> (List<RadarPoint> points, int k, double nSigma)` | SOR：仅用 X/Y/Z 计算 k 近邻平均距离，超过 `均值 + nSigma × 标准差` 的点剔除；`n < k+1` 或标准差为 0 时原样返回副本；返回新集合、不修改入参 |
| `NormalizeAndWeight` | `double[][] (List<RadarPoint> points, int[] cols, double[] weights)` | 逐列 Z-score 归一化（**总体标准差，除以 N**，对标 sklearn `StandardScaler`）后乘权重；行=点、列=特征；权重缺失或长度不足按 1 处理，标准差为 0 的列填 0 |

#### 3.5 Dbscan

| 成员 | 签名 | 说明 |
|------|------|------|
| `NOISE` | `const int = -1` | 噪声标签（公共常量） |
| `FindKneeEps` | `double (double[][] feats, int k)` | 拐点法自适应 eps：k 近邻距离排序 → 归一化 → 二阶差分最大点；样本数 < 5、极差过小或膝盖落在端点时兜底回退均值 |
| `Cluster` | `int[] (double[][] feats, double eps, int minSamples)` | DBSCAN，返回每点标签（`NOISE = -1`）；内部预建距离矩阵，复杂度 O(N²) |
| `FilterSparseClusters` | `void (double[][] feats, int[] labels, double eps, double ratio)` | 簇内平均最近邻距离 > `ratio × eps` 的**整簇**判为噪声（原地修改 labels） |
| `FilterIntraClusterOutliers` | `void (double[][] feats, int[] labels, double eps, double ratio)` | 簇内最近邻距离 > `ratio × eps` 的**单点**判回噪声（原地修改 labels） |

#### 3.6 TrackManager / TrackState

| 成员 | 类型 / 签名 | 默认值 / 说明 |
|------|-------------|----------------|
| `MaxDistance` | `double { get; set; }` | 3.0，关联距离阈值（观测量之差的绝对值） |
| `MaxLostFrames` | `int { get; set; }` | 5，最大丢失周期数，超过则删除轨迹 |
| `TrackCount` | `int { get; }` | 当前存活轨迹数 |
| `Update` | `Dictionary<int,int> (Dictionary<int,double> keyToValue)` | 贪心最近邻匹配，返回「键 → 跟踪 ID」；未匹配轨迹 `LostCount++`，超过上限删除 |
| `Reset` | `void ()` | 清空所有轨迹并把 ID 计数器归 1 |
| `TrackState.Value` | `double { get; set; }` | 最近一次观测量（对应业务侧的报警距离 L） |
| `TrackState.LostCount` | `int { get; set; }` | 连续丢失周期数 |
| `TrackState(double value)` | 构造器 | 初始化 `LostCount = 0` |

#### 3.7 Geometry2D

| 方法 | 签名 | 说明 |
|------|------|------|
| `ConvexHull` | `List<RadarPoint> (IEnumerable<RadarPoint> points)` | 强类型重载：以 `X`（纵向）为第一坐标、`Y`（横向）为第二坐标 |
| `ConvexHull<T>` | `List<T> (IEnumerable<T> points, Func<T,double> getX, Func<T,double> getY) where T : class` | 泛型重载（Andrew monotone chain，O(n log n)）；跳过 null 元素 |

> 两个重载均：点集为空返回空列表；**点数不足 3 个或全部共线时返回不足 3 个点的列表，调用方应回退为包围盒**（画面绘制正是据此回退为长方形）。

#### 3.8 ClusterConst

| 分组 | 成员 |
|------|------|
| 常量 | `SectionName = "Cluster"` |
| 开关 | `Enabled`、`SorEnabled`、`ArmBoxEnabled`、`AntiBounceEnabled`、`HeartZFilterEnabled`、`DebugEnabled` |
| 滤波 | `SorK`、`SorNSigma`、`ClusterCols`、`FeatureWeights` |
| 聚类 | `KInit`、`MinSamplesInit`、`KDynamic`、`MinSamplesDynamic`、`SparseRatio`、`IntraRatio`、`MaxPoints` |
| 判定与跟踪 | `LOffset`、`MinClusterPoints`、`HeartZLow`、`HeartZHigh`、`TrackMaxDistance`、`TrackMaxLostFrames`、`AntiBounceLevel`、`BodyBoxMeanYLimit`、`BodyBoxXMinLimit` |
| 结果显示 | `ClusterBoxEnabled`、`BoxShape`、`BoxColorMode`、`BoxDangerDistance`、`BoxWarnDistance`、`BoxLineWidth`、`BoxFillAlpha`、`BoxShowLabel`、`BoxOnlyValid`、`BoxNoiseDots`、`BoxEmphasizeBest` |
| 方法 | `double[] GetAlignedWeights()`、`bool IsValidCol(int col)`、`void Refresh(IniFileHelper iniHelper)` |

#### 3.9 ClusterBoxShape

| 枚举成员 | 数值 | 含义 |
|----------|------|------|
| `Rectangle` | 0 | 长方形（轴对齐包围盒） |
| `ConvexHull` | 1 | 不规则凸包多边形（默认） |

### 四、配置段 `[Cluster]` 与 `ClusterConst.Refresh`

段名常量：`ClusterConst.SectionName = "Cluster"`。全部参数由 `public static void Refresh(IniFileHelper iniHelper)` 读取，`IniFileHelper` 来自 `CommonLib.Function`。

调用约定与行为：

- 业务项目在**配置热刷新**处调用一次即可（如 ARS408 的 `Base.cs` 中 `RefreshConfigs`，周期 1 秒）。
- `iniHelper` 为 null 时直接返回；方法体整体包在 try/catch 中，**读取失败静默跳过，不中断刷新线程**。
- 每项用 `TryReadValue(section, key, 默认值, out parsed)`：键缺失或解析失败时保留**内存中的当前值**。
- `ClusterBoxFillAlpha` 钳位到 `[0, 255]`；`ClusterBoxLineWidth` 下限 0.5。
- `ClusterCols` / `FeatureWeights` 用 `ReadData` + 逗号分割解析，内容为空或空白时不修改。

**注意：4 个显示类参数的 ini 键带 `Cluster` 前缀、属性名不带**，写配置时容易踩坑：

| ini 键 | 属性名 | 默认值 |
|--------|--------|--------|
| `ClusterBoxShowLabel` | `BoxShowLabel` | true |
| `ClusterBoxOnlyValid` | `BoxOnlyValid` | false |
| `ClusterBoxNoiseDots` | `BoxNoiseDots` | false |
| `ClusterBoxEmphasizeBest` | `BoxEmphasizeBest` | true |

其余 `ClusterBox*` 键（`ClusterBoxEnabled`、`ClusterBoxShape`、`ClusterBoxColorMode`、`ClusterBoxDangerDistance`、`ClusterBoxWarnDistance`、`ClusterBoxLineWidth`、`ClusterBoxFillAlpha`）与属性名一致。

关键默认值：`Enabled=false`、`SorEnabled=false`、`ArmBoxEnabled=false`、`AntiBounceEnabled=true`、`HeartZFilterEnabled=false`、`DebugEnabled=false`、`SorK=10`、`SorNSigma=2.0`、`ClusterCols={0,1}`、`FeatureWeights={1,1.5}`、`KInit=5`、`MinSamplesInit=5`、`KDynamic=4`、`MinSamplesDynamic=5`、`SparseRatio=1.5`、`IntraRatio=0.7`、`MaxPoints=300`、`LOffset=0`、`MinClusterPoints=2`、`HeartZLow=-1`、`HeartZHigh=0`、`TrackMaxDistance=3.0`、`TrackMaxLostFrames=5`、`AntiBounceLevel=3`、`BodyBoxMeanYLimit=2`、`BodyBoxXMinLimit=20`、`ClusterBoxEnabled=true`、`BoxShape=ConvexHull(1)`、`BoxColorMode=0`、`BoxDangerDistance=3.0`、`BoxWarnDistance=10.0`、`BoxLineWidth=2f`、`BoxFillAlpha=30`。

### 五、被业务项目引用与构建顺序

业务项目（如 ARS408）以 **DLL 直接引用**方式接入，**不是 ProjectReference**，因此 VS 不会自动先建库：

```xml
<Reference Include="ArsLibrary">
  <HintPath>..\..\..\Radar\RadarUtilityLibrary\ArsLibrary\bin\Debug\ArsLibrary.dll</HintPath>
</Reference>
```

**必须顺序**：① 先生成 `ArsLibrary`（Debug）→ ② 再生成业务项目。只生成业务项目会用到旧的 DLL。

本库 PostBuildEvent 会把产物同步到共享目录：

```
xcopy /e /r /y /exclude:D:/DLL/_uncopy.txt $(ProjectDir)bin\Debug D:/DLL/
```

接入三步：① 引用 `ArsLibrary.dll`；② 需要处 `using ArsLibrary.Cluster;`；③ 在配置热刷新处调用 `ClusterConst.Refresh(iniHelper)`。

**留在业务项目、不入库的部分**（因为这些依赖报文类型、帧生命周期、数据库/界面或 WinForms）：报文到 `RadarPoint` 的映射、聚类编排与距离/报警级数的覆盖、雷达开关与数据库/界面、GDI+ 画面绘制。

### 六、依赖

- 直接依赖：`CommonLib`（`CommonLib.Function.IniFileHelper`）、`MathNet.Numerics 4.12.0`、`Newtonsoft.Json 12.0.3`、`protobuf-net 2.4.6`、`SocketHelper`、`System.Data.SQLite`、`System.Drawing`
- 目标框架：`v4.5`

### 七、变更记录（本次）

### 2026-10-16 — 新增 `ArsLibrary.Cluster` 聚类（DBSCAN）防碰能力

**新增文件：** `ArsLibrary/Cluster/`（`RadarPoint.cs`、`RadarCluster.cs`、`IClusterResultSource.cs`、`PointCloudFilters.cs`、`Dbscan.cs`、`TrackManager.cs`、`Geometry2D.cs`、`ClusterConst.cs`、`ClusterBoxShape.cs`）；`ArsLibrary/ArsLibrary.csproj` 新增 9 条 `Compile`

**问题：** 聚类（DBSCAN）防碰的中性点云模型、算法与配置常量原先写在业务项目内，无法被其它雷达项目复用

**修正内容：**
- 下沉为公共库命名空间 `ArsLibrary.Cluster`，与雷达型号、报文格式、坐标系解耦
- `DbscanClusterer` → **`Dbscan`**；`ClusterTrackManager` / `ClusterTrack` → **`TrackManager`** / **`TrackState`**（`L` 改为 `Value`）；`GeometryUtil` → **`Geometry2D`**（新增 `ConvexHull(IEnumerable<RadarPoint>)` 强类型重载）
- `ClusterConst` 新增 `Refresh(IniFileHelper)`，接管 `[Cluster]` 段全部参数的读取（业务侧 80 余行就地读取改为一行）

**使用说明：**
- 命名空间 `ArsLibrary.Cluster`；调用方需在配置热刷新处调用 `ClusterConst.Refresh(iniHelper)`
- 注意 4 个显示类参数的 ini 键带 `Cluster` 前缀而属性名不带
- 业务项目以 DLL 方式引用 `ArsLibrary.dll`，**必须先生成库再生成业务项目**

---

### 2026-10-16 — 修复 DBSCAN 距离矩阵构建时的空引用崩溃

**修改文件：** `ArsLibrary/Cluster/Dbscan.cs`

**问题：** 启用聚类（`[Cluster] Enabled=1` 且雷达勾选"聚类防碰"）后，`Dbscan.Cluster` 在 `BuildDistanceMatrix` 抛 `System.NullReferenceException`，点数 ≥ 2 时每周期必崩。

**修正内容：**
- `BuildDistanceMatrix` 原先按"写一行建一行"的方式初始化距离矩阵，但内层循环会写入 `dist[j][i]`（j > i），此时 `dist[j]` 仍为 null。改为先把所有行创建出来，再统一填值。
- `FindKneeEps` 增加 `feats.Length < 2` 的入口保护：点数为 1 时不存在近邻，原实现会因 `dists` 为空而抛 `ArgumentOutOfRangeException`。

**使用说明：** 属内部实现修复，未改动任何公开签名，二进制兼容。已部署现场需更新 `ArsLibrary.dll`。

---

## 特征量纲与稀疏簇处理（新增能力）

> 位置：`ArsLibrary/Cluster/`。在原有聚类能力上补充「特征量纲可选物理量纲」与「稀疏簇按间隙拆分」两项能力，用于解决"距离较远的点被并入同一簇"以及"大范围密实目标被切碎"两类现场问题。

### 一、两种特征量纲模式

| 模式 | 触发条件 | 特征空间 | eps 含义 | 相对权重来源 |
|---|---|---|---|---|
| Z-score 归一化（原有） | 未配置 `FeatureScale` | 各列 Z-score（总体标准差）后乘权重 | 归一化单位 | `FeatureWeights` |
| 物理量纲（新增） | 配置了 `FeatureScale` | 各列原值乘物理缩放系数 | **米** | `FeatureScale` 本身 |

物理量纲模式的意义：

1. **消除"大密实目标被切碎"的副作用**。Z-score 按各列标准差缩放，某列标准差过小时该列距离被放大，导致大范围密实的目标沿该方向被切成多簇。实测 8m×2m 的密实面在归一化模式下被切成 4 簇，物理量纲模式下保持 1 簇。
2. **eps 具有米制含义**，`EpsMax` 可直接表达"多远的点算作同一个目标"。

> 注意：物理量纲模式下 `FeatureWeights` 不再生效，横向等相对权重直接写进 `FeatureScale`（按列号索引，例 `1,1.5,1,0.1,0.1,0.1,0` 表示纵向 1 米、横向 1.5 倍权重、Z 参与、速度列 0.1 米每米每秒、RCS 不参与）。

### 二、稀疏簇的判定与拆分（最小生成树间隙比）

**不能用坐标跨度判断稀疏**：单个大而密实的目标跨度可以比"两个被合并的目标"还大。改用最小生成树（MST）间隙比：

- 簇内点求 MST，**最长边**即两个密团之间的"桥"；单个大而密实目标的 MST 各边长度接近，不会被误拆。
- 断边阈值 = `max(ClusterGapMin, ClusterGapRatio × MST边长中位数)`，绝对值与相对值双条件。
- 实测间隙比：单个密实大目标 1.3~1.4，两个目标被间隙连接 7.5~32.6，两者之间没有模糊地带。
- 拆分依赖物理量纲（阈值为米），归一化模式下应跳过。

### 三、新增 API

| 成员 | 签名 | 说明 |
|---|---|---|
| `PointCloudFilters.ApplyScale` | `double[][] (List<RadarPoint> points, int[] cols, double[] scales)` | 按列的物理量纲缩放，不做 Z-score；`scales` 按列号索引（0-X 1-Y 2-Z 3-Vx 4-Vy 5-Vz 6-Rcs），值为 0 表示该列不参与 |
| `Dbscan.FindKneeEps` | `double (double[][] feats, int k, double epsMax = 0)` | 新增可选参数 `epsMax`（与 eps 同量纲），大于 0 时对自适应 eps 做上限截断 |
| `Dbscan.SplitClusterByGap` | `List<List<int>> (double[][] feats, List<int> indices, double gapMin, double gapRatio)` | 用 MST 间隙把一个簇拆成若干子簇，返回各子簇在 `feats` 中的下标集合；无法拆分时返回原簇一项 |

### 四、新增配置项（`[Cluster]` 段）

| ini 键 | 属性名 | 默认值 | 说明 |
|---|---|---|---|
| `FeatureScale` | `FeatureScale` | 未配置（为 null） | 各列物理缩放系数，按列号索引；配置后启用物理量纲模式。**不配置则保持原有 Z-score 行为** |
| `EpsMax` | `EpsMax` | 0 | 自适应 eps 上限（米），0 表示不限制 |
| `ClusterSplitEnabled` | `ClusterSplitEnabled` | false | 是否按间隙拆分被桥接的稀疏簇 |
| `ClusterGapMin` | `ClusterGapMin` | 1.5 | 断边阈值的绝对下限（米） |
| `ClusterGapRatio` | `ClusterGapRatio` | 5 | 断边阈值的相对系数（阈值 = 系数 × MST 边长中位数） |

> 解析健壮性：`FeatureScale`/`ClusterCols`/`FeatureWeights` 均改为 `TryParse` 解析，单项格式错误时保留内存中的当前值，且不再中断其后配置项的读取。

### 五、变更记录（本次）

### 2026-10-21 — 新增物理量纲模式与稀疏簇间隙拆分

**修改文件：** `ArsLibrary/Cluster/PointCloudFilters.cs`、`ArsLibrary/Cluster/Dbscan.cs`、`ArsLibrary/Cluster/ClusterConst.cs`

**问题：** 现场反馈距离较远的点被并入同一簇、簇被撑大而稀疏；同时存在"跨度很大但并不稀疏"的大范围密实目标，不能用跨度判断。

**修正内容：**
- `PointCloudFilters` 新增 `ApplyScale`，支持不做 Z-score 的物理量纲缩放，消除某列标准差过小导致大密实目标被切碎的副作用，并使 eps 具有米制含义。
- `Dbscan.FindKneeEps` 增加 `epsMax` 上限截断。
- `Dbscan` 新增 `SplitClusterByGap`，用最小生成树间隙比识别并剪开被桥接的稀疏簇；单个大而密实目标的间隙比为 1.3~1.4，被间隙连接的目标为 7.5 以上，不会误拆。
- `ClusterConst` 新增 `FeatureScale`/`EpsMax`/`ClusterSplitEnabled`/`ClusterGapMin`/`ClusterGapRatio`；配置解析改为 `TryParse`，单项错误不再影响后续读取。

**使用说明：** 均为新增能力，默认保持既有行为（`FeatureScale` 未配置即仍为 Z-score 模式，拆分默认关闭）。启用物理量纲后 `FeatureWeights` 不再生效。

---

## 簇距离聚合方式（新增能力）

> 位置：`ArsLibrary/Cluster/`。为 `RadarCluster` 增加可配置的簇距离聚合方式，解决"物体表面距离被单个突出近点拉近"的问题。

### 一、背景

`RadarPoint.DistanceToBorder` 是**该防御方向上点到检测区边界的距离**（按防御模式取不同坐标轴：点模式取三维径向、线模式忽略横向、面模式只取纵向）。因此簇内取**最小值**得到的是**物体最靠近检测边界的那一面**，即防碰需要的"物体表面距离"。

- **必须用最小值**：中位数/均值代表的是簇的**中心位置**。实测斜置车辆（点云从 x=9 铺到 x=13）时中位数给出 11.0m，比真实表面远 2.0m；报警阈值仅 2.0/1.6/1.0m，该偏差足以造成等级误判。
- **最小值的弱点**：对单个突出近点（毛刺、倒角、误检点）极度敏感。表面在 12.0m 而有一个 8.5m 的突出点时，最小值偏近 3.5m，直接造成误报。

### 二、四种聚合方式

| 方式 | 配置值 | 取值 | 对表面距离的表达 | 适用性 |
|------|--------|------|------------------|--------|
| 最小值 | `DistanceMode=0` | 簇内最小值 | 语义完全正确 | 现状；对单个突出近点敏感 |
| **剔除近端离群后取最小值** | **`DistanceMode=1`（推荐）** | 剔掉若干最小值后的最小值 | 保留表面语义 | **推荐** |
| 第k小值 | `DistanceMode=2` | 排序后第k个 | 语义正确 | 与方式1接近，但k固定 |
| 中位数 | `DistanceMode=3` | 排序后中间值 | 取到簇中心位置 | 仅供对比试验，不建议上线 |

方式1的剔除规则：

```
drop = round(点数 × DistanceDropRatio)
drop = min(drop, DistanceMaxDrop)   // 封顶
drop = min(drop, 点数 - 2)            // 至少保留2个点参与取最小值
L = 排序后第 drop+1 个值
```

- **只剔近端**：远端点对最小值本就无害，剔它会让距离偏远。
- **至少保留2个点**：否则2点簇剔1个会从"最近的那一面"翻到"更远的一面"，表面语义反转。2~3点的小簇不剔除。

### 三、新增 API

| 成员 | 签名 | 说明 |
|------|------|------|
| `RadarCluster.ComputeClusterDistance` | `double (int distanceMode, double dropRatio, int maxDrop, int kth)` | 按指定方式计算簇的报警距离（未减距离偏置）；无点时返回0 |
| `RadarCluster.Evaluate` | 新增 4 个可选参数 `distanceMode`/`dropRatio`/`maxDrop`/`kth` | 均有默认值，不传时行为与旧版完全一致（源码与二进制向后兼容） |

### 四、新增配置项（`[Cluster]` 段）

| ini 键 | 属性名 | 默认值 | 说明 |
|--------|--------|--------|------|
| `ClusterDistanceMode` | `DistanceMode` | 0 | 聚合方式：0最小值 / 1剔除近端离群后最小值 / 2第k小 / 3中位数 |
| `ClusterDistanceDropRatio` | `DistanceDropRatio` | 0.15 | 剔除比例（方式1生效） |
| `ClusterDistanceMaxDrop` | `DistanceMaxDrop` | 2 | 剔除最大个数（方式1生效） |
| `ClusterDistanceKth` | `DistanceKth` | 2 | 第k小值的k（方式2生效） |

### 五、变更记录（本次）

### 2026-10-27 — 新增簇距离聚合方式

**修改文件：** `ArsLibrary/Cluster/RadarCluster.cs`、`ArsLibrary/Cluster/ClusterConst.cs`

**问题：** 簇报警距离取簇内最小值，对单个突出近点（毛刺、倒角、误检点）极度敏感，会把距离拉近数米造成误报；而改用中位数/均值又会取到簇中心位置、破坏"物体表面距离"语义。

**修正内容：**
- `RadarCluster` 新增 `ComputeClusterDistance`，支持最小值 / 剔除近端离群后最小值 / 第k小值 / 中位数四种方式。
- `Evaluate` 增加 4 个可选参数，均有默认值，未传参时行为与旧版完全一致。
- `ClusterConst` 新增 `DistanceMode`/`DistanceDropRatio`/`DistanceMaxDrop`/`DistanceKth` 并接入 `Refresh` 读取。
- 剔除规则保证至少保留 2 个点参与取最小值，避免表面语义反转。

**使用说明：** 默认 `ClusterDistanceMode=0`（与原有行为完全一致），需现场确认后再切到 `1`。该能力只解决"簇内近端离群点"造成的距离突变，解决不了"远处点被并入同一簇"（后者需靠 `EpsMax` 与 `ClusterSplitEnabled`）。

---

## 传感器几何上下文接口（ISensorGeometryContext）

> 位置：`ArsLibrary/Model/ISensorGeometryContext.cs`、`ArsLibrary/Model/SensorGeometryContextDefault.cs`。
> 目的：让点消息模型（SensorGeneral 及其子类）不再直接依赖具体雷达实体类，从而该模型可以下沉到公共库、被多个现场项目共用一份实现。

### 一、接口构成

| 接口 | 成员数 | 说明 |
|------|--------|------|
| `ISensorGeometryContext` | 22 | 点消息换算单机坐标与边界距离所需的全部雷达参数：Id、Direction、DefenseMode、X/Y/ZOffset、X/Y/ZmodifiedRatios、RadarxMin/Max、RadaryMin/Max、Claimerx/y/zMin/Max、AngleMin/Max |
| `ISensorLimitContext` | 3 | 三个范围限制开关：WithinRadarLimit、WithinClaimerLimit、WithinAngleLimit |

两个接口分开的理由：**并非所有现场的雷达实体都具备那三个开关**（部分现场在早期版本中将其注释掉、未启用）。点消息侧用 `as ISensorLimitContext` 探测实现，未实现时按"开关全部生效"处理（不反转判定结果），与"开关为真"的语义一致。

### 二、实现方须知

- 接口成员**只有 getter**。C# 允许把 `{ get; private set; }` 或 `{ get; set; }` 的属性实现为只读接口成员，**实现类无需修改属性的可写性**，接入成本极低。
- 属性名与语义在所有实现中保持一致，避免"同名不同义"。
- 取值须与雷达实体保持同步（雷达参数在运行期可能被重新标定）。

### 三、默认上下文

`SensorGeometryContextDefault` 是"未指定雷达"时的兜底实现：Id = -1、偏移量与范围上下限全为 0、Direction = None、DefenseMode = 0。点消息模型据此跳过坐标换算与范围判定，与历史"新建一个默认雷达实体"的行为一致。

### 四、已接入的项目

| 项目 | 改动 |
|------|------|
| 煤二期 `ARS408_RadarImage_Mei2` | `Radar` 实现 `ISensorGeometryContext` + `ISensorLimitContext`；`SensorGeneral` 的 `Radar` 属性类型由具体 `Radar` 改为 `ISensorGeometryContext`；`ClusterGeneral` / `ObjectGeneral` 构造器的雷达参数类型同步改为接口 |
| 湛江 `ARS408_RadarImage_Standby` | `Radar` 实现 `ISensorGeometryContext`（不含限制开关，该现场无这三个成员）；其余同上 |

改造后，点消息模型的坐标换算与范围判定结果与改造前**逐位一致**（已用 289 项用例对照验证：7 种朝向 × 3 种防御模式 × 5 组坐标，含限制开关的开/关组合，以及未指定雷达时的兜底语义）。

### 五、后续可做的下沉工作

点消息模型（`SensorGeneral` / `ClusterGeneral` / `ObjectGeneral`）目前仍在各现场项目内，进一步下沉到本库还需处理：

1. **两个项目的 `DistanceToBorder` 语义分叉**：煤二期在面模式下带符号处理（`DistanceToBorder *= Math.Sign(x) * ...`），湛江该段被注释掉且上下方向一律取 `Math.Abs(z)`。下沉时必须先确定统一语义，否则会改变其中一个现场的报警行为。
2. **序列化机制不同**：煤二期用 ProtoBuf 特性，湛江用 SerializationFactory，同一份模型无法同时用两套机制。
3. **子类在业务侧**：`[ProtoInclude]` 直接引用具体子类，需一并处理。

### 六、变更记录（本次）

### 2026-10-27 — 新增传感器几何上下文接口并接入两个现场项目

**新增文件：** `ArsLibrary/Model/ISensorGeometryContext.cs`、`ArsLibrary/Model/SensorGeometryContextDefault.cs`；`ArsLibrary/ArsLibrary.csproj` 新增 2 条 `Compile`

**问题：** 点消息模型（SensorGeneral）直接依赖具体雷达实体类（煤二期 1218 行、湛江 1410 行），模型层无法下沉到公共库，两个现场各自维护一份已分叉的同名类。

**修正内容：**
- 新增 `ISensorGeometryContext`（22 个几何与标定参数）与 `ISensorLimitContext`（3 个范围限制开关）两个接口。
- 新增 `SensorGeometryContextDefault` 作为未指定雷达时的兜底实现。
- 煤二期与湛江两个项目的 `Radar` 分别实现对应接口；`SensorGeneral` 的 `Radar` 属性、两个子类的构造器参数由具体类型改为接口类型。
- `CheckIfWithinLimits` 的开关读取改为 `as ISensorLimitContext` 探测，未实现时按"开关全部生效"处理。

**使用说明：** 接口成员只含 getter，实现类的属性可写性无需调整。行为等价性已验证（289 项用例全部一致）。**本次仅新增 public 类型，未改动任何既有成员，其它引用 ArsLibrary.dll 的项目无需重新编译。**

---

## 点消息模型下沉（SensorGeneral / ClusterGeneral / ObjectGeneral）

> 位置：`ArsLibrary/Model/SensorGeneral.cs`、`ArsLibrary/Model/ClusterGeneral.cs`、`ArsLibrary/Model/ObjectGeneral.cs`。
> 这三个原先位于各现场项目内的点消息模型，现已下沉到公共库，由煤二期与湛江两个现场共用同一份实现。

### 一、下沉内容

| 类型 | 说明 |
|------|------|
| `SensorGeneral` | 点消息基类，抽象。含坐标换算、边界距离计算、范围判定、动态属性、颜色与 RCS 换算 |
| `ClusterGeneral` | 集群点消息，继承 `SensorGeneral` |
| `ObjectGeneral` | 目标点消息，继承 `SensorGeneral` |
| `ModifiedCoordinates` | 转换后的原始坐标载体（原在业务侧，同文件内定义） |

依赖全部落在库内：`SensorMessage` / `BaseMessage`（`ArsLibrary.Model`）、`DynProp` / `FalseAlarmProbability` / `AmbigState` / `InvalidState` / `ProbOfExist`（`ArsLibrary.Core`）、`ArsFunc`（`ArsLibrary.Core`）、`Point3D`（`CommonLib.Function.MathUtils`）、`CoordinateRatios`（`CommonLib.Function.Fitting`）、`ProtoBuf`（protobuf-net 2.4.6）。

### 二、序列化统一为 ProtoBuf

三个模型统一采用 `protobuf-net` 的 `[ProtoContract]` / `[ProtoInclude]` / `[ProtoMember]` 标注。

- 煤二期：原本就使用 ProtoBuf 标注，**下沉后完全等价**。
- 湛江：原先三个模型**没有任何序列化标注**（`using SerializationFactory;` 是冗余 using），下沉后自动获得 ProtoBuf 契约能力，**不影响原有行为**。
- 三个项目引用的 `protobuf-net` 均为 **2.4.6**（同一 HintPath），标注类型完全一致。

### 三、DistanceToBorder 语义（统一为煤二期语义）

按防御模式与雷达朝向计算"点到检测边界的距离"：

| 防御模式 | 计算式 |
|---|---|
| m=1 点 | `sqrt(x² + z² + y²)` 三维径向 |
| m=2 线 | `sqrt(x² + z²)` 忽略横向 |
| m=3 面 | `|x|`，且**带符号** |

面模式下的符号处理：`DistanceToBorder *= Math.Sign(x) * (dir == Left || dir == Back ? -1 : 1)`，可区分目标位于检测面的哪一侧；上下方向（Up/Down）且面模式时取竖直坐标 z，向下时整体取反。

> **湛江统一后的行为变化**：湛江原实现为"上下方向一律取 `Math.Abs(z)`、面模式符号处理整段注释掉"，即距离恒为正。统一为煤二期语义后，**部分方向上的目标距离会变为负值**。
>
> 实测对照（30 组：6 朝向 × 5 组坐标，面模式）：24 组完全一致，6 组仅符号相反，**无数值量级变化**。
>
> 受影响的方向（现场 24 台雷达全部为面模式，方向分布 Front×4 / Left×5 / Back×4 / Right×5 / Down×6）：
> - `Left` 方向：单机 X 为正的一侧 → 距离变负
> - `Right` 方向：单机 X 为负的一侧 → 距离变负
> - `Front` / `Back` 方向：横向坐标为负的一侧 → 距离变负
>
> **注意**：负距离在 `GetThreatLevelByValue` 的分段阈值映射中（判断条件为 `value <= -100` 才视为无效），会被判为**最高报警等级**。煤二期现场当前运行的正是这一语义，两现场雷达配置（24 台雷达的方向与防御模式）逐一对应，故统一后行为与煤二期一致。

### 四、现场接入方式

各现场只需：

1. 引用 `ArsLibrary.dll`；
2. 业务代码加 `using ArsLibrary.Model;`；
3. 删除本地的 `Model/SensorGeneral.cs`、`Model/ClusterGeneral.cs`、`Model/ObjectGeneral.cs` 及 csproj 中对应的 `Compile` 项；
4. 雷达实体实现 `ISensorGeometryContext`（及可选的 `ISensorLimitContext`）。

### 五、变更记录（本次）

### 2026-10-27 — 点消息模型下沉到公共库

**新增文件：** `ArsLibrary/Model/SensorGeneral.cs`、`ArsLibrary/Model/ClusterGeneral.cs`、`ArsLibrary/Model/ObjectGeneral.cs`；`ArsLibrary/ArsLibrary.csproj` 新增 3 条 `Compile`

**修改文件：** `ArsLibrary/ArsLibrary.csproj`（新增 3 条 Compile）、`ArsLibrary/Model/ISensorGeometryContext.cs`（配套）、`ArsLibrary/Model/SensorGeometryContextDefault.cs`（配套）

**问题：** 点消息模型原先位于各现场项目内，煤二期与湛江各维护一份已分叉的同名类（`DistanceToBorder` 语义、序列化机制、字段均有差异），维护成本高且易产生行为分叉。

**修正内容：**
- 三个模型下沉到 `ArsLibrary.Model`，统一为煤二期语义。
- 序列化统一为 ProtoBuf（湛江原无标注，不影响行为）。
- 补入湛江独有的 `ArelLongRmsNum` / `ArelLatRmsNum` / `OrientationRmsNum` 三个字段，保证湛江业务侧 `Radar.cs` 的读取不报错。
- 配套的 `ISensorGeometryContext` / `ISensorLimitContext` / `SensorGeometryContextDefault` 为前置改造（上一轮已提交）。

**使用说明：** 现场接入只需 `using ArsLibrary.Model;` 并删除本地同名文件。**湛江统一后部分方向的目标距离会变为负值（与煤二期一致），负值在等级映射中判为最高报警级，上线前建议现场确认。**
