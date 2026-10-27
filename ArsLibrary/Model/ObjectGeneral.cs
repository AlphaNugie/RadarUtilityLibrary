using ArsLibrary.Core;
using ArsLibrary.Model;
using CommonLib.Extensions;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 目标基本信息实体类
    /// </summary>
    [ProtoContract]
    public class ObjectGeneral : SensorGeneral
    {
        private MeasState meas_state = MeasState.New;
        private ProbOfExist prob_exist = ProbOfExist.Invalid;

        #region 属性
        /// <summary>
        /// 测量状态，指示目标是否有效
        /// </summary>
        public MeasState MeasState
        {
            get { return meas_state; }
            set
            {
                meas_state = value;
                MeasStateString = meas_state.GetDescription();
            }
        }

        /// <summary>
        /// 测量状态描述
        /// </summary>
        public string MeasStateString { get; set; }

        /// <summary>
        /// 存在概率
        /// </summary>
        public ProbOfExist ProbOfExist { get { return prob_exist; }
            set
            {
                prob_exist = value;
                ProbOfExistString = prob_exist.GetDescription();
                //ProbOfExistMinimum = double.Parse(prob_exist.GetAlias());
            }
        }

        /// <summary>
        /// 存在概率描述
        /// </summary>
        public string ProbOfExistString { get; set; }

        ///// <summary>
        ///// 存在概率的可能最小值
        ///// </summary>
        //public double ProbOfExistMinimum { get; set; }
        #endregion

        #region 加速度与方位角标准差（纯数值）

        /// <summary>
        /// 纵向（x）的相对加速度的标准差，单位：米/平方秒（纯数值）
        /// </summary>
        public double ArelLongRmsNum { get; set; }

        /// <summary>
        /// 横向（y）的相对加速度的标准差，单位：米/平方秒（纯数值）
        /// </summary>
        public double ArelLatRmsNum { get; set; }

        /// <summary>
        /// 方位角标准差（纯数值）
        /// </summary>
        public double OrientationRmsNum { get; set; }

        #endregion

        /// <summary>
        /// 构造器，基础信息初始化
        /// </summary>
        /// <param name="message">基础信息</param>
        /// <param name="radar">雷达几何上下文</param>
        public ObjectGeneral(BaseMessage message = null, ISensorGeometryContext radar = null) : base(message, radar) { }

        ///// <summary>
        ///// 基础信息初始化
        ///// </summary>
        ///// <param name="message">基础信息</param>
        //public ObjectGeneral(BaseMessage message) : base(message, null) { }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public ObjectGeneral() : base(null, null) { }

        /// <summary>
        /// 复制出一个同类型的新实例（不复制坐标与距离等派生数据）
        /// </summary>
        /// <returns>新实例</returns>
        public override SensorGeneral Copy()
        {
            ObjectGeneral general = new ObjectGeneral
            {
                Id = Id,
                DistLong = DistLong,
                DistLat = DistLat,
                VrelLong = VrelLong,
                VrelLat = VrelLat,
                DynProp = DynProp,
                RCS = RCS,
                PushfCounter = PushfCounter
            };
            return general;
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
                DistLong = Math.Round(0.2 * Convert.ToUInt16(binary.Substring(8, 13), 2) - 500, 1);
                DistLat = Math.Round(0.2 * Convert.ToUInt16(binary.Substring(21, 11), 2) - 204.6, 1);
                VrelLong = Math.Round(0.25 * Convert.ToUInt16(binary.Substring(32, 10), 2) - 128, 2);
                VrelLat = Math.Round(0.25 * Convert.ToUInt16(binary.Substring(42, 9), 2) - 64, 2);
                DynProp = (DynProp)Convert.ToByte(binary.Substring(53, 3), 2);
                RCS = 0.5 * Convert.ToUInt16(binary.Substring(56, 8), 2) - 64;
            }
            catch (Exception) { }
        }
    }
}
