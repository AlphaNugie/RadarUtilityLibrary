using ArsLibrary.Core;
using CommonLib.Function;
using CommonLib.Function.Fitting;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 雷达基础实体类
    /// </summary>
    [ProtoContract]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class RadarBase: IComparable<RadarBase>
    {
        #region 私有成员
        //private const string RCS_MIN_FIELD_FLAT = "rcs_min", RCS_MIN_FIELD_VERT = "rcs_min_alt";
        //private const string RCS_MAX_FIELD_FLAT = "rcs_max", RCS_MAX_FIELD_VERT = "rcs_max_alt";
        private double _degree_yoz, _degree_xoy, _degree_xoz, _degree_base_yoz, _degree_general;
        private Matrix<double> _mat_yoz, _mat_xoy, _mat_xoz, _mat_base_yoz, _mat_general, _mat_overall;

        /// <summary>
        /// 上一个距离
        /// </summary>
        protected double _previous = ArsConst.DEF_DIST;

        /// <summary>
        /// 当前距离（默认测距距离）
        /// </summary>
        protected double _current = ArsConst.DEF_DIST;

        /// <summary>
        /// 威胁级数
        /// </summary>
        protected int _threat_level = 0;

        /// <summary>
        /// 对应的雷达数据写入单元
        /// </summary>
        protected CommUnit _commUnit = null;
        #endregion

        #region 属性
        /// <summary>
        /// 帧消息处理类
        /// </summary>
        public DataFrameBase Infos { get; private set; }

        /// <summary>
        /// ID
        /// </summary>
        [ProtoMember(1)]
        [JsonProperty]
        public int Id { get; set; }

        /// <summary>
        /// 雷达名称
        /// </summary>
        [ProtoMember(2)]
        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 雷达代号
        /// </summary>
        public string Code { get; set; }

        #region 数据
        /// <summary>
        /// 工作状态，1 收到数据，0 未收到数据
        /// </summary>
        [ProtoMember(3)]
        [JsonProperty]
        public int Working
        {
            get { return State.Working; }
            set { State.Working = value; }
        }

        /// <summary>
        /// 雷达状态信息
        /// </summary>
        public RadarState State { get; set; }

        /// <summary>
        /// 刷新时间间隔
        /// </summary>
        public int RefreshInterval { get; set; } = 100;

        /// <summary>
        /// 是否使用给定值
        /// </summary>
        public bool UseGivenValue { get; set; }

        /// <summary>
        /// 给定值
        /// </summary>
        public double ValueGiven { get; set; }

        /// <summary>
        /// 当前障碍物距离
        /// </summary>
        [ProtoMember(4)]
        [JsonProperty]
        public double CurrentDistance
        {
            //假如未收到数据，则返回默认距离
            get { return Working == 1 ? _current : ArsConst.DEF_DIST; }
            set
            {
                if (!ProcessDistanceValue(ref value))
                    return;
                _previous = _current;
                _current = Math.Round(value, 3);

                //数据信号写入文件
                string signals = GetRadarSignals();
                if (_commUnit != null)
                    _commUnit.DataSource = signals;
            }
        }

        /// <summary>
        /// 雷达正对的平面拟合后与水平面的夹角
        /// </summary>
        public double SurfaceAngle { get; set; }

        /// <summary>
        /// 报警级数
        /// </summary>
        [ProtoMember(5)]
        [JsonProperty]
        public int ThreatLevel
        {
            get { return _threat_level; }
            set { _threat_level = value; }
        }

        ///// <summary>
        ///// 报警级数2进制字符串（2位）
        ///// </summary>
        //[ProtoMember(6)]
        //[Obsolete]
        //public string ThreatLevelBinary { get; set; }
        #endregion

        #region 通讯与地址
        /// <summary>
        /// IP地址
        /// </summary>
        [ProtoMember(6)]
        public string IpAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// 端口
        /// </summary>
        [ProtoMember(7)]
        public ushort Port { get; set; } = 20001;

        ///// <summary>
        ///// IP地址+端口
        ///// </summary>
        //public string Address { get; set; }

        /// <summary>
        /// 连接模式
        /// </summary>
        public ConnectionMode ConnectionMode { get; set; } = ConnectionMode.TCP_CLIENT;

        /// <summary>
        /// 是否使用本地IP与端口
        /// </summary>
        public bool UsingLocal { get; set; }

        /// <summary>
        /// 本地IP
        /// </summary>
        public string IpAddressLocal { get; set; }

        /// <summary>
        /// 本地端口
        /// </summary>
        public int PortLocal { get; set; }
        #endregion

        /// <summary>
        /// 所属大机ID
        /// </summary>
        public int OwnerShiploaderId { get; set; }

        /// <summary>
        /// 所属雷达组
        /// </summary>
        public int OwnerGroupId { get; set; }

        /// <summary>
        /// 雷达组类型，1 臂架，2 溜筒，3 门腿
        /// </summary>
        [ProtoMember(8)]
        public RadarGroupType GroupType { get; set; }

        #region 角度与转换
        /// <summary>
        /// 第一个角度：YOZ平面内旋转角度
        /// </summary>
        public double DegreeYoz
        {
            get { return _degree_yoz; }
            set
            {
                _degree_yoz = value;
                _mat_yoz = SpaceOrienting.GetAngleOrientedMatrix(_degree_yoz, AxisType.X);
                UpdateRatios();
            }
        }

        /// <summary>
        /// 第二个角度：XOY平面内旋转角度
        /// </summary>
        public double DegreeXoy
        {
            get { return _degree_xoy; }
            set
            {
                _degree_xoy = value;
                _mat_xoy = SpaceOrienting.GetAngleOrientedMatrix(_degree_xoy, AxisType.Z);
                UpdateRatios();
            }
        }

        /// <summary>
        /// 第三个角度：XOZ平面内旋转角度
        /// </summary>
        public double DegreeXoz
        {
            get { return _degree_xoz; }
            set
            {
                _degree_xoz = value;
                _mat_xoz = SpaceOrienting.GetAngleOrientedMatrix(_degree_xoz, AxisType.Y);
                UpdateRatios();
            }
        }

        /// <summary>
        /// 第四个角度：垂直于地面的轴的整体旋转角度，面向正南为0，面向海（东）为90，面向北为180，面向陆地（西）为270（或-90）
        /// </summary>
        public double DegreeBaseYoz
        {
            get { return _degree_base_yoz; }
            set
            {
                _degree_base_yoz = value;
                _mat_base_yoz = SpaceOrienting.GetAngleOrientedMatrix(_degree_base_yoz, AxisType.X);
                UpdateRatios();
            }
        }

        /// <summary>
        /// 第五个角度：垂直于地面的轴的整体旋转角度，面向正南为0，面向海（东）为90，面向北为180，面向陆地（西）为270（或-90）
        /// </summary>
        public double DegreeGeneral
        {
            get { return _degree_general; }
            set
            {
                _degree_general = value;
                _mat_general = SpaceOrienting.GetAngleOrientedMatrix(_degree_general, AxisType.Z);
                UpdateRatios();
            }
        }

        /// <summary>
        /// 修改后的X坐标的原XY坐标参数
        /// </summary>
        public CoordinateRatios XmodifiedRatios { get; set; }

        /// <summary>
        /// 修改后的Y坐标的原XY坐标参数
        /// </summary>
        public CoordinateRatios YmodifiedRatios { get; set; }

        /// <summary>
        /// 修改后的Z坐标的原XY坐标参数
        /// </summary>
        public CoordinateRatios ZmodifiedRatios { get; set; }
        #endregion

        #region 测距与检测
        /// <summary>
        /// 方向：123456，海北陆南上下
        /// </summary>
        [ProtoMember(9)]
        public Directions Direction { get; set; }

        /// <summary>
        /// 防御模式：1 点，2 线，3 面
        /// </summary>
        public int DefenseMode { get; set; }

        /// <summary>
        /// 距离校正值，以此值校正距防御边界的距离
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// X坐标校正值
        /// </summary>
        public double XOffset { get; private set; }

        /// <summary>
        /// Y坐标校正值
        /// </summary>
        public double YOffset { get; private set; }

        /// <summary>
        /// Z坐标校正值
        /// </summary>
        public double ZOffset { get; private set; }

        private int _rcsMin;
        /// <summary>
        /// RCS最小值
        /// </summary>
        public int RcsMinimum
        {
            get { return Infos == null ? _rcsMin : Infos.RcsMinimum; }
            set
            {
                _rcsMin = value;
                if (Infos != null)
                    Infos.RcsMinimum = value;
            }
        }

        private int _rcsMax;
        /// <summary>
        /// RCS最大值
        /// </summary>
        public int RcsMaximum
        {
            get { return Infos == null ? _rcsMax : Infos.RcsMaximum; }
            set
            {
                _rcsMax = value;
                if (Infos != null)
                    Infos.RcsMaximum = value;
            }
        }
        #endregion

        #region 坐标系坐标限制
        /// <summary>
        /// 是否限制雷达坐标系坐标
        /// </summary>
        public bool RadarCoorsLimited { get; set; }

        /// <summary>
        /// 限制在雷达坐标范围内或范围外，true则限制在范围内，false则限制在范围外
        /// </summary>
        public bool WithinRadarLimit { get; set; }

        /// <summary>
        /// 雷达坐标系X轴最小值
        /// </summary>
        public double RadarxMin { get; set; }

        /// <summary>
        /// 雷达坐标系x轴最大值
        /// </summary>
        public double RadarxMax { get; set; }

        /// <summary>
        /// 雷达坐标系y轴最小值
        /// </summary>
        public double RadaryMin { get; set; }

        /// <summary>
        /// 雷达坐标系y轴最大值
        /// </summary>
        public double RadaryMax { get; set; }

        /// <summary>
        /// 是否限制单机坐标系坐标
        /// </summary>
        public bool ClaimerCoorsLimited { get; set; }

        /// <summary>
        /// 限制在单机坐标范围内或范围外，true则限制在范围内，false则限制在范围外
        /// </summary>
        public bool WithinClaimerLimit { get; set; }

        /// <summary>
        /// 单机坐标系X轴最小值
        /// </summary>
        public double ClaimerxMin { get; set; }

        /// <summary>
        /// 单机坐标系X轴最大值
        /// </summary>
        public double ClaimerxMax { get; set; }

        /// <summary>
        /// 单机坐标系y轴最小值
        /// </summary>
        public double ClaimeryMin { get; set; }

        /// <summary>
        /// 单机坐标系y轴最大值
        /// </summary>
        public double ClaimeryMax { get; set; }

        /// <summary>
        /// 单机坐标系z轴最小值
        /// </summary>
        public double ClaimerzMin { get; set; }

        /// <summary>
        /// 单机坐标系z轴最小值
        /// </summary>
        public double ClaimerzMax { get; set; }
        #endregion

        #region 角度限制
        /// <summary>
        /// 是否限制角度
        /// </summary>
        public bool AngleLimited { get; set; }

        /// <summary>
        /// 限制在角度范围内或范围外，true则限制在范围内，false则限制在范围外
        /// </summary>
        public bool WithinAngleLimit { get; set; }

        /// <summary>
        /// 角度最小值
        /// </summary>
        public double AngleMin { get; set; }

        /// <summary>
        /// 角度最大值
        /// </summary>
        public double AngleMax { get; set; }
        #endregion

        #region 检测特性
        /// <summary>
        /// 是否应用集群或目标过滤器
        /// </summary>
        public bool ApplyFilter { get; set; }

        /// <summary>
        /// 是否应用迭代
        /// </summary>
        public bool ApplyIteration { get; set; }

        /// <summary>
        /// push finalization的最大次数，雷达帧的累积周期
        /// </summary>
        public int PushfMaxCount { get; set; }

        /// <summary>
        /// 是否使用公共过滤条件
        /// </summary>
        public bool UsePublicFilters { get; set; }

        /// <summary>
        /// 错误警报概率过滤器
        /// </summary>
        public List<FalseAlarmProbability> FalseAlarmFilter { get; set; }

        /// <summary>
        /// 径向速度不确定性过滤器
        /// </summary>
        public List<AmbigState> AmbigStateFilter { get; set; }

        /// <summary>
        /// 有效性(有效/高概率多目标)
        /// </summary>
        public List<InvalidState> InvalidStateFilter { get; set; }

        /// <summary>
        /// 测量状态过滤器
        /// </summary>
        public List<MeasState> MeasStateFilter { get; set; }

        /// <summary>
        /// 存在概率过滤器
        /// </summary>
        public List<ProbOfExist> ProbOfExistFilter { get; set; }

        private string false_alarm_string;
        /// <summary>
        /// 错误警报概率过滤器
        /// </summary>
        public string FalseAlarmFilterString
        {
            get { return false_alarm_string; }
            set
            {
                //false_alarm_string = value == null ? string.Empty : value;
                false_alarm_string = value ?? string.Empty;
                FalseAlarmFilter = false_alarm_string.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (FalseAlarmProbability)int.Parse(p)).ToList();
            }
        }

        private string ambig_state_string;
        /// <summary>
        /// 径向速度不确定性过滤器
        /// </summary>
        public string AmbigStateFilterString
        {
            get { return ambig_state_string; }
            set
            {
                //ambig_state_string = value == null ? string.Empty : value;
                ambig_state_string = value ?? string.Empty;
                AmbigStateFilter = ambig_state_string.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (AmbigState)int.Parse(p)).ToList();
            }
        }

        private string invalid_state_string;
        /// <summary>
        /// 有效性(有效/高概率多目标)
        /// </summary>
        public string InvalidStateFilterString
        {
            get { return invalid_state_string; }
            set
            {
                //invalid_state_string = value == null ? string.Empty : value;
                invalid_state_string = value ?? string.Empty;
                InvalidStateFilter = invalid_state_string.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (InvalidState)int.Parse(p)).ToList();
            }
        }

        private string meas_state_string;
        /// <summary>
        /// 测量状态过滤器
        /// </summary>
        public string MeasStateFilterString
        {
            get { return meas_state_string; }
            set
            {
                //meas_state_string = value == null ? string.Empty : value;
                meas_state_string = value ?? string.Empty;
                MeasStateFilter = meas_state_string.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (MeasState)int.Parse(p)).ToList();
            }
        }

        private string prob_exist_string;
        /// <summary>
        /// 存在概率过滤器
        /// </summary>
        public string ProbOfExistFilterString
        {
            get { return prob_exist_string; }
            set
            {
                //prob_exist_string = value == null ? string.Empty : value;
                prob_exist_string = value ?? string.Empty;
                ProbOfExistFilter = prob_exist_string.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => (ProbOfExist)int.Parse(p)).ToList();
            }
        }
        #endregion

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        #endregion

        #region 构造器
        /// <summary>
        /// 默认构造器
        /// </summary>
        public RadarBase()
        {
            //Infos = new DataFrameBase(this);
            Infos = GetDataFrameEntity();
            //if (Infos == null)
            //    throw new ArgumentNullException("DataFrame实体类为空", nameof(Infos));
            State = new RadarState();
            Id = -1;
            Name = "ARS408-21";
            Direction = Directions.None;
            GroupType = RadarGroupType.None;
            InitFieldValues();
            //RefreshInterval = BaseConst.RefreshInterval;
            //IpAddress = BaseConst.IpAddress;
            //Port = BaseConst.Port;
            //ConnectionMode = BaseConst.ConnectionMode;
            //UsingLocal = BaseConst.UsingLocal;
            //PortLocal = BaseConst.Port_Local;
            DegreeYoz = 0;
            DegreeXoy = 0;
            DegreeXoz = 0;
            DegreeBaseYoz = 0;
            DegreeGeneral = 0;
            RcsMinimum = ArsConst.RcsMinimum;
            RcsMaximum = ArsConst.RcsMaximum;
            ApplyFilter = true;
            ApplyIteration = true;
            PushfMaxCount = 1;
            UsePublicFilters = true;
            FalseAlarmFilterString = AmbigStateFilterString = InvalidStateFilterString = MeasStateFilterString = ProbOfExistFilterString = string.Empty;
        }

        /// <summary>
        /// 构造器，从公共变量获取属性值，再用给定的DataRow对象覆盖各属性的值
        /// </summary>
        /// <param name="row"></param>
        public RadarBase(DataRow row) : this()
        {
            if (row == null)
                return;

            Id = row.Convert<int>("radar_id");
            Name = row.Convert<string>("radar_name");
            //RefreshInterval = row.Convert("refresh_interval", 100);
            IpAddress = row.Convert<string>("ip_address");
            Port = row.Convert<ushort>("port");
            ConnectionMode = (ConnectionMode)row.Convert("conn_mode_id", 1);
            UsingLocal = row.Convert<int>("using_local") == 1;
            IpAddressLocal = row.Convert<string>("ip_address_local");
            PortLocal = row.Convert<int>("port_local");
            OwnerShiploaderId = row.Convert<int>("shiploader_id");
            OwnerGroupId = row.Convert<int>("owner_group_id");
            GroupType = (RadarGroupType)row.Convert<int>("group_type");
            DegreeYoz = row.Convert<double>("degree_yoz");
            DegreeXoy = row.Convert<double>("degree_xoy");
            DegreeXoz = row.Convert<double>("degree_xoz");
            DegreeBaseYoz = row.Convert<double>("degree_base_yoz");
            DegreeGeneral = row.Convert<double>("degree_general");
            Direction = (Directions)row.Convert<int>("direction_id");
            DefenseMode = row.Convert<int>("defense_mode_id");
            Offset = row.Convert<double>("offset");
            Remark = row.Convert<string>("remark");
            RefreshRcsLimits();
            #region 坐标或角度限制
            RadarCoorsLimited = row.Convert<int>("radar_coors_limited") == 1;
            WithinRadarLimit = row.Convert("within_radar_limit", 1) == 1;
            RadarxMin = row.Convert<double>("radar_x_min");
            RadarxMax = row.Convert<double>("radar_x_max");
            RadaryMin = row.Convert<double>("radar_y_min");
            RadaryMax = row.Convert<double>("radar_y_max");
            ClaimerCoorsLimited = row.Convert<int>("claimer_coors_limited") == 1;
            WithinClaimerLimit = row.Convert("within_claimer_limit", 1) == 1;
            ClaimerxMin = row.Convert<double>("claimer_x_min");
            ClaimerxMax = row.Convert<double>("claimer_x_max");
            ClaimeryMin = row.Convert<double>("claimer_y_min");
            ClaimeryMax = row.Convert<double>("claimer_y_max");
            ClaimerzMin = row.Convert<double>("claimer_z_min");
            ClaimerzMax = row.Convert<double>("claimer_z_max");
            AngleLimited = row.Convert<int>("angle_limited") == 1;
            WithinAngleLimit = row.Convert("within_angle_limit", 1) == 1;
            AngleMin = row.Convert<double>("angle_min");
            AngleMax = row.Convert<double>("angle_max");
            #endregion
            #region 检测特性
            ApplyFilter = row.Convert<int>("apply_filter") == 1;
            ApplyIteration = row.Convert<int>("apply_iteration") == 1;
            PushfMaxCount = row.Convert<int>("pushf_max_count");
            UsePublicFilters = row.Convert<int>("use_public_filters") == 1;
            FalseAlarmFilterString = row.Convert<string>("false_alarm_filter");
            AmbigStateFilterString = row.Convert<string>("ambig_state_filter");
            InvalidStateFilterString = row.Convert<string>("invalid_state_filter");
            MeasStateFilterString = row.Convert<string>("meas_state_filter");
            ProbOfExistFilterString = row.Convert<string>("prob_exist_filter");
            #endregion
            //double given;
        }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 在给CurrentDistance赋值前对输入的距离值根据需要进行预处理，假如输入值符合需求返回true，否则返回false
        /// </summary>
        /// <param name="dist">输入的距离值</param>
        /// <returns></returns>
        protected abstract bool ProcessDistanceValue(ref double dist);

        /// <summary>
        /// 获取CommUnit写入的雷达信号数据（默认为空字符串）
        /// </summary>
        /// <returns></returns>
        protected abstract string GetRadarSignals();

        /// <summary>
        /// 初始化并返回继承DataFrameBase的子类的一个实体
        /// </summary>
        /// <returns></returns>
        protected abstract DataFrameBase GetDataFrameEntity();

        /// <summary>
        /// 进行必要的操作以给本地变量赋初始值
        /// </summary>
        protected abstract void InitFieldValues();

        /// <summary>
        /// （从数据源）刷新RCS值范围
        /// </summary>
        public abstract void RefreshRcsLimits();
        #endregion

        #region 对象比较
        /// <summary>
        /// 返回此实例的哈希代码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return CurrentDistance.GetHashCode() | ThreatLevel.GetHashCode();
        }

        #region 是否相等的比较
        /// <summary>
        /// 判断当前实例与某对象是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is RadarBase radar && CurrentDistance == radar.CurrentDistance && ThreatLevel == radar.ThreatLevel;
        }

        /// <summary>
        /// 重新定义的相等符号
        /// </summary>
        /// <param name="left">左侧实例</param>
        /// <param name="right">右侧实例</param>
        /// <returns></returns>
        public static bool operator ==(RadarBase left, RadarBase right)
        {
            //return (object)left == null ? (object)right == null : left.Equals(right);
            return left is null ? right is null : left.Equals(right);
        }

        /// <summary>
        /// 重新定义的不等符号
        /// </summary>
        /// <param name="left">左侧实例</param>
        /// <param name="right">右侧实例</param>
        /// <returns></returns>
        public static bool operator !=(RadarBase left, RadarBase right)
        {
            return !(left == right);
        }
        #endregion

        #region 大小的比较
        /// <summary>
        /// 将当前实例与另一实例相比较，并返回比较结果符号：-1 小于，0 相等，1 大于
        /// </summary>
        /// <param name="other">与当前实例比较的另一实例</param>
        /// <returns></returns>
        public int CompareTo(RadarBase other)
        {
            int d = CurrentDistance.CompareTo(other.CurrentDistance), a = ThreatLevel.CompareTo(other.ThreatLevel);
            int result;
            if (d == 0 && a == 0)
                result = 0;
            else
                result = (d == -1 && a != -1) || (d != -1 && a == 1) ? -1 : 1;
            return result;
        }

        /// <summary>
        /// 重新定义的小于符号
        /// </summary>
        /// <param name="left">左侧实例</param>
        /// <param name="right">右侧实例</param>
        /// <returns></returns>
        public static bool operator <(RadarBase left, RadarBase right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// 重新定义的小于等于符号
        /// </summary>
        /// <param name="left">左侧实例</param>
        /// <param name="right">右侧实例</param>
        /// <returns></returns>
        public static bool operator <=(RadarBase left, RadarBase right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// 重新定义的大于符号
        /// </summary>
        /// <param name="left">左侧实例</param>
        /// <param name="right">右侧实例</param>
        /// <returns></returns>
        public static bool operator >(RadarBase left, RadarBase right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// 重新定义的大于等于符号
        /// </summary>
        /// <param name="left">左侧实例</param>
        /// <param name="right">右侧实例</param>
        /// <returns></returns>
        public static bool operator >=(RadarBase left, RadarBase right)
        {
            return left.CompareTo(right) >= 0;
        }
        #endregion
        #endregion

        /// <summary>
        /// 更新修改后XYZ坐标的系数
        /// </summary>
        public void UpdateRatios()
        {
            //每个旋转角度均有变换矩阵，计算所有这些矩阵相乘之后的最终矩阵，假如计算失败（或至少有一个为空），直接跳出
            //第一次初始化时可能有为空的矩阵
            if (_mat_general == null || _mat_base_yoz == null || _mat_xoz == null || _mat_xoy == null || _mat_yoz == null)
                return;
            try { _mat_overall = _mat_general * _mat_base_yoz * _mat_xoz * _mat_xoy * _mat_yoz; }
            catch (Exception) { return; }
            double[,] F = _mat_overall.ToArray();
            //X          x
            //Y =   F *  y
            //Z          0
            //最终矩阵F为3X3矩阵，此矩阵中与最终坐标XYZ所对应的每行的第一个元素为雷达坐标x的系数、第二个元素为雷达坐标y的系数，索引1为行索引，索引2为列索引
            XmodifiedRatios = new CoordinateRatios { Xratio = F[0, 0], Yratio = F[0, 1] };
            YmodifiedRatios = new CoordinateRatios { Xratio = F[1, 0], Yratio = F[1, 1] };
            ZmodifiedRatios = new CoordinateRatios { Xratio = F[2, 0], Yratio = F[2, 1] };

            ////不考虑底座YOZ角度的XY坐标系数计算公式
            //XmodifiedRatios = new CoordinateRatios() { Xratio = _cosphi * _coslamda * _cosg - _sinphi * _sing, Yratio = 0 - _sintheta * _sinlamda * _cosg - _costheta * _sinphi * _coslamda * _cosg - _costheta * _cosphi * _sing };
            //YmodifiedRatios = new CoordinateRatios() { Xratio = _cosphi * _coslamda * _sing + _sinphi * _cosg, Yratio = _costheta * _cosphi * _cosg - _costheta * _sinphi * _coslamda * _sing - _sintheta * _sinlamda * _sing };
            //ZmodifiedRatios = new CoordinateRatios() { Xratio = _cosphi * _sinlamda, Yratio = _sintheta * _coslamda - _costheta * _sinphi * _sinlamda };
        }

        /// <summary>
        /// 获取雷达信息字符串
        /// </summary>
        /// <returns></returns>
        public string GetRadarString()
        {
            string result = string.Empty;
            if (Infos != null)
            {
                result = string.Format(@"  雷达ID: {0}
  是否工作: {1},
  距离: {2},", PortLocal + "_" + Name, Infos.RadarState.Working, Infos.CurrentDistance);
            }

            return result;
        }
    }

    /// <summary>
    /// 修改后的坐标中原XY坐标的系数
    /// </summary>
    public class CoordinateRatios
    {
        /// <summary>
        /// 原X坐标的系数
        /// </summary>
        public double Xratio;

        /// <summary>
        /// 原Y坐标的系数
        /// </summary>
        public double Yratio;

        /// <summary>
        /// 获取字符串描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0:f4}, {1:f4}", Xratio, Yratio);
        }
    }
}
