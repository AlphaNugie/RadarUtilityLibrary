using CommonLib.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model.Rhd
{
    /// <summary>
    /// 数据包类型枚举
    /// </summary>
    public enum PacketType
    {
        /// <summary>Cluster数据包</summary>
        Cluster,
        /// <summary>Object数据包</summary>
        Object
    }

    /// <summary>
    /// 数据包基类，提供类型标识和抽象访问
    /// </summary>
    public abstract class PacketBase
    {
        /// <summary>
        /// 数据生成时间 (UTC毫秒时间戳)
        /// </summary>
        public abstract ulong DataTimeUtc { get; internal set; }

        /// <summary>
        /// 数据生成时间（UTC时间）
        /// </summary>
        public abstract DateTime DataTime { get; }

        /// <summary>
        /// 目标数量
        /// </summary>
        public abstract ushort Count { get; internal set; }

        /// <summary>
        /// 获取数据包类型
        /// </summary>
        public abstract PacketType Type { get; }
    }

    /// <summary>
    /// 包含Cluster/Object数据包的结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Packet<T> : PacketBase where T : Data
    {
        private ulong _dataTimeUtc;
        private DateTime _dataTime;

        /// <summary>
        /// 数据生成时间 (UTC毫秒时间戳)
        /// </summary>
        public override ulong DataTimeUtc
        {
            get { return _dataTimeUtc; }
            internal set
            {
                _dataTimeUtc = value;
                _dataTime = DateTimeHelper.GetUtcTimeByTimeStampMillisec(_dataTimeUtc);
            }
        }

        /// <summary>
        /// 数据生成时间（UTC时间）
        /// </summary>
        public override DateTime DataTime { get { return _dataTime; } }

        /// <summary>
        /// 目标数量
        /// </summary>
        public override ushort Count { get; internal set; }

        /// <summary>
        /// 数据包类型
        /// </summary>
        public override PacketType Type{ get; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> Units { get; internal set; } = new List<T>();

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Packet {{" +
                $"DataTimeUtc: {DataTimeUtc}, " +
                $"DataTime: {DataTime:yyyy-MM-dd HH:mm:ss.fff}, " +
                $"Count: {Count}, " +
                $"Units: {Units.Count}" +
                $"}}";
        }
    }
}
