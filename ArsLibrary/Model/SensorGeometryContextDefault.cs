using CommonLib.Function.Fitting;
using ArsLibrary.Core;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 默认的传感器几何上下文：用于未指定雷达时的兜底
    /// </summary>
    /// <remarks>
    /// 作用：点消息模型在未关联到具体雷达时需要一个非空的上下文，
    /// 否则访问坐标偏移等属性会抛空引用。
    ///
    /// 取值约定（与"新建一个默认雷达实体"的历史行为保持一致）：
    /// <list type="bullet">
    /// <item>Id = -1：表示未设置，点消息据此跳过坐标换算与范围判定</item>
    /// <item>各偏移量为 0、比例为 0：坐标换算结果退化为原值</item>
    /// <item>Direction = None、DefenseMode = 0：不做任何方向与防御模式解释</item>
    /// <item>各范围上下限为 0：范围判定不会误判为"在范围内"</item>
    /// </list>
    /// </remarks>
    public class SensorGeometryContextDefault : ISensorGeometryContext, ISensorLimitContext
    {
        #region 标识与姿态

        /// <summary>
        /// 雷达编号，-1 表示未设置
        /// </summary>
        public int Id { get; set; } = -1;

        /// <summary>
        /// 雷达朝向：海北陆南上下，默认为无方向
        /// </summary>
        public Directions Direction { get; set; } = Directions.None;

        /// <summary>
        /// 防御模式：1 点，2 线，3 面，默认为 0（不解释）
        /// </summary>
        public int DefenseMode { get; set; }

        #endregion

        #region 坐标偏移与转换系数

        /// <summary>
        /// X坐标（纵向）校正值，单位：米
        /// </summary>
        public double XOffset { get; set; }

        /// <summary>
        /// Y坐标（横向）校正值，单位：米
        /// </summary>
        public double YOffset { get; set; }

        /// <summary>
        /// Z坐标（高度）校正值，单位：米
        /// </summary>
        public double ZOffset { get; set; }

        /// <summary>
        /// 用于计算修改后X坐标的原XYZ坐标系数
        /// </summary>
        public CoordinateRatios XmodifiedRatios { get; set; } = new CoordinateRatios();

        /// <summary>
        /// 用于计算修改后Y坐标的原XYZ坐标系数
        /// </summary>
        public CoordinateRatios YmodifiedRatios { get; set; } = new CoordinateRatios();

        /// <summary>
        /// 用于计算修改后Z坐标的原XYZ坐标系数
        /// </summary>
        public CoordinateRatios ZmodifiedRatios { get; set; } = new CoordinateRatios();

        #endregion

        #region 检测范围（雷达坐标）

        /// <summary>
        /// 雷达坐标纵向（x）下限，单位：米
        /// </summary>
        public double RadarxMin { get; set; }

        /// <summary>
        /// 雷达坐标纵向（x）上限，单位：米
        /// </summary>
        public double RadarxMax { get; set; }

        /// <summary>
        /// 雷达坐标横向（y）下限，单位：米
        /// </summary>
        public double RadaryMin { get; set; }

        /// <summary>
        /// 雷达坐标横向（y）上限，单位：米
        /// </summary>
        public double RadaryMax { get; set; }

        #endregion

        #region 检测范围（单机坐标）

        /// <summary>
        /// 单机坐标X下限，单位：米
        /// </summary>
        public double ClaimerxMin { get; set; }

        /// <summary>
        /// 单机坐标X上限，单位：米
        /// </summary>
        public double ClaimerxMax { get; set; }

        /// <summary>
        /// 单机坐标Y下限，单位：米
        /// </summary>
        public double ClaimeryMin { get; set; }

        /// <summary>
        /// 单机坐标Y上限，单位：米
        /// </summary>
        public double ClaimeryMax { get; set; }

        /// <summary>
        /// 单机坐标Z下限，单位：米
        /// </summary>
        public double ClaimerzMin { get; set; }

        /// <summary>
        /// 单机坐标Z上限，单位：米
        /// </summary>
        public double ClaimerzMax { get; set; }

        #endregion

        #region 角度范围

        /// <summary>
        /// 角度下限，单位：度
        /// </summary>
        public double AngleMin { get; set; }

        /// <summary>
        /// 角度上限，单位：度
        /// </summary>
        public double AngleMax { get; set; }

        #endregion

        #region 限制开关

        /// <summary>
        /// 雷达坐标范围限制是否生效，默认生效
        /// </summary>
        public bool WithinRadarLimit { get; set; } = true;

        /// <summary>
        /// 单机坐标范围限制是否生效，默认生效
        /// </summary>
        public bool WithinClaimerLimit { get; set; } = true;

        /// <summary>
        /// 角度范围限制是否生效，默认生效
        /// </summary>
        public bool WithinAngleLimit { get; set; } = true;

        #endregion
    }
}
