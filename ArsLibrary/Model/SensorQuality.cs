using ArsLibrary.Core;
using CommonLib.Extensions;
using CommonLib.Extensions.Property;
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
        #region 静态属性，标准差上限
        /// <summary>
        /// 纵向坐标（单位：米）的标准差上限
        /// </summary>
        public static double DistLongRmsMax { get; set; }

        /// <summary>
        /// 横向坐标（单位：米）的标准差上限
        /// </summary>
        public static double DistLatRmsMax { get; set; }
        #endregion

        #region 属性
        /// <summary>
        /// 目标ID（编号）
        /// </summary>
        public int Id { get; set; }

        #region 标准差
        /// <summary>
        /// 纵向（x）坐标（单位：米）的标准差
        /// </summary>
        //public SignalValueEnum DistLongRms { get; set; }
        public SignalValue DistLongRms { get; set; }

        /// <summary>
        /// 横向（y）坐标（单位：米）的标准差
        /// </summary>
        //public SignalValueEnum DistLatRms { get; set; }
        public SignalValue DistLatRms { get; set; }

        /// <summary>
        /// 纵向（x）的相对速度（单位：米/秒）的标准差
        /// </summary>
        //public SignalValueEnum VrelLongRms { get; set; }
        public SignalValue VrelLongRms { get; set; }

        /// <summary>
        /// 横向（y）的相对速度（单位：米/秒）的标准差
        /// </summary>
        //public SignalValueEnum VrelLatRms { get; set; }
        public SignalValue VrelLatRms { get; set; }
        #endregion

        #region 标准差（纯数值）
        /// <summary>
        /// 纵向（x）坐标（单位：米）的标准差（纯数值）
        /// </summary>
        [PropertyMapperTo("DistLongRmsNum")]
        public double DistLongRmsNum { get { return DistLongRms == null || !DistLongRms.Valid ? -1 : DistLongRms.Value.Value; } }

        /// <summary>
        /// 横向（y）坐标（单位：米）的标准差（纯数值）
        /// </summary>
        [PropertyMapperTo("DistLatRmsNum")]
        public double DistLatRmsNum { get { return DistLatRms == null || !DistLatRms.Valid ? -1 : DistLatRms.Value.Value; } }

        /// <summary>
        /// 纵向（x）的相对速度（单位：米/秒）的标准差（纯数值）
        /// </summary>
        [PropertyMapperTo("VrelLongRmsNum")]
        public double VrelLongRmsNum { get { return VrelLongRms == null || !VrelLongRms.Valid ? -1 : VrelLongRms.Value.Value; } }

        /// <summary>
        /// 横向（y）的相对速度（单位：米/秒）的标准差（纯数值）
        /// </summary>
        [PropertyMapperTo("VrelLatRmsNum")]
        public double VrelLatRmsNum { get { return VrelLatRms == null || !VrelLatRms.Valid ? -1 : VrelLatRms.Value.Value; } }
        #endregion
        #endregion

        /// <summary>
        /// 返回一个当前对象的拷贝
        /// </summary>
        /// <returns></returns>
        public abstract SensorQuality Copy();
    }

    /// <summary>
    /// 纵向与横向高度、相对速度、相对加速度的标准差的值
    /// </summary>
    public class SignalValue
    {
        private SignalValueEnum _enum;
        /// <summary>
        /// 标准差范围的枚举值
        /// </summary>
        public SignalValueEnum Enum
        {
            get { return _enum; }
            set
            {
                _enum = value;
                if (EnumValid) Value = _enum.GetValue();
            }
        }

        /// <summary>
        /// 当前枚举是否代表有效
        /// </summary>
        private bool EnumValid { get { return _enum != SignalValueEnum.InvalidValue && _enum != SignalValueEnum.Invalid; } }

        /// <summary>
        /// 当前值是否有效
        /// </summary>
        public bool Valid { get { return Value.HasValue; } }

        /// <summary>
        /// 标准差的值
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="enum">标准差范围的枚举</param>
        public SignalValue(SignalValueEnum @enum)
        {
            Enum = @enum;
        }
    }

    #region 枚举
    /// <summary>
    /// 纵向与横向高度、相对速度、相对加速度的标准差的范围
    /// </summary>
    public enum SignalValueEnum
    {
        /// <summary>
        /// 无效值（指给出的值本身没有意义）
        /// </summary>
        [EnumDescription("无效值")]
        [EnumValue("null")]
        InvalidValue = -1,

        /// <summary>
        /// 小于0.005
        /// </summary>
        [EnumDescription("<0.005")]
        [EnumValue("0.0049")]
        lt0005 = 0x0,

        /// <summary>
        /// 小于0.006
        /// </summary>
        [EnumDescription("<0.006")]
        [EnumValue("0.0059")]
        lt0006 = 0x1,

        /// <summary>
        /// 小于0.008
        /// </summary>
        [EnumDescription("<0.008")]
        [EnumValue("0.0079")]
        lt0008 = 0x2,

        /// <summary>
        /// 小于0.011
        /// </summary>
        [EnumDescription("<0.011")]
        [EnumValue("0.0109")]
        lt0011 = 0x3,

        /// <summary>
        /// 小于0.014
        /// </summary>
        [EnumDescription("<0.014")]
        [EnumValue("0.0139")]
        lt0014 = 0x4,

        /// <summary>
        /// 小于0.018
        /// </summary>
        [EnumDescription("<0.018")]
        [EnumValue("0.0179")]
        lt0018 = 0x5,

        /// <summary>
        /// 小于0.023
        /// </summary>
        [EnumDescription("<0.023")]
        [EnumValue("0.0229")]
        lt0023 = 0x6,

        /// <summary>
        /// 小于0.029
        /// </summary>
        [EnumDescription("<0.029")]
        [EnumValue("0.0289")]
        lt0029 = 0x7,

        /// <summary>
        /// 小于0.038
        /// </summary>
        [EnumDescription("<0.038")]
        [EnumValue("0.0379")]
        lt0038 = 0x8,

        /// <summary>
        /// 小于0.049
        /// </summary>
        [EnumDescription("<0.049")]
        [EnumValue("0.0489")]
        lt0049 = 0x9,

        /// <summary>
        /// 小于0.063
        /// </summary>
        [EnumDescription("<0.063")]
        [EnumValue("0.0629")]
        lt0063 = 0xA,

        /// <summary>
        /// 小于0.081
        /// </summary>
        [EnumDescription("<0.081")]
        [EnumValue("0.0809")]
        lt0081 = 0xB,

        /// <summary>
        /// 小于0.105
        /// </summary>
        [EnumDescription("<0.105")]
        [EnumValue("0.1049")]
        lt0105 = 0xC,

        /// <summary>
        /// 小于0.135
        /// </summary>
        [EnumDescription("<0.135")]
        [EnumValue("0.1349")]
        lt0135 = 0xD,

        /// <summary>
        /// 小于0.174
        /// </summary>
        [EnumDescription("<0.174")]
        [EnumValue("0.1739")]
        lt0174 = 0xE,

        /// <summary>
        /// 小于0.224
        /// </summary>
        [EnumDescription("<0.224")]
        [EnumValue("0.2239")]
        lt0224 = 0xF,

        /// <summary>
        /// 小于0.288
        /// </summary>
        [EnumDescription("<0.288")]
        [EnumValue("0.2879")]
        lt0288 = 0x10,

        /// <summary>
        /// 小于0.371
        /// </summary>
        [EnumDescription("<0.371")]
        [EnumValue("0.3709")]
        lt0371 = 0x11,

        /// <summary>
        /// 小于0.478
        /// </summary>
        [EnumDescription("<0.478")]
        [EnumValue("0.4779")]
        lt0478 = 0x12,

        /// <summary>
        /// 小于0.616
        /// </summary>
        [EnumDescription("<0.616")]
        [EnumValue("0.6159")]
        lt0616 = 0x13,

        /// <summary>
        /// 小于0.794
        /// </summary>
        [EnumDescription("<0.794")]
        [EnumValue("0.7939")]
        lt0794 = 0x14,

        /// <summary>
        /// 小于1.023
        /// </summary>
        [EnumDescription("<1.023")]
        [EnumValue("1.0229")]
        lt1023 = 0x15,

        /// <summary>
        /// 小于1.317
        /// </summary>
        [EnumDescription("<1.317")]
        [EnumValue("1.3169")]
        lt1317 = 0x16,

        /// <summary>
        /// 小于1.697
        /// </summary>
        [EnumDescription("<1.697")]
        [EnumValue("1.6969")]
        lt1697 = 0x17,

        /// <summary>
        /// 小于2.187
        /// </summary>
        [EnumDescription("<2.187")]
        [EnumValue("2.1869")]
        lt2187 = 0x18,

        /// <summary>
        /// 小于2.817
        /// </summary>
        [EnumDescription("<2.817")]
        [EnumValue("2.8169")]
        lt2817 = 0x19,

        /// <summary>
        /// 小于3.630
        /// </summary>
        [EnumDescription("<3.630")]
        [EnumValue("3.6299")]
        lt3630 = 0x1A,

        /// <summary>
        /// 小于4.676
        /// </summary>
        [EnumDescription("<4.676")]
        [EnumValue("4.6759")]
        lt4676 = 0x1B,

        /// <summary>
        /// 小于6.025
        /// </summary>
        [EnumDescription("<6.025")]
        [EnumValue("6.0249")]
        lt6025 = 0x1C,

        /// <summary>
        /// 小于7.762
        /// </summary>
        [EnumDescription("<7.762")]
        [EnumValue("7.7619")]
        lt7762 = 0x1D,

        /// <summary>
        /// 小于10.000
        /// </summary>
        [EnumDescription("<10.000")]
        [EnumValue("9.9999")]
        lt10000 = 0x1E,

        /// <summary>
        /// 无效数值
        /// </summary>
        [EnumDescription("无效数值")]
        [EnumValue("null")]
        Invalid = 0x1F
    }
    #endregion
}
