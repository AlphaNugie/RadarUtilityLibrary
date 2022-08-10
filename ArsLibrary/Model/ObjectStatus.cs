﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 目标状态实体类
    /// </summary>
    public class ObjectStatus : SensorMessage
    {
        #region 属性
        /// <summary>
        /// 检测到的目标数量
        /// </summary>
        public byte NofObjects { get; set; }

        /// <summary>
        /// 测量周期基数器 (从传感器启动后开始计数，当超过65535时清零)
        /// </summary>
        public ushort MeasCounter { get; set; }

        /// <summary>
        /// 目标列表，CAN 接口版本
        /// </summary>
        public byte InterfaceVersion { get; set; }
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public ObjectStatus() { }

        /// <summary>
        /// 基础信息初始化
        /// </summary>
        /// <param name="message">基础信息</param>
        public ObjectStatus(BaseMessage message)
        {
            Base = message;
        }

        /// <summary>
        /// 转换2进制数据
        /// </summary>
        /// <param name="binary"></param>
        protected override void DataConvert(string binary)
        {
            try
            {
                NofObjects = Convert.ToByte(binary.Substring(0, 8), 2);
                MeasCounter = Convert.ToUInt16(binary.Substring(8, 16), 2);
                InterfaceVersion = Convert.ToByte(binary.Substring(24, 4), 2);
            }
            catch (Exception) { }
        }
    }
}
