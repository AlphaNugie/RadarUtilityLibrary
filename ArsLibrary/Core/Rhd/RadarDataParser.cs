using ArsLibrary.Model.Rhd;
using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Core.Rhd
{
    /* TODO：解析帧数据部分使用了BinaryReader，它会按照当前机器的字节序来读取。而协议要求小端，所以如果当前机器是大端，则读取错误。因此，我们需要确保按小端读取
     * 为了确保跨平台，我们修改解析部分：不使用BinaryReader的ReadUInt16等方法，而是读取字节数组，然后用BitConverter转换，并强制按小端解释。
     * 例如：
     * 
     * ushort value = BitConverter.ToUInt16(bytes, 0); // 但这样依赖于当前机器的字节序
     * 
     * 所以，我们可以这样：
     * 
     * if (BitConverter.IsLittleEndian)
     *     value = BitConverter.ToUInt16(bytes, 0);
     * else
     *     value = BitConverter.ToUInt16(bytes.Reverse().ToArray(), 0);
     *     
     * 但是这样效率低。我们可以写一个辅助方法：
     * 
     * public static class LittleEndian
     * {
     *     public static ushort ToUInt16(byte[] data, int offset)
     *     {
     *         if (BitConverter.IsLittleEndian)
     *             return BitConverter.ToUInt16(data, offset);
     *         else
     *             return (ushort)(data[offset] | (data[offset+1] << 8));
     *     }
     *     // 同理实现其他类型
     * }
     * 
     * 但是，我们为了简单，假设运行环境是小端（x86和x64都是小端），所以暂时不考虑大端机器。
     */

    /// <summary>
    /// 雷达解析配置参数，用于支持不同型号雷达的数据解析
    /// </summary>
    public class RadarParseConfig
    {
        /// <summary>坐标位置比例因子（XYZ），默认为20.0</summary>
        public double PositionScaleFactor { get; set; } = 20.0;

        /// <summary>速度比例因子（VxVyVz），默认为50.0</summary>
        public double VelocityScaleFactor { get; set; } = 50.0;

        /// <summary>RCS比例因子，默认为10.0</summary>
        public double RcsScaleFactor { get; set; } = 10.0;

        /// <summary>长度/宽度/高度比例因子（Object模式），默认为2.0</summary>
        public double SizeScaleFactor { get; set; } = 2.0;

        /// <summary>
        /// HURYS基数据协议版本号，默认为v2.1.3（RHD 4d雷达）
        /// </summary>
        public ProtocolVersion ProtocolVersion { get; set; } = ProtocolVersion.v213;

        /// <summary>
        /// 使用给定的HURYS雷达通信协议版本号初始化，默认为v2.1.3（RHD 4d雷达）、此时使用默认配置；假如为其它版本，则会改特定参数并在解析时做出特定处理
        /// </summary>
        /// <param name="version">HURYS雷达通信协议版本号，默认为v2.1.3（RHD 4d雷达）</param>
        public RadarParseConfig(ProtocolVersion version = ProtocolVersion.v213)
        {
            ProtocolVersion = version;
            if (version == ProtocolVersion.v212)
            {
                PositionScaleFactor = 50.0;
                SizeScaleFactor = 10.0;
            }
        }

        /// <summary>预定义配置：标准型号（RHD 4D雷达，XYZ除以20）</summary>
        public static RadarParseConfig Rhd => new RadarParseConfig();

        /// <summary>
        /// 预定义配置：新型号
        /// <para/>XYZ除以50，（object）长宽高除以10
        /// </summary>
        public static RadarParseConfig Fd => new RadarParseConfig(ProtocolVersion.v212);
        //public static RadarParseConfig Fd => new RadarParseConfig
        //{
        //    PositionScaleFactor = 50.0,
        //    SizeScaleFactor = 10.0,
        //    ProtocolVersion = ProtocolVersion.v212
        //};
    }

    /// <summary>
    /// 雷达数据解析器，实现协议解析逻辑
    /// </summary>
    public static class RadarDataParser
    {
        // 协议常量定义
        private static readonly byte[] PacketStartMarker = { 0xCA, 0xCB, 0xCC, 0xCD };
        private static readonly byte[] PacketEndMarker = { 0xEA, 0xEB, 0xEC, 0xED };
        private const int HeaderSize = 4 + 4 + 2 + 1 + 32 + 8 + 8 + 4; // 包头到帧序列号的固定长度
        private const int DeviceIdLength = 32;
        private const int ReservedLength = 8;

        // CRC16校验表 (MSB格式)，长度为256（0~0xFF）
        private static readonly ushort[] Crc16Table =
        {
            0x0000,0x1021,0x2042,0x3063,0x4084,0x50a5,0x60c6,0x70e7,0x8108,0x9129,0xa14a,0xb16b,0xc18c,0xd1ad,0xe1ce,0xf1ef,
            0x1231,0x0210,0x3273,0x2252,0x52b5,0x4294,0x72f7,0x62d6,0x9339,0x8318,0xb37b,0xa35a,0xd3bd,0xc39c,0xf3ff,0xe3de,
            0x2462,0x3443,0x0420,0x1401,0x64e6,0x74c7,0x44a4,0x5485,0xa56a,0xb54b,0x8528,0x9509,0xe5ee,0xf5cf,0xc5ac,0xd58d,
            0x3653,0x2672,0x1611,0x0630,0x76d7,0x66f6,0x5695,0x46b4,0xb75b,0xa77a,0x9719,0x8738,0xf7df,0xe7fe,0xd79d,0xc7bc,
            0x48c4,0x58e5,0x6886,0x78a7,0x0840,0x1861,0x2802,0x3823,0xc9cc,0xd9ed,0xe98e,0xf9af,0x8948,0x9969,0xa90a,0xb92b,
            0x5af5,0x4ad4,0x7ab7,0x6a96,0x1a71,0x0a50,0x3a33,0x2a12,0xdbfd,0xcbdc,0xfbbf,0xeb9e,0x9b79,0x8b58,0xbb3b,0xab1a,
            0x6ca6,0x7c87,0x4ce4,0x5cc5,0x2c22,0x3c03,0x0c60,0x1c41,0xedae,0xfd8f,0xcdec,0xddcd,0xad2a,0xbd0b,0x8d68,0x9d49,
            0x7e97,0x6eb6,0x5ed5,0x4ef4,0x3e13,0x2e32,0x1e51,0x0e70,0xff9f,0xefbe,0xdfdd,0xcffc,0xbf1b,0xaf3a,0x9f59,0x8f78,
            0x9188,0x81a9,0xb1ca,0xa1eb,0xd10c,0xc12d,0xf14e,0xe16f,0x1080,0x00a1,0x30c2,0x20e3,0x5004,0x4025,0x7046,0x6067,
            0x83b9,0x9398,0xa3fb,0xb3da,0xc33d,0xd31c,0xe37f,0xf35e,0x02b1,0x1290,0x22f3,0x32d2,0x4235,0x5214,0x6277,0x7256,
            0xb5ea,0xa5cb,0x95a8,0x8589,0xf56e,0xe54f,0xd52c,0xc50d,0x34e2,0x24c3,0x14a0,0x0481,0x7466,0x6447,0x5424,0x4405,
            0xa7db,0xb7fa,0x8799,0x97b8,0xe75f,0xf77e,0xc71d,0xd73c,0x26d3,0x36f2,0x0691,0x16b0,0x6657,0x7676,0x4615,0x5634,
            0xd94c,0xc96d,0xf90e,0xe92f,0x99c8,0x89e9,0xb98a,0xa9ab,0x5844,0x4865,0x7806,0x6827,0x18c0,0x08e1,0x3882,0x28a3,
            0xcb7d,0xdb5c,0xeb3f,0xfb1e,0x8bf9,0x9bd8,0xabbb,0xbb9a,0x4a75,0x5a54,0x6a37,0x7a16,0x0af1,0x1ad0,0x2ab3,0x3a92,
            0xfd2e,0xed0f,0xdd6c,0xcd4d,0xbdaa,0xad8b,0x9de8,0x8dc9,0x7c26,0x6c07,0x5c64,0x4c45,0x3ca2,0x2c83,0x1ce0,0x0cc1,
            0xef1f,0xff3e,0xcf5d,0xdf7c,0xaf9b,0xbfba,0x8fd9,0x9ff8,0x6e17,0x7e36,0x4e55,0x5e74,0x2e93,0x3eb2,0x0ed1,0x1ef0
        };

		/// <summary>
		/// 解析雷达数据包，从十六进制字符串中提取完整的协议包
		/// </summary>
		/// <param name="hex">16进制字符串，自动转换为原始字节数据</param>
		/// <param name="paramSet">空间旋转位移参数集，假如为null将不进行任何变换（点在单机坐标系下的坐标与在雷达自身坐标系下相同）</param>
		/// <param name="config">解析配置参数，默认为标准型号配置</param>
		/// <returns>解析后的RadarPacket对象</returns>
		/// <exception cref="InvalidDataException">数据格式无效时抛出</exception>
		public static RadarPacket Parse(string hex, CoordTransParamSet paramSet = null, RadarParseConfig config = null)
        {
            return Parse(hex, out _, paramSet, config);
        }

		/// <summary>
		/// 解析雷达数据包，从十六进制字符串中提取完整的协议包
		/// </summary>
		/// <param name="hex">16进制字符串，自动转换为原始字节数据</param>
		/// <param name="processHex">以16进制字符串形式提取出来的完整协议包（从包头到包尾，包含标记）</param>
		/// <param name="paramSet">空间旋转位移参数集，假如为null将不进行任何变换（点在单机坐标系下的坐标与在雷达自身坐标系下相同）</param>
		/// <param name="config">解析配置参数，默认为标准型号配置</param>
		/// <returns>解析后的RadarPacket对象</returns>
		/// <exception cref="InvalidDataException">数据格式无效时抛出</exception>
		public static RadarPacket Parse(string hex, out string processHex, CoordTransParamSet paramSet = null, RadarParseConfig config = null)
		{
            byte[] bytes = new byte[0];
            if (!string.IsNullOrWhiteSpace(hex))
                bytes = HexHelper.HexString2Bytes(hex);
            var packet = Parse(bytes, out byte[] processData, paramSet, config);
            processHex = processData == null ? string.Empty : HexHelper.ByteArray2HexString(processData);
            return packet;

			//if (string.IsNullOrWhiteSpace(hex))
			//             return new RadarPacket();

			//         // 将十六进制字符串转换为字节数组
			//         byte[] bytes = HexHelper.HexString2Bytes(hex);

			//         // 提取完整的协议包：从包头到包尾（包含包头和包尾）
			//         byte[] packetBytes = ExtractPacketBetweenMarkers(bytes);

			//         if (packetBytes == null || packetBytes.Length < HeaderSize)
			//             return new RadarPacket();

			//         return Parse(packetBytes);
		}

		/// <summary>
		/// 解析雷达数据包，自动查找完整的协议包
		/// </summary>
		/// <param name="data">原始字节数据</param>
		/// <param name="paramSet">空间旋转位移参数集，假如为null将不进行任何变换（点在单机坐标系下的坐标与在雷达自身坐标系下相同）</param>
		/// <param name="config">解析配置参数，默认为标准型号配置</param>
		/// <returns>解析后的RadarPacket对象</returns>
		/// <exception cref="InvalidDataException">数据格式无效时抛出</exception>
		public static RadarPacket Parse(byte[] data, CoordTransParamSet paramSet = null, RadarParseConfig config = null)
        {
            return Parse(data, out _, paramSet, config);
        }

		/// <summary>
		/// 解析雷达数据包，自动查找完整的协议包，并将完整协议包通过out标记的参数输出
		/// </summary>
		/// <param name="data">原始字节数据</param>
		/// <param name="processData">以byte数组形式提取出来的完整协议包（从包头到包尾，包含标记）</param>
		/// <param name="paramSet">空间旋转位移参数集，假如为null将不进行任何变换（点在单机坐标系下的坐标与在雷达自身坐标系下相同）</param>
		/// <param name="config">解析配置参数，默认为标准型号配置</param>
		/// <returns>解析后的RadarPacket对象</returns>
		/// <exception cref="InvalidDataException">数据格式无效时抛出</exception>
		public static RadarPacket Parse(byte[] data, out byte[] processData, CoordTransParamSet paramSet = null, RadarParseConfig config = null)
        {
            processData = null;
			if (data == null || data.Length < HeaderSize)
                return new RadarPacket();

            // 提取完整的协议包（从包头到包尾，包含标记）
            /*byte[] */processData = ExtractPacketBetweenMarkers(data);
            if (processData == null || processData.Length < HeaderSize)
            {
                throw new InvalidDataException("Packet start or end marker not found in data");
            }

#if NET45_OR_GREATER
            var stream = new MemoryStream(processData);
            var reader = new BinaryReader(stream);
#elif NET9_0_OR_GREATER
            using var stream = new MemoryStream(processData);
            using var reader = new BinaryReader(stream);
#endif

            // 1. 验证包头
            var startBytes = reader.ReadBytes(4);
            if (!startBytes.SequenceEqual(PacketStartMarker))
            {
                throw new InvalidDataException("Invalid packet start marker");
            }

            // 2. 读取数据长度 (小端字节序)
            var dataLength = reader.ReadUInt32();

            // 3. 读取整个数据块 (包含协议版本到包尾)
            var payloadBlock = reader.ReadBytes((int)dataLength);
            if (payloadBlock.Length < dataLength)
            {
                throw new InvalidDataException("Incomplete payload block");
            }

            // 4. CRC校验 (范围: 数据长度字段+协议版本到帧数据)
            var crcData = new byte[4 + dataLength - 6]; // 4字节数据长度 + (数据块长度-6)
            Buffer.BlockCopy(BitConverter.GetBytes(dataLength), 0, crcData, 0, 4); // 包含数据长度原始字节
            Buffer.BlockCopy(payloadBlock, 0, crcData, 4, (int)dataLength - 6); // 协议版本到帧数据

            var calculatedCrc = ComputeCrc16(crcData);
            var storedCrc = BitConverter.ToUInt16(payloadBlock, (int)dataLength - 6);

            if (calculatedCrc != storedCrc)
            {
                throw new InvalidDataException($"CRC mismatch: {calculatedCrc} vs {storedCrc}");
            }

            // 5. 验证包尾
            var endBytes = new byte[4];
            Buffer.BlockCopy(payloadBlock, (int)dataLength - 4, endBytes, 0, 4);

            if (!endBytes.SequenceEqual(PacketEndMarker))
            {
                throw new InvalidDataException("Invalid packet end marker");
            }

            // 6. 解析数据块 (不包括CRC和包尾)
#if NET45_OR_GREATER
            var payloadStream = new MemoryStream(payloadBlock, 0, (int)dataLength - 6);
            var payloadReader = new BinaryReader(payloadStream);
#elif NET9_0_OR_GREATER
            using var payloadStream = new MemoryStream(payloadBlock, 0, (int)dataLength - 6);
            using var payloadReader = new BinaryReader(payloadStream);
#endif

            var header = ParseHeader(payloadReader);
            header.StartBytes = startBytes;
            header.DataLength = dataLength;
            header.StoredCrc = storedCrc;
#if NET45_OR_GREATER
            //object dataContent = null;
            PacketBase dataContent = null;
            //switch (header.DataTypeNum)
            switch (header.DataType)
            {
                // Object类型
                //case 0x01:
                case SensorMode.Object:
                    dataContent = ParseObjectData(payloadReader, paramSet, config);
                    break;
                // Cluster类型
                //case 0x02:
                case SensorMode.Cluster:
                    dataContent = ParseClusterData(payloadReader, paramSet, config);
                    break;
                 default:
                    throw new InvalidDataException($"Unknown data type: {header.DataTypeNum}");
            }
#elif NET9_0_OR_GREATER
            object? dataContent = header.DataType switch
            {
                SensorMode.Object => ParseObjectData(payloadReader, paramSet, config),  // Object类型
                SensorMode.Cluster => ParseClusterData(payloadReader, paramSet, config),// Cluster类型
                _ => throw new InvalidDataException($"Unknown data type: {header.DataTypeNum}")
            };
#endif

#if NET45_OR_GREATER
            stream.Close();
            reader.Close();
            payloadStream.Close();
            payloadReader.Close();
#endif

            return new RadarPacket
            {
                Header = header,
                DataContent = dataContent
            };
        }

        /// <summary>
        /// 解析数据包头
        /// </summary>
        private static RadarPacketHeader ParseHeader(BinaryReader reader)
        {
            return new RadarPacketHeader
            {
                //ProtocolVersion = reader.ReadUInt16(),
                ProtocolVersionInBytes = reader.ReadBytes(2).Reverse().ToArray(),
                DataTypeNum = reader.ReadByte(),
                DeviceId = Encoding.UTF8.GetString(reader.ReadBytes(DeviceIdLength)).TrimEnd('\0'),
                Reserved = reader.ReadBytes(ReservedLength),
                FrameTimeUtc = reader.ReadUInt64(),
                FrameSequence = reader.ReadUInt32()
            };
        }

		/// <summary>
		/// 解析Cluster数据
		/// </summary>
		/// <param name="reader">二进制数据读取器</param>
		/// <param name="paramSet">空间旋转位移参数集，假如为null将不进行任何变换（点在单机坐标系下的坐标与在雷达自身坐标系下相同）</param>
		/// <param name="config">解析配置参数，默认为标准型号配置</param>
		private static ClusterPacket ParseClusterData(BinaryReader reader, CoordTransParamSet paramSet = null, RadarParseConfig config = null)
        {
#if NET45_OR_GREATER
            if (config == null)
                config = RadarParseConfig.Rhd;
#elif NET9_0_OR_GREATER
            config ??= RadarParseConfig.Rhd;
#endif

            var packet = new ClusterPacket
            {
                DataTimeUtc = reader.ReadUInt64(),
                Count = reader.ReadUInt16()
            };

            for (int i = 0; i < packet.Count; i++)
            {
                var data = new ClusterData
                {
                    Id = reader.ReadUInt16(),
                    X = reader.ReadUInt16() / config.PositionScaleFactor - 100.0,
                    Y = reader.ReadInt16() / config.PositionScaleFactor,
                    Z = reader.ReadInt16() / config.PositionScaleFactor,
                    Vx = reader.ReadInt16() / config.VelocityScaleFactor,
                    Vy = reader.ReadInt16() / config.VelocityScaleFactor,
                    Vz = reader.ReadInt16() / config.VelocityScaleFactor,
                    Rcs = reader.ReadInt16() / config.RcsScaleFactor
                };
                data.UpdateCoordinates(data.X, data.Y, data.Z);
                if (paramSet != null)
                    CoordinateTransformer.TransformPoint(ref data, paramSet);
				packet.Units.Add(data);
            }

            return packet;
        }

		/// <summary>
		/// 解析Object数据
		/// </summary>
		/// <param name="reader">二进制数据读取器</param>
		/// <param name="paramSet">空间旋转位移参数集，假如为null将不进行任何变换（点在单机坐标系下的坐标与在雷达自身坐标系下相同）</param>
		/// <param name="config">解析配置参数，默认为标准型号配置</param>
		private static ObjectPacket ParseObjectData(BinaryReader reader, CoordTransParamSet paramSet = null, RadarParseConfig config = null)
        {
#if NET45_OR_GREATER
            if (config == null)
                config = RadarParseConfig.Rhd;
#elif NET9_0_OR_GREATER
            config ??= RadarParseConfig.Rhd;
#endif

            var packet = new ObjectPacket
            {
                DataTimeUtc = reader.ReadUInt64(),
                Count = reader.ReadUInt16()
            };

            for (int i = 0; i < packet.Count; i++)
            {
                var data = new ObjectData
                {
                    Id = reader.ReadUInt16(),
                    //ReservedByte = reader.ReadByte(),
                    Type = (ObjectType)reader.ReadByte(),
                    Length = reader.ReadUInt16() / config.SizeScaleFactor,
                    Width = reader.ReadByte() / config.SizeScaleFactor,
                    Height = reader.ReadByte() / config.SizeScaleFactor,
                    X = reader.ReadUInt16() / config.PositionScaleFactor - 100.0,
                    Y = reader.ReadInt16() / config.PositionScaleFactor,
                    Z = reader.ReadInt16() / config.PositionScaleFactor,
                    Vx = reader.ReadInt16() / config.VelocityScaleFactor,
                    Vy = reader.ReadInt16() / config.VelocityScaleFactor,
                    Vz = reader.ReadInt16() / config.VelocityScaleFactor,
                    Speed = reader.ReadInt16() / config.VelocityScaleFactor,
                };
				data.UpdateCoordinates(data.X, data.Y, data.Z);
				if (paramSet != null)
					CoordinateTransformer.TransformPoint(ref data, paramSet);

                switch (config.ProtocolVersion)
                {
                    case ProtocolVersion.v212:
                        // 加速度值，仅在协议v2.1.2中存在字段
                        data.Ax = reader.ReadInt16() / 100.0;
                        data.Ay = reader.ReadInt16() / 100.0;
                        data.Az = reader.ReadInt16() / 100.0;
                        data.Acceleration = reader.ReadInt16() / 100.0;
                        break;
                    case ProtocolVersion.v213:
                    default:
                        // 跳过保留字段 (8字节)
                        reader.ReadBytes(8);
                        break;
                }

                data.CourseAngle = reader.ReadUInt16() / 10.0;
                data.Longitude = reader.ReadInt64() / 100000000.0;
                data.Latitude = reader.ReadInt64() / 100000000.0;
                data.Altitude = reader.ReadUInt16() / 10.0 - 500;
                data.Rcs = reader.ReadInt16() / config.RcsScaleFactor;

                switch (config.ProtocolVersion)
                {
                    case ProtocolVersion.v212:
                        // 置信度/检测状态，仅在协议v2.1.2中存在字段
                        data.Confidence = (ProbOfExist)reader.ReadByte();
                        data.MeasState = (MeasState)reader.ReadByte();
                        break;
                    case ProtocolVersion.v213:
                    default:
                        // 跳过保留字段 (1字节)
                        reader.ReadByte();
                        break;
                }

                packet.Units.Add(data);
            }

            return packet;
        }

        /// <summary>
        /// 计算CRC16校验值 (MSB格式)
        /// </summary>
        /// <param name="data">待校验数据</param>
        /// <returns>16位CRC校验码</returns>
        private static ushort ComputeCrc16(byte[] data)
        {
            ushort crc = 0;
            foreach (byte b in data)
            {
                crc = (ushort)(Crc16Table[((crc >> 8) ^ b) & 0xFF] ^ (crc << 8));
            }
            return crc;
        }

  //      /// <summary>
  //      /// 在数据数组中查找包头标记 (0xCA, 0xCB, 0xCC, 0xCD)
  //      /// </summary>
  //      /// <param name="data">待搜索的数据数组</param>
  //      /// <returns>包头标记的起始索引，未找到返回-1</returns>
  //      private static int FindStartMarker(byte[] data)
  //      {
  //          if (data == null || data.Length < PacketStartMarker.Length)
  //              return -1;

  //          // 遍历数据数组，查找包头标记
  //          for (int i = 0; i <= data.Length - PacketStartMarker.Length; i++)
  //          {
  //              bool match = true;
  //              for (int j = 0; j < PacketStartMarker.Length; j++)
  //              {
  //                  if (data[i + j] != PacketStartMarker[j])
  //                  {
  //                      match = false;
  //                      break;
  //                  }
  //              }
  //              if (match)
  //                  return i;
  //          }
  //          return -1;
		//}

		/// <summary>
		/// 从字节数组中提取从包头标记到包尾标记之间的数据
		/// </summary>
		/// <param name="data">原始字节数据</param>
		/// <returns>提取的协议包数据（包含包头和包尾），未找到返回null</returns>
		private static byte[] ExtractPacketBetweenMarkers(byte[] data)
		{
			if (data == null || data.Length < PacketStartMarker.Length + PacketEndMarker.Length)
				return null;

			// 1. 查找包头标记位置
			int startMarkerIndex = FindMarkerIndex(data, PacketStartMarker);
			if (startMarkerIndex == -1)
				return null;

			// 2. 从包头之后查找包尾标记位置
			int endMarkerIndex = FindMarkerIndex(data, PacketEndMarker, startMarkerIndex + PacketStartMarker.Length);
			if (endMarkerIndex == -1)
				return null;

			// 3. 提取从包头到包尾的数据（包含包头和包尾）
			int packetLength = endMarkerIndex + PacketEndMarker.Length - startMarkerIndex;
			byte[] packetBytes = new byte[packetLength];
			Array.Copy(data, startMarkerIndex, packetBytes, 0, packetLength);

			return packetBytes;
		}

		/// <summary>
		/// 在字节数组中查找标记序列的位置
		/// </summary>
		/// <param name="data">待搜索的数据数组</param>
		/// <param name="marker">要查找的标记序列</param>
		/// <param name="startIndex">开始搜索的位置，默认为0</param>
		/// <returns>标记的起始索引，未找到返回-1</returns>
		private static int FindMarkerIndex(byte[] data, byte[] marker, int startIndex = 0)
		{
			if (data == null || marker == null || data.Length < marker.Length)
				return -1;

			for (int i = startIndex; i <= data.Length - marker.Length; i++)
			{
				bool match = true;
				for (int j = 0; j < marker.Length; j++)
				{
					if (data[i + j] != marker[j])
					{
						match = false;
						break;
					}
				}
				if (match)
					return i;
			}
			return -1;
		}
	}
}
