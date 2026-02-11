using ArsLibrary.Core;
using ArsLibrary.Core.Rhd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model.Rhd
{
    /// <summary>
    /// 表示单个Object目标的结构化数据
    /// </summary>
    public class ObjectData : Data
    {
        ///// <summary>
        ///// 保留字段（目标编号之后的1个字节）
        ///// </summary>
        //public byte ReservedByte { get; set; }

        /// <summary>
        /// 目标类型
        /// <para/>对于协议v2.1.3为保留字段，默认为0（<see cref="ObjectType.Unknown"/>，即未知类型）
        /// </summary>
        public ObjectType Type { get; internal set; } = ObjectType.Unknown;

        /// <summary>
        /// 目标长度 (单位：米，分辨率0.5)
        /// </summary>
        public double Length { get; internal set; }

        /// <summary>
        /// 目标宽度 (单位：米，分辨率0.5)
        /// </summary>
        public double Width { get; internal set; }

        /// <summary>
        /// 目标高度 (单位：米，分辨率0.5)
        /// </summary>
        public double Height { get; internal set; }

        //X轴坐标
        //Y轴坐标
        //Z轴坐标
        //X轴速度
        //Y轴速度
        //Z轴速度

        /// <summary>
        /// 目标运动方向速度 (单位：米/秒，分辨率0.02)
        /// </summary>
        public double Speed { get; internal set; }

        /// <summary>
        /// X轴加速度（单位：米/秒^2，分辨率0.01）
        /// <para/>对于协议v2.1.3为保留字段，默认为0
        /// </summary>
        public double Ax { get; internal set; }

        /// <summary>
        /// Y轴加速度（单位：米/秒^2，分辨率0.01）
        /// <para/>对于协议v2.1.3为保留字段，默认为0
        /// </summary>
        public double Ay { get; internal set; }

        /// <summary>
        /// Z轴加速度（单位：米/秒^2，分辨率0.01）
        /// <para/>对于协议v2.1.3为保留字段，默认为0
        /// </summary>
        public double Az { get; internal set; }

        /// <summary>
        /// 运动方向加速度（单位：米/秒^2，分辨率0.01）
        /// <para/>对于协议v2.1.3为保留字段，默认为0
        /// </summary>
        public double Acceleration { get; internal set; }

        /// <summary>
        /// 目标航向角 (单位：度，分辨率0.1)，从正北方向顺时针计算
        /// </summary>
        public double CourseAngle { get; internal set; }

        /// <summary>
        /// 经度 (单位：度，分辨率1e-8)，东经为正
        /// </summary>
        public double Longitude { get; internal set; }

        /// <summary>
        /// 纬度 (单位：度，分辨率1e-8)，北纬为正
        /// </summary>
        public double Latitude { get; internal set; }

        /// <summary>
        /// 海拔高度 (单位：米，分辨率0.1)
        /// </summary>
        public double Altitude { get; internal set; }

        //RCS值

        /// <summary>
        /// 置信度（存在概率）
        /// <para/>对于协议v2.1.3为保留字段，默认为7（<see cref="ProbOfExist.Lte100"/>，即小于等于100%）
        /// </summary>
        public ProbOfExist Confidence { get; internal set; } = ProbOfExist.Lte100;

        /// <summary>
        /// 检测状态
        /// <para/>仅在协议v2.1.2中存在，对于协议v2.1.3默认为2（<see cref="MeasState.Measured"/>，即已测量的目标）
        /// </summary>
        public MeasState MeasState { get; internal set; } = MeasState.Measured;
    }
}
