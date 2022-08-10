using ArsLibrary.Core;
using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 帧消息处理基础类
    /// </summary>
    public abstract class DataFrameBase
    {
        #region 私有成员
        private readonly Regex pattern = new Regex(ArsConst.Pattern_SensorMessage, RegexOptions.Compiled);
        private SensorGeneral _general_most_threat;
        private int _count = 0;
        //private readonly int limit_factor = 1;
        private double _new, _assumed, _diff, _diff1;
        private int _pushf_counter = 0; //计算push finalization的次数
        private readonly int _id_step = 500; //累积不同帧的点时为防止ID重复所添加的ID步长（与_pushf_counter结合使用）

        //public static bool tcp_applied = false; //TCP模式是否已应用
        //private readonly AlarmRecordTask _alarmTask;
        //private readonly Stopwatch _stopWatch = new Stopwatch();
        //private readonly CommUnit _commUnit = null;
        //private readonly DataService_Radar dataService_Radar = new DataService_Radar();
        #endregion

        #region 属性
        /// <summary>
        /// push finalization的最大次数
        /// </summary>
        private int PushfMaxCount { get { return Radar.PushfMaxCount; } }

        /// <summary>
        /// 雷达信息对象
        /// </summary>
        public RadarBase Radar { get; set; }

        /// <summary>
        /// 帧消息的过滤条件集合
        /// </summary>
        public FrameFilterFlags FilterFlags { get; } = new FrameFilterFlags();

        private int _rcsMinimum = -64;
        /// <summary>
        /// RCS最小值
        /// </summary>
        public int RcsMinimum
        {
            //是否使用公共RCS值范围
            get { return ArsConst.UsePublicRcsRange ? ArsConst.RcsMinimum : _rcsMinimum; }
            set
            {
                if (ArsConst.UsePublicRcsRange)
                    ArsConst.RcsMinimum = value;
                else
                {
                    _rcsMinimum = value;
                    UpdateRadarRcsMinValue();
                }
            }
        }

        private int _rcsMaximum = 64;
        /// <summary>
        /// RCS最大值
        /// </summary>
        public int RcsMaximum
        {
            get { return ArsConst.UsePublicRcsRange ? ArsConst.RcsMaximum : _rcsMaximum; }
            set
            {
                if (ArsConst.UsePublicRcsRange)
                    ArsConst.RcsMaximum = value;
                else
                {
                    _rcsMaximum = value;
                    UpdateRadarRcsMaxValue();
                }
            }
        }

        /// <summary>
        /// 雷达状态信息
        /// </summary>
        public RadarState RadarState
        {
            get { return Radar.State; }
            set { Radar.State = value; }
        }

        /// <summary>
        /// 当前检测模式
        /// </summary>
        public SensorMode CurrentSensorMode { get; set; }

        /// <summary>
        /// 接收缓冲区大小（达到此大小则放入正式数据）
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        /// 接收目标实际数量
        /// </summary>
        public int ActualSize { get; set; }

        /// <summary>
        /// 接收缓冲区（存放临时数据，直到接收完一组数据再放入正式数据）
        /// </summary>
        public List<SensorGeneral> ListBuffer { get; set; }

        /// <summary>
        /// 接收缓冲区（除合格数据外其它所有数据）（存放临时数据，直到接收完一组数据再放入正式数据）
        /// </summary>
        public List<SensorGeneral> ListBuffer_AllOther { get; set; }

        /// <summary>
        /// 正式数据
        /// </summary>
        public List<SensorGeneral> ListTrigger { get; set; }

        ///// <summary>
        ///// 正式数据长度
        ///// </summary>
        //public int ListTriggerCount { get => ListTrigger.Count; }

        /// <summary>
        /// 待发送列表
        /// </summary>
        public List<SensorGeneral> ListToSend { get; set; }

        /// <summary>
        /// 最具有威胁的集群或目标点
        /// </summary>
        public SensorGeneral GeneralMostThreat
        {
            get { return _general_most_threat; }
            set
            {
                _general_most_threat = value;
                CurrentDistance = _general_most_threat != null ? Math.Round(_general_most_threat.DistanceToBorder * DistCoeff, 4) : ArsConst.DEF_DIST;
            }
        }

        /// <summary>
        /// 当前距离值的系数（对于装船机溜筒防碰雷达来说是大臂不完全垂直轨道伸出时的旋转修正系数）
        /// </summary>
        public double DistCoeff { get => GetDistCoeff(); }

        /// <summary>
        /// 当前障碍物距离，保留4位小数
        /// </summary>
        public double CurrentDistance
        {
            //get { return Radar._current; }
            get { return Radar.CurrentDistance; }
            set
            {
                ProcessDistanceValue(value);
                ThreatLevel = GetThreatLevel();
            }
        }

        /// <summary>
        /// 迭代过程距离差限定值的比例因子（默认为1）
        /// </summary>
        protected int IteLimitFactor { get => GetIteLimitFactor(); }

        /// <summary>
        /// 报警级数
        /// </summary>
        public int ThreatLevel
        {
            get { return Radar.ThreatLevel; }
            set { ProcessThreatLevelValue(value); }
        }

        private int timer = 0;
        /// <summary>
        /// 雷达无数据输出的计时器（单位：秒）
        /// </summary>
        public int Timer
        {
            get { return timer; }
            set
            {
                timer = value;
                RadarState.Working = timer < 5 ? 1 : 0;
            }
        }

        /// <summary>
        /// 检查雷达工作状态的线程
        /// </summary>
        public Thread ThreadCheck { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="radar">雷达信息对象</param>
        public DataFrameBase(RadarBase radar)
        {
            //Radar = radar == null ? new Radar() : radar;
            Radar = radar ?? throw new ArgumentNullException("雷达对象为空", nameof(radar));
            //_alarmTask = new AlarmRecordTask(Radar);
            //Flags = new List<bool>() { false, false, false, false, false, false, false, true, true, true };
            CurrentSensorMode = SensorMode.Cluster;
            ListBuffer = new List<SensorGeneral>();
            //ListBuffer_Other = new List<SensorGeneral>();
            ListBuffer_AllOther = new List<SensorGeneral>();
            ListTrigger = new List<SensorGeneral>();
            ListToSend = new List<SensorGeneral>();
            ThreadCheck = new Thread(new ThreadStart(CheckIfRadarsWorking)) { IsBackground = true };
            //IsShore = Radar.GroupType == RadarGroupType.Shore; //是否为岸基雷达
            //_commUnit = BaseConst.CommUnits.Find(unit => unit.RemoteIp.Equals(Radar.IpAddress) && unit.Port == Radar.Port);
            //_commUnit.Titles = "日期时间,距离,级别,南北上下左右前后,走行,俯仰,回转,伸缩,时间戳";

            ThreadCheck.Start();
            //_alarmTask.Run();
            //_stopWatch.Start();
        }

        #region 抽象方法
        /// <summary>
        /// 向雷达数据源更新雷达RCS最小值
        /// </summary>
        public abstract void UpdateRadarRcsMinValue();

        /// <summary>
        /// 向雷达数据源更新雷达RCS最大值
        /// </summary>
        public abstract void UpdateRadarRcsMaxValue();

        /// <summary>
        /// 计算并返回当前距离值的系数（默认返回1，对于装船机溜筒防碰雷达来说是大臂不完全垂直于轨道伸出时的旋转修正系数）
        /// </summary>
        /// <returns></returns>
        protected abstract double GetDistCoeff();

        /// <summary>
        /// 根据输入的距离值进行必要的处理并对雷达当前距离CurrentDistance赋值（处理：迭代（所有大机）或向TCP发送数据（装船机））
        /// </summary>
        /// <param name="value"></param>
        protected abstract void ProcessDistanceValue(double value);

        /// <summary>
        /// 根据标准计算并返回威胁等级的值
        /// </summary>
        protected abstract int GetThreatLevel();

        /// <summary>
        /// 根据输入的威胁级数值进行必要的处理并对雷达当前威胁等级ThreatLevel赋值
        /// </summary>
        /// <param name="value"></param>
        protected abstract void ProcessThreatLevelValue(int value);

        /// <summary>
        /// 获取迭代方法距离差限定值的比例因子（比如根据雷达类型决定，默认返回1）
        /// </summary>
        /// <returns></returns>
        protected abstract int GetIteLimitFactor();

        /// <summary>
        /// 检查输入的一般消息对象在各项限制条件下的通过情况（默认不进行操作，对装船机来说是确认坐标点是否在溜筒水平或纵向范围内）
        /// </summary>
        /// <param name="general">输入的一般消息类对象</param>
        protected abstract void CheckFilterFlags(SensorGeneral general);

        /// <summary>
        /// 计算并返回雷达所对平面（假如有）在拟合后与水平面的夹角（默认为0）
        /// </summary>
        /// <returns></returns>
        protected abstract double GetSurfaceAngle();
        #endregion

        #region 功能
        /// <summary>
        /// 循环并累计雷达无数据输出的时间
        /// </summary>
        public void CheckIfRadarsWorking()
        {
            int interval = 1;
            while (true)
            {
                Thread.Sleep(interval * 1000);
                Timer += interval;
            }
        }

        /// <summary>
        /// 处理输入的原始雷达数据，对各类消息ID进行分类并分别处理
        /// </summary>
        /// <param name="input">输入的信息</param>
        public void Filter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            #region 方法3 新正则提取
            MatchCollection col = pattern.Matches(input);
            if (col == null || col.Count == 0)
                return;
            foreach (Match match in col)
            {
                BaseMessage message = new BaseMessage(match.Value);
                dynamic obj;
                switch (message.MessageId)
                {
                    case SensorMessageId_0.RadarState_Out:
                        RadarState.Base = message;
                        break;
                    case SensorMessageId_0.Cluster_0_Status_Out:
                        obj = new ClusterStatus(message);
                        DataPushFinalize();
                        CurrentSensorMode = SensorMode.Cluster;
                        BufferSize = obj.NofClustersNear + obj.NofClustersFar;
                        break;
                    case SensorMessageId_0.Cluster_1_General_Out:
                        obj = new ClusterGeneral(message, Radar);
                        DataPush(obj);
                        ActualSize++;
                        break;
                    case SensorMessageId_0.Cluster_2_Quality_Out:
                        obj = new ClusterQuality(message);
                        DataQualityUpdate(obj);
                        break;
                    case SensorMessageId_0.Obj_0_Status_Out:
                        obj = new ObjectStatus(message);
                        DataPushFinalize();
                        CurrentSensorMode = SensorMode.Object;
                        BufferSize = obj.NofObjects;
                        break;
                    case SensorMessageId_0.Obj_1_General_Out:
                        obj = new ObjectGeneral(message, Radar);
                        DataPush(obj);
                        ActualSize++;
                        break;
                    case SensorMessageId_0.Obj_2_Quality_Out:
                        obj = new ObjectQuality(message);
                        DataQualityUpdate(obj);
                        break;
                    default:
                        continue;
                }
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// 将一般信息压入缓冲区--雷达分组处理
        /// </summary>
        /// <param name="general">一般信息对象</param>
        public void DataPush(SensorGeneral general)
        {
            //double z = general.ModiCoors.Z;

            #region 目标点的过滤
            //Flags[2] = !general.RCS.Between(RcsMinimum, RcsMaximum); //RCS值是否不在范围内
            FilterFlags.RcsInBound = general.RCS.Between(RcsMinimum, RcsMaximum); //RCS值是否不在范围内
            //假如为从配置初始化的雷达
            if (Radar.Id >= 0)
            {
                CheckFilterFlags(general);
                FilterFlags.RadarCoorsInBound = !Radar.RadarCoorsLimited || general.WithinRadarLimits; //雷达坐标系坐标的限制
                FilterFlags.ClaimerCoorsInBound = !Radar.ClaimerCoorsLimited || general.WithinClaimerLimits; //单机坐标系坐标的限制
                FilterFlags.AngleInBound = !Radar.AngleLimited || general.WithinAngleLimits; //角度的限制
            }
            //TODO (所有雷达)过滤条件Lv1：RCS值、坐标在限定范围内 / RCS值在范围内
            //TODO (非溜筒下方)过滤条件Lv2：距边界范围在阈值内，溜筒雷达Z方向坐标不低于大铲最低点
            bool save2list = FilterFlags.IsAllInBound();
            bool save2allother = !save2list;
            #endregion

            general.PushfCounter = _pushf_counter;
            general.Id += _pushf_counter * _id_step;
            if (save2list)
                ListBuffer.Add(general);
            if (save2allother)
                ListBuffer_AllOther.Add(general);
        }

        /// <summary>
        /// 用输入的质量信息去更新一般信息内的字段
        /// </summary>
        /// <param name="q">输入的质量信息对象</param>
        public void DataQualityUpdate(SensorQuality q)
        {
            try
            {
                List<SensorGeneral> list = ListBuffer;
                q.Id += _pushf_counter * _id_step;
                //根据ID找对应的一般信息
                SensorGeneral g = list.Find(c => c.Id == q.Id);
                if (g == null)
                {
                    list = ListBuffer_AllOther;
                    g = list.Find(c => c.Id == q.Id);
                }
                if (g == null)
                    return;

                if (q is ClusterQuality)
                {
                    ClusterQuality quality = q as ClusterQuality;
                    ClusterGeneral general = g as ClusterGeneral;

                    general.Pdh0 = quality.Pdh0;
                    general.InvalidState = quality.InvalidState;
                    general.AmbigState = quality.AmbigState;
                    List<FalseAlarmProbability> listFalseAlarm = Radar.UsePublicFilters ? ClusterQuality.FalseAlarmFilter : Radar.FalseAlarmFilter;
                    List<AmbigState> listAmbigState = Radar.UsePublicFilters ? ClusterQuality.AmbigStateFilter : Radar.AmbigStateFilter;
                    List<InvalidState> listInvalidState = Radar.UsePublicFilters ? ClusterQuality.InvalidStateFilter : Radar.InvalidStateFilter;
                    //TODO 集群模式输出结果过滤条件2：（过滤器启用、过滤器不为空）不在集群/不确定性/有效性过滤器内
                    if (ArsConst.ClusterFilterEnabled && Radar.ApplyFilter && (
                        (listFalseAlarm.Count > 0 && !listFalseAlarm.Contains(general.Pdh0)) ||
                        (listAmbigState.Count > 0 && !listAmbigState.Contains(general.AmbigState)) ||
                        (listInvalidState.Count > 0 && !listInvalidState.Contains(general.InvalidState))))
                    {
                        list.Remove(general);
                        ListBuffer_AllOther.Add(general);
                    }
                }
                else
                {
                    ObjectQuality quality = q as ObjectQuality;
                    ObjectGeneral general = g as ObjectGeneral;

                    general.MeasState = quality.MeasState;
                    general.ProbOfExist = quality.ProbOfExist;
                    List<MeasState> listMeasState = Radar.UsePublicFilters ? ObjectQuality.MeasStateFilter : Radar.MeasStateFilter;
                    List<ProbOfExist> listProbExist = Radar.UsePublicFilters ? ObjectQuality.ProbOfExistFilter : Radar.ProbOfExistFilter;
                    //TODO 目标模式输出结果过滤条件2：（假如过滤器启用）判断存在概率的可能最小值是否小于允许的最低值
                    if (ArsConst.ObjectFilterEnabled && Radar.ApplyFilter && (
                        (listMeasState.Count > 0 && !listMeasState.Contains(general.MeasState)) ||
                        (listProbExist.Count > 0 && !listProbExist.Contains(general.ProbOfExist))))
                    {
                        list.Remove(general);
                        ListBuffer_AllOther.Add(general);
                    }
                }
            }
            catch (Exception) { }
        }

        private readonly Queue<double> surfaceAnglesQueue = new Queue<double>();
        /// <summary>
        /// 结束一个阶段的数据压入，将缓冲区数据汇入正式数据
        /// </summary>
        public void DataPushFinalize()
        {
            //假如应获取的集群/目标数量不为0但实际未收到，则退出（收到了空的帧）
            if (BufferSize != 0 && ActualSize == 0)
                return;

            if (++_pushf_counter >= PushfMaxCount)
            {
                //不要添加ListBuffer_Cluster与ListBuffer_Cluster_Other数量是否均为0的判断，否则当不存在目标时无法及时反映在数据上
                ListBuffer.Sort(SensorGeneral.DistanceComparison); //根据距检测区的最短距离排序
                GeneralMostThreat = ListBuffer.Count() > 0 ? ListBuffer.First() : null; //找出距离最小的点
                ListTrigger.Clear();
                ListToSend.Clear();
                ListTrigger.AddRange(ListBuffer);
                ListToSend.AddRange(ListBuffer);
                if (ArsConst.ShowDesertedPoints)
                    ListTrigger.AddRange(ListBuffer_AllOther);
                ListToSend.AddRange(ListBuffer_AllOther);

                Radar.SurfaceAngle = GetSurfaceAngle();

                ListBuffer.Clear();
                ListBuffer_AllOther.Clear();
                _pushf_counter = 0;
            }

            ActualSize = 0;
        }

        /// <summary>
        /// 用新值来迭代距障碍物的距离
        /// </summary>
        /// <param name="value">新的距离值</param>
        public void IterateDistance(double value)
        {
            _new = value; //新值
            //_diff = Math.Abs(_new - Radar._current); //新值与当前值的差
            _diff = Math.Abs(_new - Radar.CurrentDistance); //新值与当前值的差
            _diff1 = Math.Abs(_new - _assumed); //新值与假定值的差
            //假如未启用迭代 / 当前值为0 / 新值与当前值的差不超过距离限定值：计数置0，用新值取代现有值
            //if (!BaseConst.IterationEnabled || _diff <= BaseConst.IteDistLimit * limit_factor)
            if (!(ArsConst.IterationEnabled && Radar.ApplyIteration) || _diff <= ArsConst.IteDistLimit * IteLimitFactor)
            {
                _count = 0;
                //Radar._current = _new;
                Radar.CurrentDistance = _new;
            }
            //假如新值与当前值的差超过距离限定值，计数刷新，用新值取代假定值
            else
            {
                //假如新值与假定值的差未超过距离限定值，计数+1（否则置0）
                _count = _diff1 <= ArsConst.IteDistLimit * IteLimitFactor ? _count + 1 : 0;
                _assumed = _new;
                //假如计数超过计数限定值，则用新值取代现有值
                if (_count > ArsConst.IteCountLimit)
                {
                    _count = 0;
                    //Radar._current = _new;
                    Radar.CurrentDistance = _new;
                }
            }
        }
    }
}
