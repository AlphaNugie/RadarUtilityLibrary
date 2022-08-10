using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Core
{
    /// <summary>
    /// 连接模式
    /// </summary>
    public enum ConnectionMode
    {
        /// <summary>
        /// TCP客户端
        /// </summary>
        TCP_CLIENT = 1,

        /// <summary>
        /// UDP
        /// </summary>
        UDP = 2,

        /// <summary>
        /// TCP监听
        /// </summary>
        TCP_SERVER = 3
    }

    /// <summary>
    /// 传感器模式
    /// </summary>
    public enum SensorMode
    {
        /// <summary>
        /// 集群模式
        /// </summary>
        Cluster = 0,

        /// <summary>
        /// 目标模式
        /// </summary>
        Object = 1
    }

    /// <summary>
    /// 传感器消息ID（对应ID为0的传感器），各传感器(ID0-7)对应消息ID计算方式：MsgId = MsgId_0 + SensorId * 0x10
    /// 例如ID为0x210的消息对应传感器ID1
    /// </summary>
    public enum SensorMessageId_0
    {
        /// <summary>
        /// 传感器配置
        /// </summary>
        [EnumDescription("传感器配置")]
        RadarCfg_In = 0x200,

        /// <summary>
        /// 传感器状态
        /// </summary>
        [EnumDescription("传感器状态")]
        RadarState_Out = 0x201,

        /// <summary>
        /// 过滤配置
        /// </summary>
        [EnumDescription("过滤配置")]
        FilterCfg_In = 0x202,

        /// <summary>
        /// 过滤状态包头
        /// </summary>
        [EnumDescription("过滤状态包头")]
        FilterState_Header_Out = 0x203,

        /// <summary>
        /// 过滤配置状态
        /// </summary>
        [EnumDescription("过滤配置状态")]
        FilterState_Cfg_Out = 0x204,

        /// <summary>
        /// 碰撞检测配置
        /// </summary>
        [EnumDescription("碰撞检测配置")]
        CollDetCfg_In = 0x400,

        /// <summary>
        /// 碰撞探测区域配置
        /// </summary>
        [EnumDescription("碰撞探测区域配置")]
        CollDetRegionCfg_In = 0x401,

        /// <summary>
        /// 碰撞检测状态
        /// </summary>
        [EnumDescription("碰撞检测状态")]
        CollDetState_Out = 0x408,

        /// <summary>
        /// 碰撞检测区域状态
        /// </summary>
        [EnumDescription("碰撞检测区域状态")]
        CollDetRegionState_Out = 0x402,

        /// <summary>
        /// 车辆速度
        /// </summary>
        [EnumDescription("车辆速度")]
        SpeedInformation_In = 0x300,

        /// <summary>
        /// 车辆偏航角速度
        /// </summary>
        [EnumDescription("车辆偏航角速度")]
        YawRateInformation_In = 0x301,

        /// <summary>
        /// 集群状态 (列表头)
        /// </summary>
        [EnumDescription("集群状态(列表头)")]
        Cluster_0_Status_Out = 0x600,

        /// <summary>
        /// 集群一般信息
        /// </summary>
        [EnumDescription("集群一般信息")]
        Cluster_1_General_Out = 0x701,

        /// <summary>
        /// 集群重要信息
        /// </summary>
        [EnumDescription("集群重要信息")]
        Cluster_2_Quality_Out = 0x702,

        /// <summary>
        /// 目标状态(列表头)
        /// </summary>
        [EnumDescription("目标状态(列表头)")]
        Obj_0_Status_Out = 0x60A,

        /// <summary>
        /// 目标一般信息
        /// </summary>
        [EnumDescription("目标一般信息")]
        Obj_1_General_Out = 0x60B,

        /// <summary>
        /// 目标重要信息
        /// </summary>
        [EnumDescription("目标重要信息")]
        Obj_2_Quality_Out = 0x60C,

        /// <summary>
        /// 目标拓展信息
        /// </summary>
        [EnumDescription("目标拓展信息")]
        Obj_3_Extended_Out = 0x60D,

        /// <summary>
        /// 目标碰撞检测预警
        /// </summary>
        [EnumDescription("目标碰撞检测预警")]
        Obj_4_Warning_Out = 0x60E,

        /// <summary>
        /// 软件版本
        /// </summary>
        [EnumDescription("软件版本")]
        VersionID_Out = 0x700,

        /// <summary>
        /// 继电器控制信息
        /// </summary>
        [EnumDescription("继电器控制信息")]
        CollDetRelayCtrl_Out = 0x8
    }

    /// <summary>
    /// 方向
    /// </summary>
    public enum Directions
    {
        /// <summary>
        /// 无归属
        /// </summary>
        None = 0,

        /// <summary>
        /// 前方（原方向：海）
        /// </summary>
        Front = 1,

        /// <summary>
        /// 左方（原方向：北）
        /// </summary>
        Left = 2,

        /// <summary>
        /// 后方（原方向：陆）
        /// </summary>
        Back = 3,

        /// <summary>
        /// 右方（原方向：南）
        /// </summary>
        Right = 4,

        /// <summary>
        /// 上方
        /// </summary>
        Up = 5,

        /// <summary>
        /// 下方
        /// </summary>
        Down = 6
    }

    /// <summary>
    /// 雷达组类型
    /// </summary>
    public enum RadarGroupType
    {
        /// <summary>
        /// 无归属
        /// </summary>
        [EnumDescription("无归属")]
        None = 0,

        /// <summary>
        /// 臂架
        /// </summary>
        [EnumDescription("臂架")]
        Arm = 1,

        /// <summary>
        /// 门腿
        /// </summary>
        [EnumDescription("门腿")]
        Feet = 3,

        #region 堆/取料机
        /// <summary>
        /// 斗轮（落料口）
        /// </summary>
        [EnumDescription("斗轮（落料口）")]
        Wheel = 2,

        /// <summary>
        /// 堆取料机悬皮
        /// </summary>
        [EnumDescription("堆取料机悬皮")]
        Belt = 4,

        /// <summary>
        /// 堆取料机配重
        /// </summary>
        [EnumDescription("堆取料机配重")]
        Counterweight = 5,
        #endregion

        #region 装/卸船机
        /// <summary>
        /// 溜筒（料爪）
        /// </summary>
        [EnumDescription("溜筒（料爪）")]
        Bucket = 6,
        //Bucket = 2,

        /// <summary>
        /// 岸基（门腿向海侧）
        /// </summary>
        [EnumDescription("岸基")]
        Shore = 7,
        //Shore = 4,

        /// <summary>
        /// 舱盖
        /// </summary>
        [EnumDescription("舱盖")]
        Hatch = 8,
        //Hatch = 5,
        #endregion
    }

    /// <summary>
    /// 集群的动态属性
    /// </summary>
    public enum DynProp
    {
        /// <summary>
        /// 移动中
        /// </summary>
        [EnumDescription("移动中")]
        Moving = 0x0,

        /// <summary>
        /// 静止
        /// </summary>
        [EnumDescription("静止")]
        Stationary = 0x1,

        /// <summary>
        /// 迎面而来
        /// </summary>
        [EnumDescription("迎面而来")]
        Oncoming = 0x2,

        /// <summary>
        /// 备选的静止点（疑似静止？）
        /// </summary>
        [EnumDescription("备选静止")]
        StationaryCandidate = 0x3,

        /// <summary>
        /// 未知
        /// </summary>
        [EnumDescription("未知")]
        Unknown = 0x4,

        /// <summary>
        /// 
        /// </summary>
        [EnumDescription("Crossing Stationary")]
        CrossingStationary = 0x5,

        /// <summary>
        /// 
        /// </summary>
        [EnumDescription("Crossing Moving")]
        CrossingMoving = 0x6,

        /// <summary>
        /// 停止（移动转静止？）
        /// </summary>
        [EnumDescription("停止")]
        Stopped = 0x7
    }

    /// <summary>
    /// 纵向与横向高度、相对速度、相对加速度的标准差的范围
    /// </summary>
    public enum SignalValue
    {
        /// <summary>
        /// 小于0.005
        /// </summary>
        [EnumDescription("<0.005")]
        lt0005 = 0x0,

        /// <summary>
        /// 小于0.006
        /// </summary>
        [EnumDescription("<0.006")]
        lt0006 = 0x1,

        /// <summary>
        /// 小于0.008
        /// </summary>
        [EnumDescription("<0.008")]
        lt0008 = 0x2,

        /// <summary>
        /// 小于0.011
        /// </summary>
        [EnumDescription("<0.011")]
        lt0011 = 0x3,

        /// <summary>
        /// 小于0.014
        /// </summary>
        [EnumDescription("<0.014")]
        lt0014 = 0x4,

        /// <summary>
        /// 小于0.018
        /// </summary>
        [EnumDescription("<0.018")]
        lt0018 = 0x5,

        /// <summary>
        /// 小于0.023
        /// </summary>
        [EnumDescription("<0.023")]
        lt0023 = 0x6,

        /// <summary>
        /// 小于0.029
        /// </summary>
        [EnumDescription("<0.029")]
        lt0029 = 0x7,

        /// <summary>
        /// 小于0.038
        /// </summary>
        [EnumDescription("<0.038")]
        lt0038 = 0x8,

        /// <summary>
        /// 小于0.049
        /// </summary>
        [EnumDescription("<0.049")]
        lt0049 = 0x9,

        /// <summary>
        /// 小于0.063
        /// </summary>
        [EnumDescription("<0.063")]
        lt0063 = 0xA,

        /// <summary>
        /// 小于0.081
        /// </summary>
        [EnumDescription("<0.081")]
        lt0081 = 0xB,

        /// <summary>
        /// 小于0.105
        /// </summary>
        [EnumDescription("<0.105")]
        lt0105 = 0xC,

        /// <summary>
        /// 小于0.135
        /// </summary>
        [EnumDescription("<0.135")]
        lt0135 = 0xD,

        /// <summary>
        /// 小于0.174
        /// </summary>
        [EnumDescription("<0.174")]
        lt0174 = 0xE,

        /// <summary>
        /// 小于0.224
        /// </summary>
        [EnumDescription("<0.224")]
        lt0224 = 0xF,

        /// <summary>
        /// 小于0.288
        /// </summary>
        [EnumDescription("<0.288")]
        lt0288 = 0x10,

        /// <summary>
        /// 小于0.371
        /// </summary>
        [EnumDescription("<0.371")]
        lt0371 = 0x11,

        /// <summary>
        /// 小于0.478
        /// </summary>
        [EnumDescription("<0.478")]
        lt0478 = 0x12,

        /// <summary>
        /// 小于0.616
        /// </summary>
        [EnumDescription("<0.616")]
        lt0616 = 0x13,

        /// <summary>
        /// 小于0.794
        /// </summary>
        [EnumDescription("<0.794")]
        lt0794 = 0x14,

        /// <summary>
        /// 小于1.023
        /// </summary>
        [EnumDescription("<1.023")]
        lt1023 = 0x15,

        /// <summary>
        /// 小于1.317
        /// </summary>
        [EnumDescription("<1.317")]
        lt1317 = 0x16,

        /// <summary>
        /// 小于1.697
        /// </summary>
        [EnumDescription("<1.697")]
        lt1697 = 0x17,

        /// <summary>
        /// 小于2.187
        /// </summary>
        [EnumDescription("<2.187")]
        lt2187 = 0x18,

        /// <summary>
        /// 小于2.817
        /// </summary>
        [EnumDescription("<2.817")]
        lt2817 = 0x19,

        /// <summary>
        /// 小于3.630
        /// </summary>
        [EnumDescription("<3.630")]
        lt3630 = 0x1A,

        /// <summary>
        /// 小于4.676
        /// </summary>
        [EnumDescription("<4.676")]
        lt4676 = 0x1B,

        /// <summary>
        /// 小于6.025
        /// </summary>
        [EnumDescription("<6.025")]
        lt6025 = 0x1C,

        /// <summary>
        /// 小于7.762
        /// </summary>
        [EnumDescription("<7.762")]
        lt7762 = 0x1D,

        /// <summary>
        /// 小于10.000
        /// </summary>
        [EnumDescription("<10.000")]
        lt10000 = 0x1E,

        /// <summary>
        /// 无效数值
        /// </summary>
        [EnumDescription("无效数值")]
        Invalid = 0x1F
    }
}
