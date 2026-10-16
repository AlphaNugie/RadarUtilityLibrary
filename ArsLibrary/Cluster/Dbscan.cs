using System;
using System.Collections.Generic;
using System.Linq;

namespace ArsLibrary.Cluster
{
    /// <summary>
    /// DBSCAN聚类器（对应原Python实现的find_knee_eps + DBSCAN + 聚类后过滤）
    /// 输入为归一化并加权后的特征矩阵，输出为每个点的簇标签（-1表示噪声）
    /// </summary>
    public static class Dbscan
    {
        /// <summary>
        /// 噪声标签
        /// </summary>
        public const int NOISE = -1;

        /// <summary>
        /// 未分类标记（内部使用）
        /// </summary>
        private const int UNCLASSIFIED = -2;

        /// <summary>
        /// 自适应计算DBSCAN的eps（拐点法，对应原实现的find_knee_eps）
        /// 计算k近邻距离曲线，取二阶差分最大点（膝盖点）作为eps
        /// </summary>
        /// <param name="feats">归一化后的特征矩阵</param>
        /// <param name="k">近邻数量</param>
        /// <returns>eps</returns>
        public static double FindKneeEps(double[][] feats, int k)
        {
            //点数不足2个时不存在"近邻"（dists为空集合），直接返回0，避免dists[kUse-1]越界
            if (feats == null || feats.Length < 2)
                return 0;

            int n = feats.Length;
            int kUse = Math.Min(Math.Max(k, 1), Math.Max(n - 1, 1));

            //每个点到其第k近邻的距离
            double[] knn = new double[n];
            for (int i = 0; i < n; i++)
            {
                List<double> dists = new List<double>(n - 1);
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        continue;
                    dists.Add(Distance(feats[i], feats[j]));
                }
                dists.Sort();
                knn[i] = dists[kUse - 1];
            }

            double mean = knn.Average();
            if (n < 5) //点数过少时回退均值
                return mean;

            double[] sorted = new double[n];
            Array.Copy(knn, sorted, n);
            Array.Sort(sorted);
            double min = sorted[0], max = sorted[n - 1];
            if (max - min < 1e-9) //极差过小时返回最大值
                return max;

            //归一化到[0,1]后取二阶差分最大点（+1为对齐回原数组索引的修正）
            double[] norm = new double[n];
            for (int i = 0; i < n; i++)
                norm[i] = (sorted[i] - min) / (max - min);

            int kneeIdx = 0;
            double maxDiff = double.MinValue;
            for (int i = 0; i + 2 < n; i++)
            {
                double diff = norm[i + 2] - 2 * norm[i + 1] + norm[i];
                if (diff > maxDiff)
                {
                    maxDiff = diff;
                    kneeIdx = i + 1;
                }
            }

            double eps = sorted[kneeIdx];
            if (eps <= min || eps >= max) //兜底回退均值
                eps = mean;
            return eps;
        }

        /// <summary>
        /// DBSCAN聚类
        /// </summary>
        /// <param name="feats">归一化后的特征矩阵</param>
        /// <param name="eps">邻域半径</param>
        /// <param name="minSamples">核心点的最小邻域点数（含自身）</param>
        /// <returns>每个点的簇标签，-1表示噪声</returns>
        public static int[] Cluster(double[][] feats, double eps, int minSamples)
        {
            int n = feats == null ? 0 : feats.Length;
            int[] labels = new int[n];
            for (int i = 0; i < n; i++)
                labels[i] = UNCLASSIFIED;
            if (n == 0)
                return labels;

            double[][] dist = BuildDistanceMatrix(feats);
            int clusterId = 0;
            for (int i = 0; i < n; i++)
            {
                if (labels[i] != UNCLASSIFIED)
                    continue;

                List<int> neighbors = RegionQuery(dist, i, eps);
                if (neighbors.Count < minSamples) //非核心点，暂标噪声
                {
                    labels[i] = NOISE;
                    continue;
                }

                labels[i] = clusterId;
                Queue<int> seeds = new Queue<int>();
                foreach (int nb in neighbors)
                    if (nb != i)
                        seeds.Enqueue(nb);

                while (seeds.Count > 0)
                {
                    int j = seeds.Dequeue();
                    if (labels[j] == NOISE) //边界点归入当前簇
                        labels[j] = clusterId;
                    if (labels[j] != UNCLASSIFIED)
                        continue;

                    labels[j] = clusterId;
                    List<int> jNeighbors = RegionQuery(dist, j, eps);
                    if (jNeighbors.Count >= minSamples) //核心点继续扩展
                        foreach (int nb in jNeighbors)
                            if (labels[nb] == UNCLASSIFIED)
                                seeds.Enqueue(nb);
                }

                clusterId++;
            }

            return labels;
        }

