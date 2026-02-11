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
    /// 目标重要信息
    /// </summary>
    public class ObjectQuality : SensorQuality
    {
        #region 静态属性，过滤器
        /// <summary>
        /// 测量状态过滤器
        /// </summary>
        public static List<MeasState> MeasStateFilter { get; set; }

        /// <summary>
        /// 存在概率过滤器
        /// </summary>
        public static List<ProbOfExist> ProbOfExistFilter { get; set; }
        #endregion

        #region 属性
        #region 标准差
        /// <summary>
        /// 纵向（x）的相对加速度的标准差，米/平方秒
        /// </summary>
        //[PropertyMapperTo("ArelLongRms")]
        //public SignalValueEnum ArelLongRms { get; set; }
        public SignalValue ArelLongRms { get; set; }

        /// <summary>
        /// 横向（y）的相对加速度的标准差，米/平方秒
        /// </summary>
        //[PropertyMapperTo("ArelLatRms")]
        //public SignalValueEnum ArelLatRms { get; set; }
        public SignalValue ArelLatRms { get; set; }

        /// <summary>
        /// 方位角标准差
        /// </summary>
        //[PropertyMapperTo("OrientationRms")]
        //public SignalValueEnum_Degree OrientationRms { get; set; }
        public SignalValue_Degree OrientationRms { get; set; }
        #endregion

        #region 标准差（纯数值）
        /// <summary>
        /// 纵向（x）的相对加速度的标准差，米/平方秒（纯数值）
        /// </summary>
        [PropertyMapperTo("ArelLongRmsNum")]
        public double ArelLongRmsNum { get { return ArelLongRms == null || !ArelLongRms.Valid ? -1 : ArelLongRms.Value.Value; } }

        /// <summary>
        /// 横向（y）的相对加速度的标准差，米/平方秒（纯数值）
        /// </summary>
        [PropertyMapperTo("ArelLatRmsNum")]
        public double ArelLatRmsNum { get { return ArelLatRms == null || !ArelLatRms.Valid ? -1 : ArelLatRms.Value.Value; } }

        /// <summary>
        /// 方位角标准差（纯数值）
        /// </summary>
        [PropertyMapperTo("OrientationRmsNum")]
        public double OrientationRmsNum { get { return OrientationRms == null || !OrientationRms.Valid ? -1 : OrientationRms.Value.Value; } }
        #endregion

        /// <summary>
        /// 测量状态，指示目标是否有效
        /// </summary>
        [PropertyMapperTo("MeasState")]
        public MeasState MeasState { get; set; }

        /// <summary>
        /// 存在概率
        /// </summary>
        [PropertyMapperTo("ProbOfExist")]
        public ProbOfExist ProbOfExist { get; set; }
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public ObjectQuality() { }

        /// <summary>
        /// 基础信息初始化
        /// </summary>
        /// <param name="message">基础信息</param>
        public ObjectQuality(BaseMessage message)
        {
            Base = message;
        }

        /// <summary>
        /// 获取目标质量信息的副本
        /// </summary>
        /// <returns></returns>
        public override SensorQuality Copy()
        {
            return new ObjectQuality
            {
                Id = Id,
                DistLongRms = DistLongRms,
                DistLatRms = DistLatRms,
                VrelLongRms = VrelLongRms,
                VrelLatRms = VrelLatRms,
                ArelLongRms = ArelLongRms,
                ArelLatRms = ArelLatRms,
                OrientationRms = OrientationRms,
                ProbOfExist = ProbOfExist,
                MeasState = MeasState
            };
        }

        /// <summary>
        /// 转换2进制数据
        /// </summary>
        /// <param name="binary"></param>
        protected override void DataConvert(string binary)
        {
            try
            {
                Id = Convert.ToByte(binary.Substring(0, 8), 2);
                DistLongRms = new SignalValue((SignalValueEnum)Convert.ToByte(binary.Substring(8, 5), 2));
                DistLatRms = new SignalValue((SignalValueEnum)Convert.ToByte(binary.Substring(13, 5), 2));
                VrelLongRms = new SignalValue((SignalValueEnum)Convert.ToByte(binary.Substring(18, 5), 2));
                VrelLatRms = new SignalValue((SignalValueEnum)Convert.ToByte(binary.Substring(23, 5), 2));
                ArelLongRms = new SignalValue((SignalValueEnum)Convert.ToByte(binary.Substring(28, 5), 2));
                ArelLatRms = new SignalValue((SignalValueEnum)Convert.ToByte(binary.Substring(33, 5), 2));
                OrientationRms = new SignalValue_Degree((SignalValueEnum_Degree)Convert.ToByte(binary.Substring(38, 5), 2));
                ProbOfExist = (ProbOfExist)Convert.ToByte(binary.Substring(48, 3), 2);
                MeasState = (MeasState)Convert.ToByte(binary.Substring(51, 3), 2);
            }
            catch (Exception) { }
        }
    }

    /// <summary>
    /// Object航向角的标准差的值
    /// </summary>
    public class SignalValue_Degree
    {
        private SignalValueEnum_Degree _enum;
        /// <summary>
        /// 标准差范围的枚举值
        /// </summary>
        public SignalValueEnum_Degree Enum
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
        private bool EnumValid { get { return _enum != SignalValueEnum_Degree.InvalidValue && _enum != SignalValueEnum_Degree.Invalid; } }

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
        public SignalValue_Degree(SignalValueEnum_Degree @enum)
        {
            Enum = @enum;
        }
    }

    #region 枚举
    /// <summary>
    /// Object航向角的标准差的范围
    /// </summary>
    public enum SignalValueEnum_Degree
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
        /// 小于0.007
        /// </summary>
        [EnumDescription("<0.007")]
        [EnumValue("0.0069")]
        lt0007 = 0x1,

        /// <summary>
        /// 小于0.010
        /// </summary>
        [EnumDescription("<0.010")]
        [EnumValue("0.0099")]
        lt0010 = 0x2,

        /// <summary>
        /// 小于0.014
        /// </summary>
        [EnumDescription("<0.014")]
        [EnumValue("0.0139")]
        lt0014 = 0x3,

        /// <summary>
        /// 小于0.020
        /// </summary>
        [EnumDescription("<0.020")]
        [EnumValue("0.0199")]
        lt0020 = 0x4,

        /// <summary>
        /// 小于0.029
        /// </summary>
        [EnumDescription("<0.029")]
        [EnumValue("0.0289")]
        lt0029 = 0x5,

        /// <summary>
        /// 小于0.041
        /// </summary>
        [EnumDescription("<0.041")]
        [EnumValue("0.0409")]
        lt0041 = 0x6,

        /// <summary>
        /// 小于0.058
        /// </summary>
        [EnumDescription("<0.058")]
        [EnumValue("0.0579")]
        lt0058 = 0x7,

        /// <summary>
        /// 小于0.082
        /// </summary>
        [EnumDescription("<0.082")]
        [EnumValue("0.0819")]
        lt0082 = 0x8,

        /// <summary>
        /// 小于0.116
        /// </summary>
        [EnumDescription("<0.116")]
        [EnumValue("0.1159")]
        lt0116 = 0x9,

        /// <summary>
        /// 小于0.165
        /// </summary>
        [EnumDescription("<0.165")]
        [EnumValue("0.1649")]
        lt0165 = 0xA,

        /// <summary>
        /// 小于0.234
        /// </summary>
        [EnumDescription("<0.234")]
        [EnumValue("0.2339")]
        lt0234 = 0xB,

        /// <summary>
        /// 小于0.332
        /// </summary>
        [EnumDescription("<0.332")]
        [EnumValue("0.3319")]
        lt0332 = 0xC,

        /// <summary>
        /// 小于0.471
        /// </summary>
        [EnumDescription("<0.471")]
        [EnumValue("0.4709")]
        lt0471 = 0xD,

        /// <summary>
        /// 小于0.669
        /// </summary>
        [EnumDescription("<0.669")]
        [EnumValue("0.6689")]
        lt0669 = 0xE,

        /// <summary>
        /// 小于0.949
        /// </summary>
        [EnumDescription("<0.949")]
        [EnumValue("0.9489")]
        lt0949 = 0xF,

        /// <summary>
        /// 小于1.346
        /// </summary>
        [EnumDescription("<1.346")]
        [EnumValue("1.3459")]
        lt1346 = 0x10,

        /// <summary>
        /// 小于1.909
        /// </summary>
        [EnumDescription("<1.909")]
        [EnumValue("1.9089")]
        lt1909 = 0x11,

        /// <summary>
        /// 小于2.709
        /// </summary>
        [EnumDescription("<2.709")]
        [EnumValue("2.7089")]
        lt2709 = 0x12,

        /// <summary>
        /// 小于3.843
        /// </summary>
        [EnumDescription("<3.843")]
        [EnumValue("3.8429")]
        lt3843 = 0x13,

        /// <summary>
        /// 小于5.451
        /// </summary>
        [EnumDescription("<5.451")]
        [EnumValue("5.4509")]
        lt5451 = 0x14,

        /// <summary>
        /// 小于7.734
        /// </summary>
        [EnumDescription("<7.734")]
        [EnumValue("7.7339")]
        lt7734 = 0x15,

        /// <summary>
        /// 小于10.971
        /// </summary>
        [EnumDescription("<10.971")]
        [EnumValue("10.9709")]
        lt10971 = 0x16,

        /// <summary>
        /// 小于5.565
        /// </summary>
        [EnumDescription("<5.565")]
        [EnumValue("5.5649")]
        lt15565 = 0x17,

        /// <summary>
        /// 小于22.081
        /// </summary>
        [EnumDescription("<22.081")]
        [EnumValue("22.0809")]
        lt22081 = 0x18,

        /// <summary>
        /// 小于31.325
        /// </summary>
        [EnumDescription("<31.325")]
        [EnumValue("31.3249")]
        lt31325 = 0x19,

        /// <summary>
        /// 小于44.439
        /// </summary>
        [EnumDescription("<44.439")]
        [EnumValue("44.4389")]
        lt44439 = 0x1A,

        /// <summary>
        /// 小于63.044
        /// </summary>
        [EnumDescription("<63.044")]
        [EnumValue("63.0439")]
        lt63044 = 0x1B,

        /// <summary>
        /// 小于89.437
        /// </summary>
        [EnumDescription("<89.437")]
        [EnumValue("89.4369")]
        lt89437 = 0x1C,

        /// <summary>
        /// 小于126.881
        /// </summary>
        [EnumDescription("<126.881")]
        [EnumValue("126.8809")]
        lt126881 = 0x1D,

        /// <summary>
        /// 小于180.000
        /// </summary>
        [EnumDescription("<180.000")]
        [EnumValue("179.9999")]
        lt180000 = 0x1E,

        /// <summary>
        /// 无效数值
        /// </summary>
        [EnumDescription("无效数值")]
        [EnumValue("null")]
        Invalid = 0x1F
    }

    ///// <summary>
    ///// 测量状态，指示目标是否有效
    ///// </summary>
    //public enum MeasState
    //{
    //    /// <summary>
    //    /// 无效值（指给出的值本身没有意义）
    //    /// </summary>
    //    [EnumDescription("无效值")]
    //    InvalidValue = -1,

    //    /// <summary>
    //    /// 被删除，ID消失前的最后一轮数据传输中出现
    //    /// </summary>
    //    [EnumDescription("被删除")]
    //    Deleted = 0,

    //    /// <summary>
    //    /// 新出现，ID创建后的第一轮数据传输中出现
    //    /// </summary>
    //    [EnumDescription("新出现")]
    //    New = 1,

    //    /// <summary>
    //    /// 已测量，目标的出现被实际测量证实
    //    /// </summary>
    //    [EnumDescription("已测量")]
    //    Measured = 2,

    //    /// <summary>
    //    /// 预测的，目标的出现无法被实际测量证实
    //    /// </summary>
    //    [EnumDescription("预测的")]
    //    Predicted = 3,

    //    /// <summary>
    //    /// 为合并删除，为与另一个目标合并而被删除
    //    /// </summary>
    //    [EnumDescription("为合并删除")]
    //    DeletedForMerge = 4,

    //    /// <summary>
    //    /// 合并为新的，合并后产生的新目标
    //    /// </summary>
    //    [EnumDescription("合并为新的")]
    //    NewFromMerge = 5
    //}

    ///// <summary>
    ///// 存在概率
    ///// </summary>
    //public enum ProbOfExist
    //{
    //    /// <summary>
    //    /// 无效值（指给出的值本身没有意义）
    //    /// </summary>
    //    [EnumDescription("无效值")]
    //    InvalidValue = -1,

    //    /// <summary>
    //    /// 无效
    //    /// </summary>
    //    [EnumDescription("无效")]
    //    [EnumAlias("-1")]
    //    Invalid = 0,

    //    /// <summary>
    //    /// 小于25%
    //    /// </summary>
    //    [EnumDescription("<25%")]
    //    [EnumAlias("0")]
    //    Lt025 = 1,

    //    /// <summary>
    //    /// 小于50%
    //    /// </summary>
    //    [EnumDescription("<50%")]
    //    [EnumAlias("0.25")]
    //    Lt050 = 2,

    //    /// <summary>
    //    /// 小于75%
    //    /// </summary>
    //    [EnumDescription("<75%")]
    //    [EnumAlias("0.5")]
    //    Lt075 = 3,

    //    /// <summary>
    //    /// 小于90%
    //    /// </summary>
    //    [EnumDescription("<90%")]
    //    [EnumAlias("0.75")]
    //    Lt090 = 4,

    //    /// <summary>
    //    /// 小于99%
    //    /// </summary>
    //    [EnumDescription("<99%")]
    //    [EnumAlias("0.9")]
    //    Lt099 = 5,

    //    /// <summary>
    //    /// 小于99.9%
    //    /// </summary>
    //    [EnumDescription("<99.9%")]
    //    [EnumAlias("0.99")]
    //    Lt999 = 6,

    //    /// <summary>
    //    /// 小于等于100%
    //    /// </summary>
    //    [EnumDescription("<=100%")]
    //    [EnumAlias("0.999")]
    //    Lte100 = 7
    //}
    #endregion
}
