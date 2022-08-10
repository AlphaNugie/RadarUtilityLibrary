using ArsLibrary.Core;
using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 传感器消息一般消息基础类
    /// </summary>
    public abstract class SensorGeneral : SensorMessage
    {
        private RadarBase _radar = null;

        #region static
        /// <summary>
        /// 根据距检测区的最短距离排序
        /// </summary>
        public static Comparison<SensorGeneral> DistanceComparison = (a, b) => a.DistanceToBorder.CompareTo(b.DistanceToBorder);

        /// <summary>
        /// 根据转换后Z坐标的大小排序
        /// </summary>
        public static Comparison<SensorGeneral> HeightComparison = (a, b) => a.Z.CompareTo(b.Z);
        #endregion

        #region 属性
        /// <summary>
        /// 所在帧累积未清除的结算(push finalization)次数
        /// </summary>
        public int PushfCounter { get; set; }

        /// <summary>
        /// 消息ID（编号）
        /// </summary>
        public int Id { get; set; }

        private double _dist_long;
        /// <summary>
        /// 纵向（x）坐标，米
        /// </summary>
        public double DistLong
        {
            get { return _dist_long; }
            set
            {
                _dist_long = value;
                CalculateConvertedCoors();
                CalculateAngle();
                CheckIfWithinLimits();
            }
        }

        private double _dist_lat;
        /// <summary>
        /// 横向（y）坐标，米
        /// </summary>
        public double DistLat
        {
            get { return _dist_lat; }
            set
            {
                _dist_lat = value;
                CalculateConvertedCoors();
                CalculateAngle();
                CheckIfWithinLimits();
            }
        }

        /// <summary>
        /// 角度
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// 雷达坐标系内点距离原点的距离
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// 纵向的相对速度（x），米/秒
        /// </summary>
        public double VrelLong { get; set; }

        /// <summary>
        /// 横向的相对速度（y），米/秒
        /// </summary>
        public double VrelLat { get; set; }

        /// <summary>
        /// 转换后的最原始坐标
        /// </summary>
        public ModifiedCoordinates ModiCoors { get; set; }

        /// <summary>
        /// 转换后的X坐标（经过偏移处理）
        /// </summary>
        public double X
        {
            get => ModiCoors.X + (Radar == null ? 0 : Radar.XOffset);
            //set { ModiCoors.X = value - Radar.XOffset; }
        }

        /// <summary>
        /// 转换后的Y坐标（经过偏移处理）
        /// </summary>
        public double Y
        {
            get => ModiCoors.Y + (Radar == null ? 0 : Radar.YOffset);
            //set { ModiCoors.Y = value - Radar.YOffset; }
        }

        /// <summary>
        /// 转换后的Z坐标（经过偏移处理）
        /// </summary>
        public double Z
        {
            get => ModiCoors.Z + (Radar == null ? 0 : Radar.ZOffset);
            //set { ModiCoors.Z = value - Radar.ZOffset; }
        }


        /// <summary>
        /// 是否处于雷达坐标限制范围内
        /// </summary>
        public bool WithinRadarLimits { get; set; }

        /// <summary>
        /// 是否处于单机坐标限制范围内
        /// </summary>
        public bool WithinClaimerLimits { get; set; }

        /// <summary>
        /// 是否位于角度限制范围内
        /// </summary>
        public bool WithinAngleLimits { get; set; }

        private DynProp _prop = new DynProp();
        /// <summary>
        /// 动态属性，指示是否在移动或是否已停止等属性
        /// </summary>
        public DynProp DynProp
        {
            get { return _prop; }
            set
            {
                _prop = value;
                DynPropString = _prop.GetDescription();
                Color = ArsFunc.GetColorByDynProp(_prop, Color);
            }
        }

        /// <summary>
        /// 动态属性的描述
        /// </summary>
        public string DynPropString { get; set; }

        /// <summary>
        /// 动态属性所对应的颜色
        /// </summary>
        public Color Color { get; set; }

        private double _rcs = 0;
        /// <summary>
        /// 雷达散射截面(Radar Crossing Section)，单位 dBm2（分贝，转换为平米：10^(0.1*dB)，例如，-10分贝为0.1平方米）
        /// </summary>
        public double RCS
        {
            get { return _rcs; }
            set
            {
                _rcs = value;
                RCS_M2 = Math.Pow(10, 0.1 * _rcs);
                Color = ArsFunc.GetColorByRcs(_rcs, Color);
            }
        }

        /// <summary>
        /// 雷达散射截面(Radar Crossing Section)，单位 m2（平方米）
        /// </summary>
        public double RCS_M2 { get; set; }

        /// <summary>
        /// 对应雷达信息
        /// </summary>
        public RadarBase Radar
        {
            get { return _radar; }
            protected set { _radar = value; }
        }

        /// <summary>
        /// 距检测边界的距离，与检测方式（点线面）与雷达朝向（海北陆南）有关
        /// </summary>
        public double DistanceToBorder { get; set; }

        /// <summary>
        /// 辅助标志（拟合平面时是否曾修正过坐标）
        /// </summary>
        public bool HelpFlag { get; set; }
        #endregion

        /// <summary>
        /// 基础信息初始化
        /// </summary>
        /// <param name="message">基础信息</param>
        /// <param name="radar">雷达信息</param>
        protected SensorGeneral(BaseMessage message, RadarBase radar)
        {
            Color = Color.FromArgb(255, 255, 255);
            ModiCoors = new ModifiedCoordinates();
            WithinRadarLimits = true;
            WithinClaimerLimits = true;
            Radar = radar;
            Base = message;
        }

        /// <summary>
        /// 将从雷达收到的二进制数据转换为雷达基础信息
        /// </summary>
        /// <param name="binary"></param>
        protected abstract override void DataConvert(string binary);

        /// <summary>
        /// 根据横纵坐标计算角度
        /// </summary>
        public void CalculateAngle()
        {
            Angle = _dist_long == 0 ? Math.Sign(_dist_lat) * 90 : Math.Atan(_dist_lat / _dist_long) * 180 / Math.PI;
            Radius = Math.Sqrt(Math.Pow(_dist_long, 2) + Math.Pow(_dist_lat, 2));
            //AngleYoz = Y == 0 ? Math.Sign(Z) * 90 : Math.Atan(Z / Y) * 180 / Math.PI;
        }

        /// <summary>
        /// 计算转换后坐标
        /// </summary>
        public void CalculateConvertedCoors()
        {
            if (Radar == null || Radar.Id <= 0)
                return;

            Directions dir = Radar.Direction;
            ModiCoors.X = Radar.XmodifiedRatios.Xratio * DistLong + Radar.XmodifiedRatios.Yratio * DistLat;
            ModiCoors.Y = Radar.YmodifiedRatios.Xratio * DistLong + Radar.YmodifiedRatios.Yratio * DistLat;
            ModiCoors.Z = Radar.ZmodifiedRatios.Xratio * DistLong + Radar.ZmodifiedRatios.Yratio * DistLat;
            bool leftright = dir == Directions.Left || dir == Directions.Right; //是否朝向左或右
            double x = leftright ? X : Y, y = leftright ? Y : X, z = Z; //根据方向调换X/Y的值
            int m = Radar.DefenseMode; //防御模式：1 点，2 线，3 面
            //d = (a*x^2+b*z^2+c*y^2)^0.5，其中a, b, c由4-m, 3-m, 2-m的值决定，假如大于0则为1，小于等于0为0（公式形如Math.Sign(4 - m) == 1 ? 1 : 0）
            //含义：面模式，a=1,b=c=0；线模式，a=b=1,c=0；点模式，a=b=c=1
            //假如方向为上下，则只计算竖直方向Z坐标的值
            DistanceToBorder = (dir == Directions.Up || dir == Directions.Down) ? z : Math.Sqrt((Math.Sign(4 - m) == 1 ? 1 : 0) * Math.Pow(x, 2) + (Math.Sign(3 - m) == 1 ? 1 : 0) * Math.Pow(z, 2) + (Math.Sign(2 - m) == 1 ? 1 : 0) * Math.Pow(y, 2));
            DistanceToBorder = (dir == Directions.Down ? -1 : 1) * DistanceToBorder + Radar.Offset; //当方向向下时，在距离前乘以一个值为-1的系数（向下指时Z坐标均为负数）
            //假如防御模式为面，再添加处理步骤：乘以x的符号，效果为使边界距离带符号；假如面向左或后，则再乘以-1（所面向方向坐标均为负数）
            if (m == 3 && dir != Directions.Up && dir != Directions.Down)
                DistanceToBorder *= Math.Sign(x) * (dir == Directions.Left || dir == Directions.Back ? -1 : 1);
        }

        /// <summary>
        /// 判断纵横坐标、转换后的XYZ坐标与角度是否在给定范围内
        /// </summary>
        public void CheckIfWithinLimits()
        {
            if (Radar == null || Radar.Id < 0)
                return;

            //WithinRadarLimits = _dist_long.Between(Radar.RadarxMin, Radar.RadarxMax) && _dist_lat.Between(Radar.RadaryMin, Radar.RadaryMax);
            //WithinClaimerLimits = X.Between(Radar.ClaimerxMin, Radar.ClaimerxMax) && Y.Between(Radar.ClaimeryMin, Radar.ClaimeryMax) && Z.Between(Radar.ClaimerzMin, Radar.ClaimerzMax);
            //WithinAngleLimits = Angle.Between(Radar.AngleMin, Radar.AngleMax);
            bool radar_limited = _dist_long.Between(Radar.RadarxMin, Radar.RadarxMax) && _dist_lat.Between(Radar.RadaryMin, Radar.RadaryMax);
            bool claimer_limited = X.Between(Radar.ClaimerxMin, Radar.ClaimerxMax) && Y.Between(Radar.ClaimeryMin, Radar.ClaimeryMax) && Z.Between(Radar.ClaimerzMin, Radar.ClaimerzMax);
            bool angle_limited = Angle.Between(Radar.AngleMin, Radar.AngleMax);
            if (!Radar.WithinRadarLimit)
                radar_limited = !radar_limited;
            if (!Radar.WithinClaimerLimit)
                claimer_limited = !claimer_limited;
            if (!Radar.WithinAngleLimit)
                angle_limited = !angle_limited;
            WithinRadarLimits = radar_limited;
            WithinClaimerLimits = claimer_limited;
            WithinAngleLimits = angle_limited;
        }

        ///// <summary>
        ///// 获取定制信息
        ///// </summary>
        ///// <returns></returns>
        //public string GetCustomInfo()
        //{
        //    return !BaseConst.AddingCustomInfo ? string.Empty : string.Format(" {0} {1} {2} {3} {4}", Id, VrelLong, VrelLat, (byte)DynProp, RCS);
        //}
    }

    /// <summary>
    /// 修改后坐标
    /// </summary>
    public class ModifiedCoordinates
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public double X;

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y;

        /// <summary>
        /// Z坐标
        /// </summary>
        public double Z;
    }
}
