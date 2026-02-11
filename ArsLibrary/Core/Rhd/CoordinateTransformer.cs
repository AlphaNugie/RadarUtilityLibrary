using ArsLibrary.Model.Rhd;
using CommonLib.Function;
using CommonLib.Function.Fitting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Core.Rhd
{
    /// <summary>
    /// 坐标变换工具类，用于处理雷达的旋转和平移变换（平移即坐标偏移）
    /// </summary>
    public static class CoordinateTransformer
    {
        private static readonly CoordTransParamSet _defaultParamSet = CoordTransParamSet.CreateDefault();

        ///// <summary>
        ///// 将点云坐标系中的点变换到现实空间坐标系，并更新点云坐标
        ///// </summary>
        ///// <param name="rawPoint">雷达点数据对象，坐标单位为米</param>
        ///// <param name="paramSet">空间旋转位移参数集</param>
        ///// <exception cref="ArgumentNullException"></exception>
        //public static void TransformPoint(ref Data rawPoint, CoordTransParamSet paramSet)
        //{
        //    //if (paramSet == null)
        //    //    throw new ArgumentNullException(nameof(paramSet), "空间旋转位移参数不能为空");
        //    if (paramSet == null) paramSet = _defaultParamSet;
        //    var coord = TransformPoint(rawPoint.X, rawPoint.Y, rawPoint.Z, paramSet);
        //    rawPoint.UpdateCoordinates(coord[0], coord[1], coord[2]);
        //}

        /// <summary>
        /// 将点云坐标系中的Cluster点变换到现实空间坐标系，并更新点云坐标
        /// </summary>
        /// <param name="rawPoint">雷达点数据对象，坐标单位为米</param>
        /// <param name="paramSet">空间旋转位移参数集</param>
        /// <exception cref="ArgumentNullException">空间旋转位移参数不能为空</exception>
        ///// <param name="paramSet">空间旋转位移参数集，假如为null将不进行任何变换（点在单机坐标系下的坐标与在雷达自身坐标系下相同）</param>
        public static void TransformPoint(ref ClusterData rawPoint, CoordTransParamSet paramSet)
        {
            if (paramSet == null)
                throw new ArgumentNullException(nameof(paramSet), "空间旋转位移参数不能为空");
            //if (paramSet == null) paramSet = _defaultParamSet;
            var coord = TransformPoint(rawPoint.X, rawPoint.Y, rawPoint.Z, paramSet);
            //rawPoint.UpdateCoordinates(coord[0], coord[1], coord[2]);
            rawPoint.UpdateCoordinates(coord[0], coord[1], coord[2], paramSet.DefenseMode, paramSet.Direction);
        }

        /// <summary>
        /// 将点云坐标系中的Object点变换到现实空间坐标系，并更新点云坐标
        /// </summary>
        /// <param name="rawPoint">雷达点数据对象，坐标单位为米</param>
        /// <param name="paramSet">空间旋转位移参数集</param>
        /// <exception cref="ArgumentNullException">空间旋转位移参数不能为空</exception>
        ///// <param name="paramSet">空间旋转位移参数集，假如为null将不进行任何变换（点在单机坐标系下的坐标与在雷达自身坐标系下相同）</param>
        public static void TransformPoint(ref ObjectData rawPoint, CoordTransParamSet paramSet)
        {
            if (paramSet == null)
                throw new ArgumentNullException(nameof(paramSet), "空间旋转位移参数不能为空");
            //if (paramSet == null) paramSet = _defaultParamSet;
            var coord = TransformPoint(rawPoint.X, rawPoint.Y, rawPoint.Z, paramSet);
            //rawPoint.UpdateCoordinates(coord[0], coord[1], coord[2]);
            rawPoint.UpdateCoordinates(coord[0], coord[1], coord[2], paramSet.DefenseMode, paramSet.Direction);
        }

        ///// <summary>
        ///// 将点云坐标系中的点变换到现实空间坐标系，并更新点云坐标
        ///// </summary>
        ///// <param name="rawPoint"></param>
        ///// <param name="xModiRatios">X方向的坐标比例系数</param>
        ///// <param name="yModiRatios">Y方向的坐标比例系数</param>
        ///// <param name="zModiRatios">Z方向的坐标比例系数</param>
        ///// <param name="xOffset">X坐标偏移量</param>
        ///// <param name="yOffset">Y坐标偏移量</param>
        ///// <param name="zOffset">Z坐标偏移量</param>
        //public static void TransformPoint(ref Data rawPoint, CoordinateRatios xModiRatios = null, CoordinateRatios yModiRatios = null, CoordinateRatios zModiRatios = null, double xOffset = 0, double yOffset = 0, double zOffset = 0)
        //{
        //    var coord = TransformPoint(rawPoint.X, rawPoint.Y, rawPoint.Z, xModiRatios, yModiRatios, zModiRatios, xOffset, yOffset, zOffset);
        //    rawPoint.UpdateCoordinates(coord[0], coord[1], coord[2]);
        //}

        /// <summary>
        /// 将点云坐标系中的点变换到现实空间坐标系
        /// </summary>
        /// <param name="x">点云坐标系X坐标（单位：米）</param>
        /// <param name="y">点云坐标系Y坐标（单位：米）</param>
        /// <param name="z">点云坐标系Z坐标（单位：米）</param>
        /// <param name="paramSet">空间旋转位移参数集</param>
        /// <returns>变换后的现实空间坐标（单位：米），以数组方式返回，顺序为X、Y、Z</returns>
        /// <exception cref="ArgumentNullException">空间旋转位移参数不能为空</exception>
        ///// <param name="paramSet">空间旋转位移参数集，假如为null将不进行任何变换（点在单机坐标系下的坐标与在雷达自身坐标系下相同）</param>
        public static double[] TransformPoint(double x, double y, double z, CoordTransParamSet paramSet)
        {
            if (paramSet == null)
                throw new ArgumentNullException(nameof(paramSet), "空间旋转位移参数不能为空");
            //if (paramSet == null) paramSet = _defaultParamSet;
            double xOffset = paramSet.XOffset, yOffset = paramSet.YOffset, zOffset = paramSet.ZOffset;
            CoordinateRatios xModiRatios = paramSet.XmodifiedRatios, yModiRatios = paramSet.YmodifiedRatios, zModiRatios = paramSet.ZmodifiedRatios;
            return TransformPoint(x, y, z, xModiRatios, yModiRatios, zModiRatios, xOffset, yOffset, zOffset);
        }

        /// <summary>
        /// 将点云坐标系中的点变换到现实空间坐标系
        /// </summary>
        /// <param name="x">点云坐标系X坐标（单位：米）</param>
        /// <param name="y">点云坐标系Y坐标（单位：米）</param>
        /// <param name="z">点云坐标系Z坐标（单位：米）</param>
        /// <param name="xModiRatios">X方向的坐标比例系数</param>
        /// <param name="yModiRatios">Y方向的坐标比例系数</param>
        /// <param name="zModiRatios">Z方向的坐标比例系数</param>
        /// <param name="xOffset">X坐标偏移量</param>
        /// <param name="yOffset">Y坐标偏移量</param>
        /// <param name="zOffset">Z坐标偏移量</param>
        /// <returns>变换后的现实空间坐标（单位：米），以数组方式返回，顺序为X、Y、Z</returns>
        public static double[] TransformPoint(double x, double y, double z, CoordinateRatios xModiRatios = null, CoordinateRatios yModiRatios = null, CoordinateRatios zModiRatios = null, double xOffset = 0, double yOffset = 0, double zOffset = 0)
        {
            xModiRatios = xModiRatios ?? new CoordinateRatios(1, 0, 0);
            yModiRatios = yModiRatios ?? new CoordinateRatios(0, 1, 0);
            zModiRatios = zModiRatios ?? new CoordinateRatios(0, 0, 1);
            double
                xModified = xModiRatios.Xratio * x + xModiRatios.Yratio * y + xModiRatios.Zratio * z + xOffset,
                yModified = yModiRatios.Xratio * x + yModiRatios.Yratio * y + yModiRatios.Zratio * z + yOffset,
                zModified = zModiRatios.Xratio * x + zModiRatios.Yratio * y + zModiRatios.Zratio * z + zOffset;

            return
#if NET9_0_OR_GREATER
            [
                xModified,  // 变换后X坐标
                yModified,  // 变换后Y坐标
                zModified   // 变换后Z坐标
            ];
#elif NET45_OR_GREATER
                new double[]
                {
                    xModified,  // 变换后X坐标
                    yModified,  // 变换后Y坐标
                    zModified   // 变换后Z坐标
                };
#endif
        }
    }
}
