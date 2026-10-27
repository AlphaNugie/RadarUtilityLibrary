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
        /// <param name="distanceMode">簇距离聚合方式：0=最小值，1=剔除近端离群点后的最小值，2=第k小，3=中位数</param>
        /// <param name="dropRatio">剔除近端离群点的比例（distanceMode=1时生效）</param>
        /// <param name="maxDrop">剔除近端离群点的最大个数（distanceMode=1时生效）</param>
        /// <param name="kth">第k小值方式使用的k（distanceMode=2时生效）</param>
        public void Evaluate(double lOffset, int minPoints, bool heartZFilterEnabled, double heartZLow, double heartZHigh,
            int distanceMode = 0, double dropRatio = 0.15, int maxDrop = 2, int kth = 2)
        {
            L = ComputeClusterDistance(distanceMode, dropRatio, maxDrop, kth) - lOffset;
            bool enoughPoints = PointCount >= minPoints;
            bool heightOk = !heartZFilterEnabled || (HeartZ >= heartZLow && HeartZ <= heartZHigh);
            IsValid = enoughPoints && heightOk;
        }

        /// <summary>
        /// 按指定的聚合方式计算簇的报警距离（未减距离偏置）
        /// </summary>
        /// <remarks>
        /// 簇内各点的DistanceToBorder是"该防御方向上点到检测区边界的距离"，
        /// 因此取最小值得到的是物体最靠近检测边界的那一面，即物体表面距离（防碰要的正是这个量）。
        /// 各种方式的取舍：
        ///   0 最小值：语义最正确，但会被单个突出近点（毛刺、倒角、误检点）拉近，造成误报；
        ///   1 剔除近端离群点后的最小值：保留表面语义，同时对个别突出点免疫，推荐使用；
        ///   2 第k小值：与方式1接近，但k固定、不随点数自适应；
        ///   3 中位数：得到的是簇的中心位置而非物体表面，斜置目标下会明显偏远，破坏防碰语义，仅供对比试验。
        /// </remarks>
        /// <param name="distanceMode">聚合方式</param>
        /// <param name="dropRatio">剔除比例（方式1）</param>
        /// <param name="maxDrop">剔除最大个数（方式1）</param>
        /// <param name="kth">第k小值的k（方式2）</param>
        /// <returns>簇的报警距离，无点时返回0</returns>
        public double ComputeClusterDistance(int distanceMode, double dropRatio, int maxDrop, int kth)
        {
            if (Points == null || Points.Count == 0)
                return 0;

            switch (distanceMode)
            {
                case 1: //剔除近端离群点后的最小值
                    {
                        //剔除个数按点数比例计算并封顶
                        int drop = (int)Math.Round(Points.Count * dropRatio);
                        if (drop > maxDrop)
                            drop = maxDrop;
                        if (drop < 0)
                            drop = 0;
                        //剔除数不得超过"点数减2"，即至少保留2个点参与取最小值：
                        //  保留≥2个点 ⇒ 结果仍是多个点中最小的那个，不会因剔除而翻到"更远的一面"，
                        //  保住物体表面语义；同时2~3个点的小簇不剔除（点数太少无法判断谁是离群点）
                        int maxAllowed = Points.Count - 2;
                        if (maxAllowed < 0)
                            maxAllowed = 0;
                        if (drop > maxAllowed)
                            drop = maxAllowed;
                        if (drop <= 0)
                            return Points.Min(p => p.DistanceToBorder);
                        //只剔近端（最小值端），远端点对最小值本就无害
                        var ordered = Points.Select(p => p.DistanceToBorder).OrderBy(v => v).ToList();
                        return ordered[drop];
                    }
                case 2: //第k小值
                    {
                        int k = kth < 1 ? 1 : kth;
                        if (k > Points.Count)
                            k = Points.Count;
                        var ordered = Points.Select(p => p.DistanceToBorder).OrderBy(v => v).ToList();
                        return ordered[k - 1];
                    }
                case 3: //中位数（仅供对比试验）
                    {
                        var ordered = Points.Select(p => p.DistanceToBorder).OrderBy(v => v).ToList();
                        int n = ordered.Count;
                        return n % 2 == 1 ? ordered[n / 2] : (ordered[n / 2 - 1] + ordered[n / 2]) / 2;
                    }
                default: //0 最小值（现状）
                    return Points.Min(p => p.DistanceToBorder);
            }
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
