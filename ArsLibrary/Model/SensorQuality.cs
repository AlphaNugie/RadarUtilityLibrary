using ArsLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 传感器质量信息基础类
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
    }
}
