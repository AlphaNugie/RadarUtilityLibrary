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
