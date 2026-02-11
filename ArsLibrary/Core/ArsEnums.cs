using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Core
{
    #region Object模式

    #region 质量（置信）信息
    /// <summary>
    /// 测量状态，指示目标是否有效
    /// </summary>
    public enum MeasState
    {
        /// <summary>
        /// 无效值（指给出的值本身没有意义）
        /// </summary>
        [EnumDescription("无效值")]
        InvalidValue = -1,

        /// <summary>
        /// 被删除，ID消失前的最后一轮数据传输中出现
        /// </summary>
        [EnumDescription("被删除")]
        Deleted = 0,

        /// <summary>
        /// 新出现，ID创建后的第一轮数据传输中出现
        /// </summary>
        [EnumDescription("新出现")]
        New = 1,

        /// <summary>
        /// 已测量，目标的出现被实际测量证实
        /// </summary>
        [EnumDescription("已测量")]
        Measured = 2,

        /// <summary>
        /// 预测的，目标的出现无法被实际测量证实
        /// </summary>
        [EnumDescription("预测的")]
        Predicted = 3,

        /// <summary>
        /// 为合并删除，为与另一个目标合并而被删除
        /// </summary>
        [EnumDescription("为合并删除")]
        DeletedForMerge = 4,

        /// <summary>
        /// 合并为新的，合并后产生的新目标
        /// </summary>
        [EnumDescription("合并为新的")]
        NewFromMerge = 5
    }

    /// <summary>
    /// 存在概率
    /// </summary>
    public enum ProbOfExist
    {
        /// <summary>
        /// 无效值（指给出的值本身没有意义）
        /// </summary>
        [EnumDescription("无效值")]
        InvalidValue = -1,

        /// <summary>
        /// 无效
        /// </summary>
        [EnumDescription("无效")]
        [EnumAlias("-1")]
        Invalid = 0,

        /// <summary>
        /// 小于25%
        /// </summary>
        [EnumDescription("<25%")]
        [EnumAlias("0")]
        Lt025 = 1,

        /// <summary>
        /// 小于50%
        /// </summary>
        [EnumDescription("<50%")]
        [EnumAlias("0.25")]
        Lt050 = 2,

        /// <summary>
        /// 小于75%
        /// </summary>
        [EnumDescription("<75%")]
        [EnumAlias("0.5")]
        Lt075 = 3,

        /// <summary>
        /// 小于90%
        /// </summary>
        [EnumDescription("<90%")]
        [EnumAlias("0.75")]
        Lt090 = 4,

        /// <summary>
        /// 小于99%
        /// </summary>
        [EnumDescription("<99%")]
        [EnumAlias("0.9")]
        Lt099 = 5,

        /// <summary>
        /// 小于99.9%
        /// </summary>
        [EnumDescription("<99.9%")]
        [EnumAlias("0.99")]
        Lt999 = 6,

        /// <summary>
        /// 小于等于100%
        /// </summary>
        [EnumDescription("<=100%")]
        [EnumAlias("0.999")]
        Lte100 = 7
    }
    #endregion

    #endregion

    #region 仅ARS408/404适用

    #region Cluster模式质量（置信）信息
    /// <summary>
    /// 集群虚影概率的范围，越小越好
    /// </summary>
    public enum FalseAlarmProbability
    {
        /// <summary>
        /// 无效值（指给出的值本身没有意义）
        /// </summary>
        [EnumDescription("无效值")]
        InvalidValue = -1,

        /// <summary>
        /// 无效数值
        /// </summary>
        [EnumDescription("无错报")]
        Invalid = 0x0,

        /// <summary>
        /// 小于25%
        /// </summary>
        [EnumDescription("<25%")]
        lt25 = 0x1,

        /// <summary>
        /// 小于50%
        /// </summary>
        [EnumDescription("<50%")]
        lt50 = 0x2,

        /// <summary>
        /// 小于75%
        /// </summary>
        [EnumDescription("<75%")]
        lt75 = 0x3,

        /// <summary>
        /// 小于90%
        /// </summary>
        [EnumDescription("<90%")]
        lt90 = 0x4,

        /// <summary>
        /// 小于99%
        /// </summary>
        [EnumDescription("<99%")]
        lt99 = 0x5,

        /// <summary>
        /// 小于99.9%
        /// </summary>
        [EnumDescription("<99.9%")]
        lt999 = 0x6,

        /// <summary>
        /// 小于等于100%
        /// </summary>
        [EnumDescription("<=100%")]
        lte100 = 0x7
    }

    /// <summary>
    /// 不确定状态的类型
    /// </summary>
    public enum AmbigState
    {
        /// <summary>
        /// 无效值（指给出的值本身没有意义）
        /// </summary>
        [EnumDescription("无效值")]
        InvalidValue = -1,

        /// <summary>
        /// 无效值
        /// </summary>
        [EnumDescription("无效值")]
        Invalid = 0x0,

        /// <summary>
        /// 模糊（因为黑暗、模糊等含糊的状态使集群不清晰）
        /// </summary>
        [EnumDescription("模糊")]
        Ambiguous = 0x1,

        /// <summary>
        /// 意义不明
        /// </summary>
        [EnumDescription("Staggered Ramp")]
        StaggeredRamp = 0x2,

        /// <summary>
        /// 清晰（一切都很清晰，模糊处已解决）
        /// </summary>
        [EnumDescription("清晰")]
        Unambiguous = 0x3,

        /// <summary>
        /// 可能的静止点（模糊处已解决，可能有静止的物体）
        /// </summary>
        [EnumDescription("可能的静止点")]
        StationaryCandidates = 0x4
    }

    /// <summary>
    /// 集群的有效状态
    /// </summary>
    public enum InvalidState
    {
        /// <summary>
        /// 无效值（指给出的值本身没有意义）
        /// </summary>
        [EnumDescription("无效值")]
        InvalidValue = -1,

        /// <summary>
        /// 有效
        /// </summary>
        [EnumDescription("有效")]
        Valid = 0x0,

        /// <summary>
        /// Invalid due to low RCS（无效，低RCS）
        /// </summary>
        [EnumDescription("无效，低RCS")]
        Invalid_LowRCS = 0x1,

        /// <summary>
        /// Invalid due to near-field artefact（无效，近距离干扰）
        /// </summary>
        [EnumDescription("无效，近距离干扰")]
        Invalid_NearFieldArtefact = 0x2,

        /// <summary>
        /// Invalid far range Cluster because not confirmed in near range（远距离集群无效，由于近距离集群无法确定）
        /// </summary>
        [EnumDescription("远距离集群无效，由于近距离集群无法确定")]
        InvalidFarRangeCluster = 0x3,

        /// <summary>
        /// Valid Cluster with low RCS（有效集群，低RCS）
        /// </summary>
        [EnumDescription("有效集群，低RCS")]
        ValidCluster_LowRCS = 0x4,

        /// <summary>
        /// 预留
        /// </summary>
        [EnumDescription("预留")]
        Reserved1 = 0x5,

        /// <summary>
        /// Invalid Cluster due to high mirror probability（无效集群，高反射概率导致）
        /// </summary>
        [EnumDescription("无效集群，高反射概率导致")]
        InvalidCluster_HighMirrorP = 0x6,

        /// <summary>
        /// Invalid because outside sensor field of view（无效，由于处在传感器视野外部）
        /// </summary>
        [EnumDescription("无效，由于处在传感器视野外部")]
        Invalid_OutsideSensorFov = 0x7,

        /// <summary>
        /// Valid Cluster with azimuth correction due to elevation（有效集群，方位角修正后）
        /// </summary>
        [EnumDescription("有效集群，方位角修正后")]
        ValidCluster_AzimuthCorrection = 0x8,

        /// <summary>
        /// Valid Cluster with high child probability
        /// </summary>
        [EnumDescription("Valid Cluster with high child probability")]
        ValidCluster_HighChildP = 0x9,

        /// <summary>
        /// Valid Cluster with high probability of being a 50 deg artefact（有效集群，很可能存在一个50°的假象）
        /// </summary>
        [EnumDescription("有效集群，很可能存在一个50°的假象")]
        ValidCluster_50DegArtefact = 0xa,

        /// <summary>
        /// Valid Cluster but no local maximum（有效集群，但没有本地最大值）
        /// </summary>
        [EnumDescription("有效集群，但没有本地最大值")]
        ValidCluster_NoLocalMaximum = 0xb,

        /// <summary>
        /// Valid Cluster with high artefact probability（有效集群，有高概率产生假象）
        /// </summary>
        [EnumDescription("有效集群，有高概率产生假象")]
        ValidCluster_HighArtefactP = 0xc,

        /// <summary>
        /// 预留
        /// </summary>
        [EnumDescription("预留")]
        Reserved2 = 0xd,

        /// <summary>
        /// Invalid Cluster because it is a harmonics（无效集群，只是谐波）
        /// </summary>
        [EnumDescription("无效集群，只是谐波")]
        InvalidCluster_Harmonics = 0xe,

        /// <summary>
        /// Valid Cluster above 95 m in near range（有效集群，近距离超过95米）
        /// </summary>
        [EnumDescription("有效集群，近距离超过95米")]
        ValidCluster_95mNearRange = 0xf,

        /// <summary>
        /// Valid Cluster with high multi-target probability（无效集群，较高概率有多目标）
        /// </summary>
        [EnumDescription("有效集群，较高概率有多目标")]
        ValidCluster_HighMultiTargetP = 0x10,

        /// <summary>
        /// Valid Cluster with suspicious angle（有效集群，有可疑角度）
        /// </summary>
        [EnumDescription("有效集群，有可疑角度")]
        ValidCluster_SuspiciousAngle = 0x11
    }
    #endregion

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

    #endregion

    /// <summary>
    /// 雷达类型
    /// </summary>
    public enum RadarModel
    {
        /// <summary>
        /// ARS408-XXX系列，兼容ARS404
        /// </summary>
        Ars408_404 = 1,

        /// <summary>
        /// RHP P19/H06系列，俗称大白壳
        /// </summary>
        Rhd_P19 = 2,

        /// <summary>
        /// FD-RSH300-AE，3D雷达，目前仅在日照使用(2026-1-26)
        /// </summary>
        Fd_Rsh300_Ae = 3
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
        Object = 1,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 2
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

    /// <summary>
    /// 防御模式
    /// </summary>
    public enum DefenseMode
    {
        /// <summary>
        /// 点
        /// </summary>
        Vertex = 1,

        /// <summary>
        /// 线
        /// </summary>
        Line = 2,

        /// <summary>
        /// 面
        /// </summary>
        Face = 3
    }
}
