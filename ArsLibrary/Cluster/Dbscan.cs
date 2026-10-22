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
        /// <param name="epsMax">eps的上限（与eps同量纲，物理量纲模式下即为米），0表示不限制</param>
        /// <returns>eps</returns>
        public static double FindKneeEps(double[][] feats, int k, double epsMax = 0)
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
            double eps;
            if (n < 5) //点数过少时回退均值
                eps = mean;
            else
            {
                double[] sorted = new double[n];
                Array.Copy(knn, sorted, n);
                Array.Sort(sorted);
                double min = sorted[0], max = sorted[n - 1];
                if (max - min < 1e-9) //极差过小时返回最大值
                    eps = max;
                else
                {
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

                    eps = sorted[kneeIdx];
                    if (eps <= min || eps >= max) //兜底回退均值
                        eps = mean;
                }
            }

            //上限截断：自适应半径过大时会把距离较远的点并进同一簇
            if (epsMax > 0 && eps > epsMax)
                eps = epsMax;
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

        /// <summary>
        /// 按最小生成树的间隙把被桥接的簇拆分成若干子簇
        /// 原理：两个各自密实的目标被合并时，最小生成树的最长边正是横跨两者间隙的"桥"，
        /// 而单个大而密实目标的生成树各边长度接近，不会被拆开；
        /// 断边阈值同时取绝对下限与相对系数中的较大者，避免小尺度稀疏被误拆
        /// </summary>
        /// <param name="feats">特征矩阵（建议使用物理量纲，使阈值具有米制含义）</param>
        /// <param name="indices">待拆分簇中各点在feats中的下标</param>
        /// <param name="gapMin">断边阈值的绝对下限（米）</param>
        /// <param name="gapRatio">断边阈值的相对系数（阈值 = gapRatio × 生成树边长中位数）</param>
        /// <returns>拆分后的子簇（每项为点在feats中的下标集合），无法拆分时返回原簇一项</returns>
        public static List<List<int>> SplitClusterByGap(double[][] feats, List<int> indices, double gapMin, double gapRatio)
        {
            List<List<int>> result = new List<List<int>>();
            if (indices == null || indices.Count == 0)
                return result;
            if (indices.Count == 1 || feats == null)
            {
                result.Add(new List<int>(indices));
                return result;
            }

            int n = indices.Count;

            //Prim算法求最小生成树，记录每条边的长度与两端
            List<double> edgeLen = new List<double>();
            List<int> edgeU = new List<int>();
            List<int> edgeV = new List<int>();
            bool[] inTree = new bool[n];
            double[] best = new double[n];
            int[] from = new int[n];
            for (int i = 0; i < n; i++)
            {
                inTree[i] = false;
                best[i] = double.MaxValue;
                from[i] = -1;
            }
            best[0] = 0;
            for (int step = 0; step < n; step++)
            {
                int u = -1;
                double bv = double.MaxValue;
                for (int i = 0; i < n; i++)
                {
                    if (!inTree[i] && best[i] < bv)
                    {
                        u = i;
                        bv = best[i];
                    }
                }
                if (u < 0)
                    break;
                inTree[u] = true;
                if (from[u] >= 0)
                {
                    edgeLen.Add(bv);
                    edgeU.Add(from[u]);
                    edgeV.Add(u);
                }
                for (int v = 0; v < n; v++)
                {
                    if (inTree[v])
                        continue;
                    double d = Distance(feats[indices[u]], feats[indices[v]]);
                    if (d < best[v])
                    {
                        best[v] = d;
                        from[v] = u;
                    }
                }
            }

            if (edgeLen.Count == 0)
            {
                result.Add(new List<int>(indices));
                return result;
            }

            List<double> sorted = new List<double>(edgeLen);
            sorted.Sort();
            double median = sorted[sorted.Count / 2];
            double threshold = Math.Max(gapMin, gapRatio * median);

            //只保留长度不超过阈值的边做单链合并，跨越间隙的长边相当于被剪断
            int[] parent = new int[n];
            for (int i = 0; i < n; i++)
                parent[i] = i;
            for (int e = 0; e < edgeLen.Count; e++)
            {
                if (edgeLen[e] > threshold)
                    continue;
                Union(parent, edgeU[e], edgeV[e]);
            }

            Dictionary<int, List<int>> groups = new Dictionary<int, List<int>>();
            for (int i = 0; i < n; i++)
            {
                int root = FindRoot(parent, i);
                if (!groups.ContainsKey(root))
                    groups[root] = new List<int>();
                groups[root].Add(indices[i]);
            }
            foreach (List<int> part in groups.Values)
                result.Add(part);
            return result;
        }

        #region 私有方法

        /// <summary>
        /// 查找并查集的根节点（带路径压缩）
        /// </summary>
        private static int FindRoot(int[] parent, int i)
        {
            while (parent[i] != i)
            {
                parent[i] = parent[parent[i]];
                i = parent[i];
            }
            return i;
        }

        /// <summary>
        /// 合并并查集中的两个集合
        /// </summary>
        private static void Union(int[] parent, int a, int b)
        {
            int ra = FindRoot(parent, a);
            int rb = FindRoot(parent, b);
            if (ra != rb)
                parent[rb] = ra;
        }

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
