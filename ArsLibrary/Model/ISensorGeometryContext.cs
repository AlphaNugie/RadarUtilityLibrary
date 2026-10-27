using CommonLib.Function.Fitting;
using ArsLibrary.Core;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 传感器几何上下文接口：提供点消息换算为单机坐标与边界距离所需的全部雷达参数
    /// </summary>
    /// <remarks>
    /// 用途：让点消息模型（如 SensorGeneral）不再直接依赖具体雷达实体类，
    /// 从而使该模型可以下沉到公共库并被多个现场项目共用。
    ///
    /// 设计约束：
    /// 1) 本接口只包含**只读纯数据属性**，不含任何行为，便于各现场以不同实现接入；
    /// 2) 各现场雷达实体的对应属性只需标注 get 访问器即可满足本接口
    ///    （C# 允许实现类把只读接口属性实现为只读属性，反之不成立），
    ///    因此现有 Radar 类无需修改属性的可写性，改动成本极低；
    /// 3) 属性名与语义在所有实现中保持一致，避免"同名不同义"。
    ///
    /// 实现方注意：属性取值必须与"雷达实体"保持同步（雷达参数在运行期可能被重新标定）。
    /// </remarks>
    public interface ISensorGeometryContext
    {
        #region 标识与姿态

        /// <summary>
        /// 雷达编号，-1 表示未设置、0 表示无效
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 雷达朝向：海北陆南上下
        /// </summary>
        Directions Direction { get; }

        /// <summary>
        /// 防御模式：1 点，2 线，3 面
        /// </summary>
        int DefenseMode { get; }

        #endregion

        #region 坐标偏移与转换系数

        /// <summary>
        /// X坐标（纵向）校正值，单位：米
        /// </summary>
        double XOffset { get; }

        /// <summary>
        /// Y坐标（横向）校正值，单位：米
        /// </summary>
        double YOffset { get; }

        /// <summary>
        /// Z坐标（高度）校正值，单位：米
        /// </summary>
        double ZOffset { get; }

        /// <summary>
        /// 用于计算修改后X坐标的原XYZ坐标系数
        /// </summary>
        CoordinateRatios XmodifiedRatios { get; }

        /// <summary>
        /// 用于计算修改后Y坐标的原XYZ坐标系数
        /// </summary>
        CoordinateRatios YmodifiedRatios { get; }

        /// <summary>
        /// 用于计算修改后Z坐标的原XYZ坐标系数
        /// </summary>
        CoordinateRatios ZmodifiedRatios { get; }

        #endregion

        #region 检测范围（雷达坐标）

        /// <summary>
        /// 雷达坐标纵向（x）下限，单位：米
        /// </summary>
        double RadarxMin { get; }

        /// <summary>
        /// 雷达坐标纵向（x）上限，单位：米
        /// </summary>
        double RadarxMax { get; }

        /// <summary>
        /// 雷达坐标横向（y）下限，单位：米
        /// </summary>
        double RadaryMin { get; }

        /// <summary>
        /// 雷达坐标横向（y）上限，单位：米
        /// </summary>
        double RadaryMax { get; }

        #endregion

        #region 检测范围（单机坐标）

        /// <summary>
        /// 单机坐标X下限，单位：米
        /// </summary>
        double ClaimerxMin { get; }

        /// <summary>
        /// 单机坐标X上限，单位：米
        /// </summary>
        double ClaimerxMax { get; }

        /// <summary>
        /// 单机坐标Y下限，单位：米
        /// </summary>
        double ClaimeryMin { get; }

        /// <summary>
        /// 单机坐标Y上限，单位：米
        /// </summary>
        double ClaimeryMax { get; }

        /// <summary>
        /// 单机坐标Z下限，单位：米
        /// </summary>
        double ClaimerzMin { get; }

        /// <summary>
        /// 单机坐标Z上限，单位：米
        /// </summary>
        double ClaimerzMax { get; }

        #endregion

        #region 角度范围

        /// <summary>
        /// 角度下限，单位：度
        /// </summary>
        double AngleMin { get; }

        /// <summary>
        /// 角度上限，单位：度
        /// </summary>
        double AngleMax { get; }

        #endregion
    }

    /// <summary>
    /// 限制开关接口：控制各检测范围判定是否生效
    /// </summary>
    /// <remarks>
    /// 抽出为独立接口的原因：并非所有现场的雷达实体都具备这三个开关
    /// （例如部分现场在早期版本中将其注释掉、未启用）。
    /// 点消息侧通过 <c>as ISensorLimitContext</c> 探测实现：
    /// 未实现本接口时，视为三个开关全部生效（即不反转判定结果），与"开关为真"的语义一致。
    /// </remarks>
    public interface ISensorLimitContext
    {
        /// <summary>
        /// 雷达坐标范围限制是否生效
        /// </summary>
        bool WithinRadarLimit { get; }

        /// <summary>
        /// 单机坐标范围限制是否生效
        /// </summary>
        bool WithinClaimerLimit { get; }

        /// <summary>
        /// 角度范围限制是否生效
        /// </summary>
        bool WithinAngleLimit { get; }
    }
}
