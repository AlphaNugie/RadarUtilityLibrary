namespace ArsLibrary.Cluster
{
    /// <summary>
    /// 聚类结果包围框的形状
    /// </summary>
    public enum ClusterBoxShape
    {
        /// <summary>
        /// 长方形（轴对齐包围盒）
        /// </summary>
        Rectangle = 0,

        /// <summary>
        /// 不规则凸包多边形
        /// </summary>
        ConvexHull = 1
    }
}
