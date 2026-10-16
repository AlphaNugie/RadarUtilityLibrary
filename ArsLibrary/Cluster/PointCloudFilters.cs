using System;
using System.Collections.Generic;
using System.Linq;

namespace ArsLibrary.Cluster
{
    /// <summary>
    /// 点云滤波与特征预处理（对应原Python实现的统计离群点去除、聚类特征归一化与加权）
    /// </summary>
    public static class PointCloudFilters
    {
        /// <summary>
        /// 统计离群点去除（SOR，对应原实现的statistical_outlier_removal）
        /// 仅使用X/Y/Z三轴坐标计算k近邻平均距离，距离超过 均值+nSigma*标准差 的点判为离群点
        /// </summary>
        /// <param name="points">待滤波的点集</param>
        /// <param name="k">近邻数量</param>
        /// <param name="nSigma">标准差倍数</param>
        /// <returns>保留下来的点（新集合，不修改原集合）</returns>
        public static List<RadarPoint> StatisticalOutlierRemoval(List<RadarPoint> points, int k, double nSigma)
        {
            if (points == null || points.Count == 0)
                return new List<RadarPoint>();

            int n = points.Count;
            if (n < k + 1) //点数过少时不做滤波
                return new List<RadarPoint>(points);

            //计算每个点的平均k近邻距离
            double[] meanDist = new double[n];
            for (int i = 0; i < n; i++)
            {
                List<double> dists = new List<double>(n - 1);
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        continue;
                    double dx = points[i].X - points[j].X;
                    double dy = points[i].Y - points[j].Y;
                    double dz = points[i].Z - points[j].Z;
                    dists.Add(Math.Sqrt(dx * dx + dy * dy + dz * dz));
                }
                dists.Sort();
                int kUse = Math.Min(k, dists.Count);
                double sum = 0;
                for (int m = 0; m < kUse; m++)
                    sum += dists[m];
                meanDist[i] = sum / kUse;
            }

            double mu = meanDist.Average();
            //总体标准差（除以N，对应sklearn.StandardScaler的ddof=0）
            double sigma = Math.Sqrt(meanDist.Average(d => (d - mu) * (d - mu)));
            if (sigma <= 0)
                return new List<RadarPoint>(points);

            double threshold = mu + nSigma * sigma;
            List<RadarPoint> result = new List<RadarPoint>();
            for (int i = 0; i < n; i++)
                if (meanDist[i] <= threshold)
                    result.Add(points[i]);
            return result;
        }

        /// <summary>
        /// 聚类特征的Z-score归一化与加权（对应原实现的StandardScaler+feature_weights）
        /// </summary>
        /// <param name="points">点集</param>
        /// <param name="cols">特征列号（0-X 1-Y 2-Z 3-Vx 4-Vy 5-Vz 6-Rcs）</param>
        /// <param name="weights">与特征列一一对应的权重</param>
        /// <returns>归一化并加权后的特征矩阵，行对应点、列对应特征</returns>
        public static double[][] NormalizeAndWeight(List<RadarPoint> points, int[] cols, double[] weights)
        {
            if (points == null || points.Count == 0 || cols == null || cols.Length == 0)
                return new double[0][];

            int n = points.Count, m = cols.Length;
            double[][] result = new double[n][];
            for (int i = 0; i < n; i++)
                result[i] = new double[m];

            for (int c = 0; c < m; c++)
            {
                int col = cols[c];
                double mean = 0, std = 0;
                for (int i = 0; i < n; i++)
                    mean += points[i].GetFeature(col);
                mean /= n;
                for (int i = 0; i < n; i++)
                {
                    double diff = points[i].GetFeature(col) - mean;
                    std += diff * diff;
                }
                std = Math.Sqrt(std / n); //总体标准差
                double weight = weights != null && c < weights.Length ? weights[c] : 1;
                for (int i = 0; i < n; i++)
                    result[i][c] = std <= 0 ? 0 : (points[i].GetFeature(col) - mean) / std * weight;
            }

            return result;
        }
    }
}
