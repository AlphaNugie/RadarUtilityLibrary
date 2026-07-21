using ArsLibrary.Core;
using CommonLib.Clients;
using CommonLib.Extensions;
using CommonLib.Function.Fitting;
using CommonLib.Function.MathUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model.Rhd
{
    /// <summary>
    /// 表示单个Cluster/Object目标的结构化数据
    /// </summary>
    public class Data
    {
        #region static
        /// <summary>
        /// 根据距检测区的最短距离排序
        /// </summary>
        //public static Comparison<Data> DistanceComparison = (a, b) => a.DistanceToBorder.CompareTo(b.DistanceToBorder);
        public static Comparison<Data> DistanceComparison = (a, b) => a.DistanceToBorder.CompareTo(b.DistanceToBorder);

        /// <summary>
        /// 根据转换后Z坐标的大小排序
        /// </summary>
        public static Comparison<Data> HeightComparison = (a, b) => a.Z.CompareTo(b.Z);
        #endregion

        private readonly ColorSmoother _colorSmoother = new ColorSmoother(-15, 15);

        #region 原生属性
        /// <summary>
        /// 目标循环ID (1-65535)
        /// </summary>
        public ushort Id { get; internal set; }

        /// <summary>
        /// X轴坐标 (单位：米，分辨率0.05)，正方向为雷达法线方向
        /// </summary>
        public double X { get; internal set; }

        /// <summary>
        /// Y轴坐标 (单位：米，分辨率0.05)，正方向为垂直X轴向左
        /// </summary>
        public double Y { get; internal set; }

        /// <summary>
        /// Z轴坐标 (单位：米，分辨率0.05)，垂直地面向上
        /// </summary>
        public double Z { get; internal set; }

        /// <summary>
        /// X轴速度 (单位：米/秒，分辨率0.02)，远离雷达方向为正
        /// </summary>
        public double Vx { get; internal set; }

        /// <summary>
        /// Y轴速度 (单位：米/秒，分辨率0.02)，向左移动为正
        /// </summary>
        public double Vy { get; internal set; }

        /// <summary>
        /// Z轴速度 (单位：米/秒，分辨率0.02)，向上移动为正
        /// </summary>
        public double Vz { get; internal set; }

        /// <summary>
        /// 雷达散射截面积 (单位：dBm²，Cluster分辨率0.5，Object分辨率0.1)
        /// </summary>
        public double Rcs { get; internal set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 转换后单机坐标系X轴坐标 (单位：米)，正方向为臂架/溜筒右侧
        /// </summary>
        public double CX { get; internal set; }

        /// <summary>
        /// 转换后单机坐标系Y轴坐标 (单位：米)，正方向为臂架/溜筒前方
        /// </summary>
        public double CY { get; internal set; }

        /// <summary>
        /// 转换后单机坐标系Z轴坐标 (单位：米)，竖直向上
        /// </summary>
        public double CZ { get; internal set; }

        ///// <summary>
        ///// 距离雷达的直线距离
        ///// </summary>
        //public double Distance { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }

        /// <summary>
        /// 距检测边界的距离，与检测方式（点线面）与雷达朝向（海北陆南）有关
        /// </summary>
        public double DistanceToBorder { get; internal set; }

        /// <summary>
        /// 角度（根据纵向与横向坐标的方位角）
        /// </summary>
        public double Angle { get; internal set; }

        /// <summary>
        /// 当前点的颜色（依据Z轴值确定）
        /// </summary>
        public Color Color { get { return GetColor(); } }

        /// <summary>
        /// 所在帧累积未清除的结算(push finalization)次数
        /// </summary>
        public int PushfCounter { get; internal set; }
        #endregion

        /// <summary>
        /// 更新单机坐标系的坐标值（然后计算单机距离）
        /// </summary>
        /// <param name="cx">单机X坐标</param>
        /// <param name="cy">单机Y坐标</param>
        /// <param name="cz">单机Z坐标</param>
        /// <param name="defense">防御模式</param>
        /// <param name="dir">方向</param>
        public void UpdateCoordinates(double cx, double cy, double cz, DefenseMode defense = DefenseMode.Vertex, Directions dir = Directions.Front)
        {
            CX = cx;
            CY = cy;
            CZ = cz;
            DistanceToBorder = GetDistanceToBorder(cx, cy, cz, defense, dir);
        }

        /// <summary>
        /// 更新帧累积未清除的结算(push finalization)次数
        /// </summary>
        /// <param name="pushfCounter"></param>
        public void UpdatePushfCounter(int pushfCounter)
        {
            PushfCounter = pushfCounter;
        }

        /// <summary>
        /// 根据单机坐标系坐标与防御方式和方向，计算转换后坐标
        /// </summary>
        /// <param name="cx">单机X坐标</param>
        /// <param name="cy">单机Y坐标</param>
        /// <param name="cz">单机Z坐标</param>
        /// <param name="defense">防御模式</param>
        /// <param name="dir">方向</param>
        public static double GetDistanceToBorder(double cx, double cy, double cz, DefenseMode defense = DefenseMode.Vertex, Directions dir = Directions.Front)
        {
            // 是否朝向左或右
            bool leftright = dir == Directions.Left || dir == Directions.Right;
            // 根据方向调换cx/cy的值
            double x = leftright ? cx : cy, y = leftright ? cy : cx, z = cz;
            //防御模式：1 点，2 线，3 面
            //d = (a*x^2+b*z^2+c*y^2)^0.5，其中a, b, c由4-m, 3-m, 2-m的值决定，假如大于0则为1，小于等于0为0（公式形如Math.Sign(4 - m) == 1 ? 1 : 0）
            //含义：面模式，a=1,b=c=0；线模式，a=b=1,c=0；点模式，a=b=c=1
            //假如方向为上下（且为面防御模式），则只计算竖直方向cz坐标的值
            int m = (int)defense;
            double dist2Border = (dir == Directions.Up || dir == Directions.Down) && m == 3 ? z : Math.Sqrt((Math.Sign(4 - m) == 1 ? 1 : 0) * Math.Pow(x, 2) + (Math.Sign(3 - m) == 1 ? 1 : 0) * Math.Pow(z, 2) + (Math.Sign(2 - m) == 1 ? 1 : 0) * Math.Pow(y, 2));
            //当方向向下（且为面防御模式）时，在距离前乘以一个值为-1的系数（向下指时cz坐标均为负数）
            //雷达距离校正Offset已在Radar.CurrentDistance的输出侧处理，故不用再次计算
            dist2Border = (dir == Directions.Down && m == 3 ? -1 : 1) * dist2Border/* + Radar.Offset*/;
            //假如防御模式为面，再添加处理步骤：乘以x的符号，效果为使边界距离带符号；假如面向北或陆，则再乘以-1（所面向方向坐标均为负数）
            if (m == 3 && dir != Directions.Up && dir != Directions.Down)
                dist2Border *= Math.Sign(x) * (dir == Directions.Left || dir == Directions.Back ? -1 : 1);
            return dist2Border;
        }

        /// <summary>
        /// 获取当前点的颜色（依据Z轴值确定）
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            return _colorSmoother.GetColor(Z);
        }

        /// <summary>
        /// 转换为三维空间坐标点对象
        /// </summary>
        /// <returns></returns>
        public Point3D ToPoint3D()
        {
            return new Point3D(CX, CY, CZ, Rcs, X, Y, Z);
        }

        /// <summary>
        /// 根据横纵坐标计算角度
        /// </summary>
        /// <param name="x">X轴坐标(单位：米)</param>
        /// <param name="y">Y轴坐标(单位：米，分辨率0.05)</param>
        /// <returns></returns>
        public static double GetAngle(double x, double y)
        {
            return x == 0 ? Math.Sign(y) * 90 : Math.Atan(y / x) * 180 / Math.PI;
            //Angle = X == 0 ? Math.Sign(Y) * 90 : Math.Atan(Y / X) * 180 / Math.PI;
            ////Radius = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        /// <summary>
        /// 此点在雷达坐标系下的坐标是否在范围内
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMin"></param>
        /// <param name="yMax"></param>
        /// <param name="zMin"></param>
        /// <param name="zMax"></param>
        /// <returns></returns>
        public bool IsWithinRadarLimits(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax)
        {
            return X.Between(xMin, xMax) && Y.Between(yMin, yMax) && Z.Between(zMin, zMax);
        }

        /// <summary>
        /// 此点在单机坐标系下的坐标是否在范围内
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMin"></param>
        /// <param name="yMax"></param>
        /// <param name="zMin"></param>
        /// <param name="zMax"></param>
        /// <returns></returns>
        public bool IsWithinClaimerLimits(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax)
        {
            return CX.Between(xMin, xMax) && CY.Between(yMin, yMax) && CZ.Between(zMin, zMax);
        }

        /// <summary>
        /// 此点的角度是否在范围内
        /// </summary>
        /// <param name="angleMin"></param>
        /// <param name="angleMax"></param>
        /// <returns></returns>
        public bool IsWithinAngleLimits(double angleMin, double angleMax)
        {
            return Angle.Between(angleMin, angleMax);
        }
    }
}
