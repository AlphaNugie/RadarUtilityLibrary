using System;
using System.Collections.Generic;

namespace ArsLibrary.Cluster
{
    /// <summary>
    /// 聚类用中性雷达点实体（与雷达型号、数据格式无关的中间载体）
    /// 字段与原Python防碰实现的(N,7)数组列一一对应，另加聚类与跟踪标签
    /// </summary>
    /// <remarks>
    /// 列号映射表（供配置项 ClusterCols 使用，与原实现的列号语义一致）：
    /// 0-X 1-Y 2-Z 3-Vx 4-Vy 5-Vz 6-Rcs
    /// 各数据格式的填充方式（注意两者坐标系不同源，不可混用）：
    /// ARS408（SensorGeneral）：X=DistLong, Y=DistLat, Z=0, Vx=VrelLong, Vy=VrelLat, Vz=0, Rcs=RCS
    ///     —— ARS408的X/Y/Z为单机坐标系坐标，因此聚类使用雷达原始纵横坐标（DistLong/DistLat），Z恒为0
    /// RHD（ArsLibrary.Model.Rhd.Data）：X=X, Y=Y, Z=Z, Vx=Vx, Vy=Vy, Vz=Vz, Rcs=Rcs
    /// </remarks>
    public class RadarPoint
    {
        /// <summary>
        /// 纵向（x）坐标，米
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 横向（y）坐标，米
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 高度（z）坐标，米（ARS408侧恒为0）
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// 纵向相对速度，米/秒
        /// </summary>
        public double Vx { get; set; }

        /// <summary>
        /// 横向相对速度，米/秒
        /// </summary>
        public double Vy { get; set; }

        /// <summary>
        /// 垂向相对速度，米/秒（ARS408侧恒为0）
        /// </summary>
        public double Vz { get; set; }

        /// <summary>
        /// 反射强度，分贝
        /// </summary>
        public double Rcs { get; set; }

        /// <summary>
        /// 聚类标签，-1表示噪声
        /// </summary>
        public int ClusterId { get; set; } = -1;

        /// <summary>
        /// 跨帧跟踪ID，-1表示未分配到轨迹
        /// </summary>
        public int TrackId { get; set; } = -1;

        /// <summary>
        /// 对应原始点对象在传入快照中的索引（用于回查原始点）
        /// </summary>
        public int SourceIndex { get; set; } = -1;

        /// <summary>
        /// 该点距检测区边界的距离（DistanceToBorder），用于计算簇的报警距离L
        /// </summary>
        public double DistanceToBorder { get; set; }

        /// <summary>
        /// 该点距雷达原点的距离
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// 根据列号取特征值（对应配置项ClusterCols）
        /// </summary>
        /// <param name="col">列号：0-X 1-Y 2-Z 3-Vx 4-Vy 5-Vz 6-Rcs</param>
        /// <returns>特征值</returns>
        public double GetFeature(int col)
        {
            switch (col)
            {
                case 0: return X;
                case 1: return Y;
                case 2: return Z;
                case 3: return Vx;
                case 4: return Vy;
                case 5: return Vz;
                case 6: return Rcs;
                default: return 0;
            }
        }
    }
}
