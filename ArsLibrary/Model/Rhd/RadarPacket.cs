using ArsLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArsLibrary.Model.Rhd
{
    /// <summary>
    /// 表示完整的雷达数据包
    /// </summary>
    public class RadarPacket
    {
        //#region 静态方法
        ///// <summary>
        ///// 从雷达数据包中获取指定类型的数据单元列表
        ///// </summary>
        ///// <typeparam name="T">数据类型（ClusterData 或 ObjectData）</typeparam>
        ///// <param name="packet">雷达数据包</param>
        ///// <returns>数据单元列表，获取失败返回null</returns>
        //public static List<T> GetUnits<T>(RadarPacket packet) where T : Data
        //{
        //    return (packet.DataContent as Packet<T>)?.Units;
        //}
        //#endregion

        /// <summary>
        /// 数据包头信息
        /// </summary>
        public RadarPacketHeader Header { get; set; } = new RadarPacketHeader();

        /// <summary>
        /// 传感器模式
        /// </summary>
        public SensorMode SensorMode { get { return Header == null ? SensorMode.Unknown : Header.DataType; } }

        /// <summary>
        /// 目标数量
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                if (DataContent is ClusterPacket cluster)
                    count = cluster.Count;
                else if (DataContent is ObjectPacket obj)
                    count = obj.Count;
                return count;
            }
        }

        /// <summary>
        /// 数据包内容 (ClusterPacket或ObjectPacket)
        /// </summary>
#if NET45_OR_GREATER
        public PacketBase DataContent { get; set; }
#elif NET9_0_OR_GREATER
        public PacketBase? DataContent { get; set; }
#endif

        /// <inheritdoc/>
        public override string ToString()
        {
            string content = DataContent?.ToString() ?? string.Empty;
            return Header.ToString() + "\n" + content;
        }

        /// <summary>
        /// 从雷达数据包中获取指定类型的数据单元列表
        /// </summary>
        /// <typeparam name="T">数据类型（ClusterData 或 ObjectData）</typeparam>
        /// <returns>数据单元列表，获取失败返回null</returns>
        public List<T> GetUnits<T>() where T : Data
        {
            return (DataContent as Packet<T>)?.Units;
        }
    }
}
