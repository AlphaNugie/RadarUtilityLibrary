using System;
using System.Collections.Generic;
using System.Linq;

namespace ArsLibrary.Cluster
{
    /// <summary>
    /// 聚类后的单个簇（障碍物）信息实体
    /// 坐标系与RadarPoint一致：ARS408侧为雷达原始纵横坐标，RHD侧为直角坐标
    /// </summary>
    public class RadarCluster
    {
        /// <summary>
        /// DBSCAN原始簇标签
        /// </summary>
        public int Label { get; set; }

        /// <summary>
        /// 跨帧跟踪ID，-1表示未分配到轨迹
        /// </summary>
        public int TrackId { get; set; } = -1;

        /// <summary>
        /// 簇内点的数量
        /// </summary>
        public int PointCount { get; set; }

        #region 包围盒

        /// <summary>
        /// 纵向（x）最小值
        /// </summary>
        public double XMin { get; set; }

        /// <summary>
        /// 纵向（x）最大值
        /// </summary>
        public double XMax { get; set; }

        /// <summary>
        /// 横向（y）最小值
        /// </summary>
        public double YMin { get; set; }

        /// <summary>
        /// 横向（y）最大值
        /// </summary>
        public double YMax { get; set; }

        /// <summary>
        /// 高度（z）最小值
        /// </summary>
        public double ZMin { get; set; }

        /// <summary>
        /// 高度（z）最大值
        /// </summary>
        public double ZMax { get; set; }

        /// <summary>
        /// 高度中心（z平均值），用于障碍物高度筛选（ARS408侧恒为0）
        /// </summary>
        public double HeartZ { get; set; }

        #endregion

        #region 距离与尺寸

        /// <summary>
        /// 报警距离L：簇内各点距检测区边界（DistanceToBorder）距离的最小值，已减去距离偏置LOffset
        /// </summary>
        public double L { get; set; }

        /// <summary>
        /// 簇内各点距检测区边界距离的平均值（备用）
        /// </summary>
        public double AverageDistanceToBorder { get; set; }

        /// <summary>
        /// 簇内各点距雷达原点距离的平均值（备用）
        /// </summary>
        public double AverageRadius { get; set; }

        /// <summary>
        /// 宽度W：簇的纵向中心 |(XMin + XMax) / 2|，对应原实现的臂架长度方向侧方距离
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// 高度H：簇的高度最大值ZMax（ARS408侧恒为0）
        /// </summary>
        public double H { get; set; }

        #endregion

        /// <summary>
        /// 簇内最小反射强度
        /// </summary>
        public double RcsMin { get; set; }

        /// <summary>
        /// 簇内平均反射强度
        /// </summary>
        public double RcsAverage { get; set; }

        /// <summary>
        /// 是否为有效簇（点数满足下限且高度中心筛选通过）
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 簇内的点
        /// </summary>
        public List<RadarPoint> Points { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public RadarCluster()
        {
            Points = new List<RadarPoint>();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="label">簇标签</param>
        /// <param name="points">簇内的点</param>
        public RadarCluster(int label, List<RadarPoint> points) : this()
        {
            Label = label;
            if (points == null || points.Count == 0)
                return;

            Points = points;
            PointCount = points.Count;
            XMin = points.Min(p => p.X);
            XMax = points.Max(p => p.X);
            YMin = points.Min(p => p.Y);
            YMax = points.Max(p => p.Y);
            ZMin = points.Min(p => p.Z);
            ZMax = points.Max(p => p.Z);
            HeartZ = points.Average(p => p.Z);
            AverageDistanceToBorder = points.Average(p => p.DistanceToBorder);
            AverageRadius = points.Average(p => p.Radius);
            RcsMin = points.Min(p => p.Rcs);
            RcsAverage = points.Average(p => p.Rcs);
            W = Math.Abs((XMin + XMax) / 2);
            H = ZMax;
        }

        /// <summary>
        /// 计算报警距离L并判定有效性
        /// </summary>
        /// <param name="lOffset">距离标定偏置</param>
        /// <param name="minPoints">有效簇的最小点数</param>
        /// <param name="heartZFilterEnabled">是否启用高度中心筛选</param>
        /// <param name="heartZLow">高度中心下限</param>
        /// <param name="heartZHigh">高度中心上限</param>
        public void Evaluate(double lOffset, int minPoints, bool heartZFilterEnabled, double heartZLow, double heartZHigh)
        {
            L = Points.Count > 0 ? Points.Min(p => p.DistanceToBorder) - lOffset : 0;
            bool enoughPoints = PointCount >= minPoints;
            bool heightOk = !heartZFilterEnabled || (HeartZ >= heartZLow && HeartZ <= heartZHigh);
            IsValid = enoughPoints && heightOk;
        }

        /// <summary>
        /// 获取簇信息描述
        /// </summary>
        /// <returns>描述字符串</returns>
        public override string ToString()
        {
            return string.Format("簇{0}(跟踪{1}) 点数{2} L={3:F2} W={4:F2} H={5:F2} Rcs={6:F1} 有效{7}",
                Label, TrackId, PointCount, L, W, H, RcsMin, IsValid ? "是" : "否");
        }
    }

    /// <summary>
    /// 聚类包围盒（用于本体/臂架盒的保存与剔除判定）
    /// </summary>
    public class ClusterBox
    {
        /// <summary>
        /// 纵向（x）最小值
        /// </summary>
        public double XMin { get; set; }

        /// <summary>
        /// 纵向（x）最大值
        /// </summary>
        public double XMax { get; set; }

        /// <summary>
        /// 横向（y）最小值
        /// </summary>
        public double YMin { get; set; }

        /// <summary>
        /// 横向（y）最大值
        /// </summary>
        public double YMax { get; set; }

        /// <summary>
        /// 高度（z）最小值
        /// </summary>
        public double ZMin { get; set; }

        /// <summary>
        /// 高度（z）最大值
        /// </summary>
        public double ZMax { get; set; }

        /// <summary>
        /// 判断给定点是否落在盒内
        /// </summary>
        /// <param name="point">给定点</param>
        /// <returns>是否落在盒内</returns>
        public bool Contains(RadarPoint point)
        {
            if (point == null)
                return false;
            return point.X >= XMin && point.X <= XMax && point.Y >= YMin && point.Y <= YMax && point.Z >= ZMin && point.Z <= ZMax;
        }
    }
}
