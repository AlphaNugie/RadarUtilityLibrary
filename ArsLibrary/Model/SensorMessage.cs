using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 传感器消息基础类
    /// </summary>
    public abstract class SensorMessage
    {
        private BaseMessage _base = new BaseMessage();

        /// <summary>
        /// 基础信息
        /// </summary>
        public BaseMessage Base
        {
            get { return _base; }
            set
            {
                _base = value;
                if (_base != null)
                    DataConvert(_base.DataString_Binary);
            }
        }

        /// <summary>
        /// 将从雷达收到的二进制数据转换为实际信息
        /// </summary>
        /// <param name="binary"></param>
        protected abstract void DataConvert(string binary);
    }
}