        /// <summary>
        /// 稀疏簇过滤（对应原实现的filter_sparse_clusters）
        /// 簇内平均最近邻距离大于 ratio*eps 的整簇判为噪声
        /// </summary>
        /// <param name="feats">归一化后的特征矩阵</param>
        /// <param name="labels">簇标签（原地修改）</param>
        /// <param name="eps">邻域半径</param>
        /// <param name="ratio">稀疏判定系数</param>
        public static void FilterSparseClusters(double[][] feats, int[] labels, double eps, double ratio)
        {
            if (feats == null || labels == null || feats.Length != labels.Length)
                return;

            foreach (int label in labels.Distinct().Where(l => l != NOISE).ToList())
            {
                List<int> indices = GetIndicesOfLabel(labels, label);
                if (indices.Count < 2)
                    continue;

                double sum = 0;
                foreach (int i in indices)
                    sum += NearestDistance(feats, indices, i);
                double mean = sum / indices.Count;
                if (mean > ratio * eps)
                    foreach (int i in indices)
                        labels[i] = NOISE;
            }
        }

        /// <summary>
        /// 簇内离群点剔除（对应原实现的filter_intra_cluster_outliers）
        /// 簇内最近邻距离大于 ratio*eps 的点判回噪声
        /// </summary>
        /// <param name="feats">归一化后的特征矩阵</param>
        /// <param name="labels">簇标签（原地修改）</param>
        /// <param name="eps">邻域半径</param>
        /// <param name="ratio">离群判定系数</param>
        public static void FilterIntraClusterOutliers(double[][] feats, int[] labels, double eps, double ratio)
        {
            if (feats == null || labels == null || feats.Length != labels.Length)
                return;

            foreach (int label in labels.Distinct().Where(l => l != NOISE).ToList())
            {
                List<int> indices = GetIndicesOfLabel(labels, label);
                if (indices.Count < 2)
                    continue;

                foreach (int i in indices)
                    if (NearestDistance(feats, indices, i) > ratio * eps)
                        labels[i] = NOISE;
            }
        }

        #region 私有方法

        /// <summary>
        /// 计算两个特征向量的欧氏距离
        /// </summary>
        private static double Distance(double[] a, double[] b)
        {
            double sum = 0;
            int len = Math.Min(a.Length, b.Length);
            for (int i = 0; i < len; i++)
            {
                double diff = a[i] - b[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// 构建距离矩阵
        /// </summary>
        private static double[][] BuildDistanceMatrix(double[][] feats)
        {
            int n = feats.Length;
            double[][] dist = new double[n][];
            //必须先把所有行都创建出来再填值：内层循环会写入dist[j][i]（j>i），
            //若写成"写一行建一行"，dist[j]此时仍为null，会抛出NullReferenceException
            for (int i = 0; i < n; i++)
                dist[i] = new double[n];

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    double d = Distance(feats[i], feats[j]);
                    dist[i][j] = d;
                    dist[j][i] = d;
                }
            }
            return dist;
        }

        /// <summary>
        /// 查询邻域内的点索引（含自身）
        /// </summary>
        private static List<int> RegionQuery(double[][] dist, int index, double eps)
        {
            List<int> result = new List<int>();
            double[] row = dist[index];
            for (int i = 0; i < row.Length; i++)
                if (row[i] <= eps)
                    result.Add(i);
            return result;
        }

        /// <summary>
        /// 获取指定标签的所有点索引
        /// </summary>
        private static List<int> GetIndicesOfLabel(int[] labels, int label)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < labels.Length; i++)
                if (labels[i] == label)
                    indices.Add(i);
            return indices;
        }

        /// <summary>
        /// 计算指定点到同簇内最近邻点的距离
        /// </summary>
        private static double NearestDistance(double[][] feats, List<int> indices, int index)
        {
            double min = double.MaxValue;
            foreach (int j in indices)
            {
                if (j == index)
                    continue;
                double d = Distance(feats[index], feats[j]);
                if (d < min)
                    min = d;
            }
            return min == double.MaxValue ? 0 : min;
        }

        #endregion
    }
}
