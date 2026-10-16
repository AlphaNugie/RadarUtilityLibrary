using System.Collections.Generic;

namespace ArsLibrary.Cluster
{
    /// <summary>
    /// 聚类结果提供器接口，使绘制侧能够以统一方式读取聚类结果，与雷达的具体数据格式无关
    /// </summary>
    /// <remarks>
    /// DataFrameMessages（ARS408）与DataFrameMessagesRhd（RHD）的公共基类DataFrameMessagesBase的泛型参数不同，
    /// 不存在统一的非泛型父类，故以接口方式解耦；所需成员在基类中均已以公共属性存在，实现本接口无需新增成员
    /// </remarks>
    public interface IClusterResultSource
    {
        /// <summary>
        /// 是否启用聚类，为全局开关ClusterConst.Enabled与雷达自身开关同时为真时才为真
        /// </summary>
        bool ClusterEnabled { get; }

        /// <summary>
        /// 最近一次聚类得到的簇列表
        /// </summary>
        List<RadarCluster> Clusters { get; }

        /// <summary>
        /// 最危险（报警距离L最小）的有效簇，无有效目标时为null
        /// </summary>
        RadarCluster BestCluster { get; }

        /// <summary>
        /// 最近一次参与聚类的点，包含噪声点（其ClusterId为-1）
        /// </summary>
        List<RadarPoint> ClusterPoints { get; }

        /// <summary>
        /// 最近一次聚类结果是否因跳变防抖被抑制而未输出
        /// </summary>
        bool ClusterDistanceSuppressed { get; }
    }
}
