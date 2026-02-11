using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model.Rhd
{
    /// <summary>
    /// 包含Cluster数据包的结构
    /// </summary>
    public sealed class ClusterPacket : Packet<ClusterData>
    {
        /// <inheritdoc/>
        public override PacketType Type => PacketType.Cluster;

        ///// <summary>
        ///// 数据生成时间 (UTC毫秒时间戳)
        ///// </summary>
        //public ulong DataTimeUtc { get; set; }

        ///// <summary>
        ///// 目标数量
        ///// </summary>
        //public ushort ObjectCount { get; set; }

        ///// <summary>
        ///// Cluster数据列表
        ///// </summary>
        //public List<ClusterData> Clusters { get; set; } = new List<ClusterData>();
    }
}
