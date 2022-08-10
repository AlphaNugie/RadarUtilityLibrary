﻿using ArsLibrary.Core;
using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 目标基本信息实体类
    /// </summary>
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
        public ProbOfExist ProbOfExist
        {
            get { return prob_exist; }
            set
            {
                prob_exist = value;
                ProbOfExistString = prob_exist.GetDescription();
                ProbOfExistMinimum = double.Parse(prob_exist.GetAlias());
            }
        }

        /// <summary>
        /// 存在概率描述
        /// </summary>
        public string ProbOfExistString { get; set; }

        /// <summary>
        /// 存在概率的可能最小值
        /// </summary>
        public double ProbOfExistMinimum { get; set; }
        #endregion

        /// <summary>
        /// 基础信息初始化
        /// </summary>
        /// <param name="message">基础信息</param>
        /// <param name="radar">雷达信息</param>
        public ObjectGeneral(BaseMessage message, RadarBase radar) : base(message, radar) { }

        /// <summary>
        /// 基础信息初始化
        /// </summary>
        /// <param name="message">基础信息</param>
        public ObjectGeneral(BaseMessage message) : this(message, null) { }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public ObjectGeneral() : this(null, null) { }

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
