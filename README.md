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
