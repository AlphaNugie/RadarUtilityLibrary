using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model.Rhd
{
    /// <summary>
    /// 包含Object数据包的结构
    /// </summary>
    public sealed class ObjectPacket : Packet<ObjectData>
    {
        /// <inheritdoc/>
        public override PacketType Type => PacketType.Object;

        ///// <summary>
        ///// 数据时间，代表数据生成时间 (UTC毫秒时间戳)
        ///// </summary>
        //public ulong DataTimeUtc { get; set; }

        ///// <summary>
        ///// 目标数量
        ///// </summary>
        //public ushort ObjectCount { get; set; }

        ///// <summary>
        ///// Object数据列表
        ///// </summary>
        //public List<ObjectData> Objects { get; set; } = new List<ObjectData>();
    }
}
