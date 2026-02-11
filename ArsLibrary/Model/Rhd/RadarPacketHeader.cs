using ArsLibrary.Core;
using CommonLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model.Rhd
{
    /// <summary>
    /// 表示完整的雷达数据包头部信息
    /// </summary>
    public class RadarPacketHeader
    {
        /// <summary>
        /// 包头标识 (0xCA 0xCB 0xCC 0xCD)
        /// </summary>
        public byte[] StartBytes { get; internal set; } = new byte[4];

        /// <summary>
        /// 数据长度 (协议版本到包尾的总字节数)
        /// </summary>
        public uint DataLength { get; internal set; }

        private byte[] _protoVerBytes;
        /// <summary>
        /// 协议版本 (字节数组，长度为2，如0x0213表示v2.1.3，字节0为0x02，字节1为0x13)
        /// </summary>
        public byte[] ProtocolVersionInBytes
        {
            get { return _protoVerBytes; }
            internal set
            {
                if (value == null || value.Length != 2)
                    return;
                _protoVerBytes = value;
                int d1 = value[0];
                int d2 = Math.DivRem(value[1], 16, out int d3);
                ProtocolVersion = string.Format("v{0}.{1}.{2}", d1, d2, d3);
            }
        }

        /// <summary>
        /// 协议版本 (如v2.0.0)
        /// </summary>
        public string ProtocolVersion { get; private set; }

        ///// <summary>
        ///// 协议版本 (如0x0200表示v2.0.0)
        ///// </summary>
        //public ushort ProtocolVersion { get; set; }

        private byte _dataTypeNum;
        /// <summary>
        /// 数据类型 (0x01:Object, 0x02:Cluster)
        /// </summary>
        public byte DataTypeNum
        {
            get { return _dataTypeNum; }
            internal set
            {
                _dataTypeNum = value;
                var sensorMode = SensorMode.Unknown;
                switch (value)
                {
                    case 0x01:
                        sensorMode = SensorMode.Object;
                        break;
                    case 0x02:
                        sensorMode = SensorMode.Cluster;
                        break;
                }
                DataType = sensorMode;
            }
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public SensorMode DataType { get; private set; } = SensorMode.Unknown;

        /// <summary>
        /// 设备唯一标识 (UTF-8编码字符串)
        /// </summary>
        public string DeviceId { get; internal set; } = string.Empty;

        /// <summary>
        /// 预留字节 (8字节填充0)
        /// </summary>
        public byte[] Reserved { get; internal set; } = new byte[8];

        private ulong _frameTimeUtc;
        /// <summary>
        /// 帧时间，代表此帧数据发送的时间 (UTC毫秒时间戳)
        /// </summary>
        public ulong FrameTimeUtc
        {
            get { return _frameTimeUtc; }
            internal set
            {
                _frameTimeUtc = value;
                FrameTime = DateTimeHelper.GetUtcTimeByTimeStampMillisec(_frameTimeUtc);
            }
        }

        /// <summary>
        /// 帧时间，代表此帧数据发送的时间（UTC时间）
        /// </summary>
        public DateTime FrameTime { get; private set; }

        /// <summary>
        /// 帧序列号 (连续递增)
        /// </summary>
        public uint FrameSequence { get; internal set; }

        /// <summary>
        /// 数据包CRC (CRC-16-CCITT)
        /// </summary>
        public ushort StoredCrc { get; internal set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"RadarPacketHeader {{ " +
                   //$"StartBytes: {BitConverter.ToString(StartBytes)}, " +
                   $"DataLength: {DataLength}, " +
                   //$"ProtocolVersionInBytes: {BitConverter.ToString(ProtocolVersionInBytes)}, " +
                   $"ProtocolVersion: {ProtocolVersion}, " +
                   //$"DataTypeNum: {DataTypeNum}, " +
                   $"DataType: {DataType}, " +
                   $"DeviceId: {DeviceId}, " +
                   //$"Reserved: {BitConverter.ToString(Reserved)}, " +
                   $"FrameTimeUtc: {FrameTimeUtc}, " +
                   $"FrameTime: {FrameTime:yyyy-MM-dd HH:mm:ss.fff}, " +
                   $"FrameSequence: {FrameSequence}, " +
                   $"StoredCrc: {StoredCrc} " +
                   $"}}";
        }
    }
}
