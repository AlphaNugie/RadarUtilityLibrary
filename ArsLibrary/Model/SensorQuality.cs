using ArsLibrary.Core;
using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 传感器重要信息基础类
    /// </summary>
    public abstract class SensorQuality : SensorMessage
    {
        #region 属性
        /// <summary>
        /// 目标ID（编号）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 纵向（x）坐标的标准差，米
        /// </summary>
        public SignalValue DistLongRms { get; set; }

        /// <summary>
        /// 横向（y）坐标的标准差，米
        /// </summary>
        public SignalValue DistLatRms { get; set; }

        /// <summary>
        /// 纵向的相对速度（x）的标准差，米/秒
        /// </summary>
        public SignalValue VrelLongRms { get; set; }

        /// <summary>
        /// 横向的相对速度（y）的标准差，米/秒
        /// </summary>
        public SignalValue VrelLatRms { get; set; }
        #endregion

        /// <summary>
        /// 返回一个当前对象的拷贝
        /// </summary>
        /// <returns></returns>
        public abstract SensorQuality Copy();
    }

    #region 枚举
    /// <summary>
    /// 纵向与横向高度、相对速度、相对加速度的标准差的范围
    /// </summary>
    public enum SignalValue
    {
        /// <summary>
        /// 无效值（指给出的值本身没有意义）
        /// </summary>
        [EnumDescription("无效值")]
        InvalidValue = -1,

        /// <summary>
        /// 小于0.005
        /// </summary>
        [EnumDescription("<0.005")]
        lt0005 = 0x0,

        /// <summary>
        /// 小于0.006
        /// </summary>
        [EnumDescription("<0.006")]
        lt0006 = 0x1,

        /// <summary>
        /// 小于0.008
        /// </summary>
        [EnumDescription("<0.008")]
        lt0008 = 0x2,

        /// <summary>
        /// 小于0.011
        /// </summary>
        [EnumDescription("<0.011")]
        lt0011 = 0x3,

        /// <summary>
        /// 小于0.014
        /// </summary>
        [EnumDescription("<0.014")]
        lt0014 = 0x4,

        /// <summary>
        /// 小于0.018
        /// </summary>
        [EnumDescription("<0.018")]
        lt0018 = 0x5,

        /// <summary>
        /// 小于0.023
        /// </summary>
        [EnumDescription("<0.023")]
        lt0023 = 0x6,

        /// <summary>
        /// 小于0.029
        /// </summary>
        [EnumDescription("<0.029")]
        lt0029 = 0x7,

        /// <summary>
        /// 小于0.038
        /// </summary>
        [EnumDescription("<0.038")]
        lt0038 = 0x8,

        /// <summary>
        /// 小于0.049
        /// </summary>
        [EnumDescription("<0.049")]
        lt0049 = 0x9,

        /// <summary>
        /// 小于0.063
        /// </summary>
        [EnumDescription("<0.063")]
        lt0063 = 0xA,

        /// <summary>
        /// 小于0.081
        /// </summary>
        [EnumDescription("<0.081")]
        lt0081 = 0xB,

        /// <summary>
        /// 小于0.105
        /// </summary>
        [EnumDescription("<0.105")]
        lt0105 = 0xC,

        /// <summary>
        /// 小于0.135
        /// </summary>
        [EnumDescription("<0.135")]
        lt0135 = 0xD,

        /// <summary>
        /// 小于0.174
        /// </summary>
        [EnumDescription("<0.174")]
        lt0174 = 0xE,

        /// <summary>
        /// 小于0.224
        /// </summary>
        [EnumDescription("<0.224")]
        lt0224 = 0xF,

        /// <summary>
        /// 小于0.288
        /// </summary>
        [EnumDescription("<0.288")]
        lt0288 = 0x10,

        /// <summary>
        /// 小于0.371
        /// </summary>
        [EnumDescription("<0.371")]
        lt0371 = 0x11,

        /// <summary>
        /// 小于0.478
        /// </summary>
        [EnumDescription("<0.478")]
        lt0478 = 0x12,

        /// <summary>
        /// 小于0.616
        /// </summary>
        [EnumDescription("<0.616")]
        lt0616 = 0x13,

        /// <summary>
        /// 小于0.794
        /// </summary>
        [EnumDescription("<0.794")]
        lt0794 = 0x14,

        /// <summary>
        /// 小于1.023
        /// </summary>
        [EnumDescription("<1.023")]
        lt1023 = 0x15,

        /// <summary>
        /// 小于1.317
        /// </summary>
        [EnumDescription("<1.317")]
        lt1317 = 0x16,

        /// <summary>
        /// 小于1.697
        /// </summary>
        [EnumDescription("<1.697")]
        lt1697 = 0x17,

        /// <summary>
        /// 小于2.187
        /// </summary>
        [EnumDescription("<2.187")]
        lt2187 = 0x18,

        /// <summary>
        /// 小于2.817
        /// </summary>
        [EnumDescription("<2.817")]
        lt2817 = 0x19,

        /// <summary>
        /// 小于3.630
        /// </summary>
        [EnumDescription("<3.630")]
        lt3630 = 0x1A,

        /// <summary>
        /// 小于4.676
        /// </summary>
        [EnumDescription("<4.676")]
        lt4676 = 0x1B,

        /// <summary>
        /// 小于6.025
        /// </summary>
        [EnumDescription("<6.025")]
        lt6025 = 0x1C,

        /// <summary>
        /// 小于7.762
        /// </summary>
        [EnumDescription("<7.762")]
        lt7762 = 0x1D,

        /// <summary>
        /// 小于10.000
        /// </summary>
        [EnumDescription("<10.000")]
        lt10000 = 0x1E,

        /// <summary>
        /// 无效数值
        /// </summary>
        [EnumDescription("无效数值")]
        Invalid = 0x1F
    }
    #endregion
}
