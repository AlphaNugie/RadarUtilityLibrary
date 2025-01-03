﻿using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Core
{
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
    /// 集群的动态属性
    /// </summary>
    public enum DynProp
    {
        /// <summary>
        /// 移动中
        /// </summary>
        [EnumDescription("移动")]
        Moving = 0x0,

        /// <summary>
        /// 静止
        /// </summary>
        [EnumDescription("静止")]
        Stationary = 0x1,

        /// <summary>
        /// 迎面而来
        /// </summary>
        [EnumDescription("来向")]
        Oncoming = 0x2,

        /// <summary>
        /// 疑似的静止点
        /// </summary>
        [EnumDescription("可能静止")]
        StationaryCandidate = 0x3,

        /// <summary>
        /// 未知
        /// </summary>
        [EnumDescription("未知")]
        Unknown = 0x4,

        /// <summary>
        /// 横穿静止
        /// </summary>
        [EnumDescription("横穿静止")]
        CrossingStationary = 0x5,

        /// <summary>
        /// 横穿移动
        /// </summary>
        [EnumDescription("横穿移动")]
        CrossingMoving = 0x6,

        /// <summary>
        /// 停止（移动转静止？）
        /// </summary>
        [EnumDescription("停止")]
        Stopped = 0x7
    }

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

        /// <summary>
        /// 皮带料流（臂架或尾车）
        /// </summary>
        [EnumDescription("皮带料流")]
        Belt = 4,

        /// <summary>
        /// 尾车（仅堆料机/装船机）
        /// </summary>
        [EnumDescription("尾车")]
        Tail = 9,

        #region 堆/取料机
        /// <summary>
        /// 斗轮（落料口）
        /// </summary>
        [EnumDescription("斗轮（落料口）")]
        Wheel = 2,

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
        Down = 6,

        /// <summary>
        /// 混合方向，当有运动但无法判明哪方在靠近时使用
        /// </summary>
        Mixed = 7
    }
}
