using System;
using System.Collections.Generic;
using System.Linq;

namespace ArsLibrary.Cluster
{
    /// <summary>
    /// 平面几何工具类，提供聚类相关的通用平面几何算法
    /// </summary>
    /// <remarks>
    /// 泛型重载通过坐标取值委托获取坐标，可作用于任意引用类型的点集合；
    /// 针对RadarPoint另提供强类型重载，其第一坐标为X（纵向）、第二坐标为Y（横向）
    /// </remarks>
    public static class Geometry2D
    {
        /// <summary>
        /// 计算RadarPoint点集的凸包，以X（纵向）为第一坐标、Y（横向）为第二坐标
        /// </summary>
        /// <param name="points">点集</param>
        /// <returns>凸包顶点列表；点集为空时返回空列表，点数不足3个或全部共线时返回不足3个点的列表，调用方应回退为包围盒</returns>
        public static List<RadarPoint> ConvexHull(IEnumerable<RadarPoint> points)
        {
            return ConvexHull(points, p => p.X, p => p.Y);
        }

        /// <summary>
        /// 计算给定点集的凸包（Andrew monotone chain算法，时间复杂度O(n log n)）
        /// </summary>
        /// <typeparam name="T">点元素类型（引用类型）</typeparam>
        /// <param name="points">点集</param>
        /// <param name="getX">从点元素中取第一坐标的函数</param>
        /// <param name="getY">从点元素中取第二坐标的函数</param>
        /// <returns>凸包顶点列表；点集为空或取坐标函数为空时返回空列表，点数不足3个或全部共线时返回不足3个点的列表，调用方应回退为包围盒</returns>
        public static List<T> ConvexHull<T>(IEnumerable<T> points, Func<T, double> getX, Func<T, double> getY) where T : class
        {
            if (points == null || getX == null || getY == null)
                return new List<T>();

            List<T> sorted = points.Where(p => p != null).OrderBy(getX).ThenBy(getY).ToList();
            if (sorted.Count < 3)
                return sorted;

            //下凸包
            List<T> lower = new List<T>();
            foreach (T point in sorted)
            {
                while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], point, getX, getY) <= 0)
                    lower.RemoveAt(lower.Count - 1);
                lower.Add(point);
            }

            //上凸包
            List<T> upper = new List<T>();
            for (int i = sorted.Count - 1; i >= 0; i--)
            {
                T point = sorted[i];
                while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], point, getX, getY) <= 0)
                    upper.RemoveAt(upper.Count - 1);
                upper.Add(point);
            }

            //去掉首尾重复点后合并
            lower.RemoveAt(lower.Count - 1);
            upper.RemoveAt(upper.Count - 1);
            lower.AddRange(upper);
            return lower;
        }

        /// <summary>
        /// 计算向量AB与BC的叉积，大于0表示左转
        /// </summary>
        /// <typeparam name="T">点元素类型（引用类型）</typeparam>
        /// <param name="a">起点</param>
        /// <param name="b">中间点</param>
        /// <param name="c">终点</param>
        /// <param name="getX">从点元素中取第一坐标的函数</param>
        /// <param name="getY">从点元素中取第二坐标的函数</param>
        /// <returns>叉积</returns>
        private static double Cross<T>(T a, T b, T c, Func<T, double> getX, Func<T, double> getY) where T : class
        {
            return (getX(b) - getX(a)) * (getY(c) - getY(b)) - (getY(b) - getY(a)) * (getX(c) - getX(b));
        }
    }
}
