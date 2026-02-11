using ArsLibrary.Core;
using CommonLib.Function.Fitting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model.Rhd
{
    /// <summary>
    /// 空间旋转与位移的参数集
    /// </summary>
    public class CoordTransParamSet
    {
        //用于计算新XYZ坐标的原坐标系数：默认值
        private readonly CoordinateRatios _defModiRatiosX = new CoordinateRatios(1, 0, 0);
        private readonly CoordinateRatios _defModiRatiosY = new CoordinateRatios(0, 1, 0);
        private readonly CoordinateRatios _defModiRatiosZ = new CoordinateRatios(0, 0, 1);
        //用于计算新XYZ坐标的原坐标系数
        private CoordinateRatios _xRatios, _yRatios, _zRatios;

        /// <summary>
        /// X坐标偏移量（单位：米）
        /// </summary>
        public double XOffset { get; internal set; }

        /// <summary>
        /// Y坐标偏移量（单位：米）
        /// </summary>
        public double YOffset { get; internal set; }

        /// <summary>
        /// Z坐标偏移量（单位：米）
        /// </summary>
        public double ZOffset { get; internal set; }

        /// <summary>
        /// 修改后的X坐标的原XYZ坐标系数，默认值保持原XYZ坐标不变，设置为null将恢复到默认值
        /// </summary>
        public CoordinateRatios XmodifiedRatios
        {
            get { return _xRatios ?? _defModiRatiosX; }
            internal set { _xRatios = value; }
        }

        /// <summary>
        /// 修改后的Y坐标的原XYZ坐标系数，默认值保持原XYZ坐标不变，设置为null将恢复到默认值
        /// </summary>
        public CoordinateRatios YmodifiedRatios
        {
            get { return _yRatios ?? _defModiRatiosY; }
            internal set { _yRatios = value; }
        }

        /// <summary>
        /// 修改后的Z坐标的原XYZ坐标系数，默认值保持原XYZ坐标不变，设置为null将恢复到默认值
        /// </summary>
        public CoordinateRatios ZmodifiedRatios
        {
            get { return _zRatios ?? _defModiRatiosZ; }
            internal set { _zRatios = value; }
        }

        /// <summary>
        /// 防御模式：1 点，2 线，3 面
        /// </summary>
        public DefenseMode DefenseMode { get; internal set; } = DefenseMode.Vertex;

        /// <summary>
        /// 方向
        /// </summary>
        public Directions Direction { get; internal set; }

        /// <summary>
        /// 默认构造函数，三轴角度和坐标偏移量初始化为0
        /// </summary>
        public CoordTransParamSet() { }

        /// <summary>
        /// 使用给定的三轴坐标系数和坐标偏移量构造函数
        /// </summary>
        /// <param name="xRatios">用于计算修改后的X坐标的原XYZ坐标系数</param>
        /// <param name="yRatios">用于计算修改后的Y坐标的原XYZ坐标系数</param>
        /// <param name="zRatios">用于计算修改后的Z坐标的原XYZ坐标系数</param>
        /// <param name="xOffset">X坐标偏移量（单位：米）</param>
        /// <param name="yOffset">Y坐标偏移量（单位：米）</param>
        /// <param name="zOffset">Z坐标偏移量（单位：米）</param>
        public CoordTransParamSet(CoordinateRatios xRatios = null, CoordinateRatios yRatios = null, CoordinateRatios zRatios = null, double xOffset = 0, double yOffset = 0, double zOffset = 0)
        {
            UpdateModiRatios(xRatios, yRatios, zRatios);
            UpdateOffsets(xOffset, yOffset, zOffset);
        }

        /// <summary>
        /// 获取默认的坐标转换参数实例
        /// </summary>
        /// <returns></returns>
        public static CoordTransParamSet CreateDefault()
        {
            return new CoordTransParamSet(null, null, null, 0, 0, 0);
        }

        /// <summary>
        /// 更新用于计算新X/Y/Z坐标的原坐标系数
        /// </summary>
        /// <param name="xRatios">用于计算修改后的X坐标的原XYZ坐标系数</param>
        /// <param name="yRatios">用于计算修改后的Y坐标的原XYZ坐标系数</param>
        /// <param name="zRatios">用于计算修改后的Z坐标的原XYZ坐标系数</param>
        public void UpdateModiRatios(CoordinateRatios xRatios = null, CoordinateRatios yRatios = null, CoordinateRatios zRatios = null)
        {
            XmodifiedRatios = xRatios;
            YmodifiedRatios = yRatios;
            ZmodifiedRatios = zRatios;
        }

        /// <summary>
        /// 更新XYZ坐标偏移量
        /// </summary>
        /// <param name="xOffset">X坐标偏移量（单位：米）</param>
        /// <param name="yOffset">Y坐标偏移量（单位：米）</param>
        /// <param name="zOffset">Z坐标偏移量（单位：米）</param>
        public void UpdateOffsets(double xOffset = 0, double yOffset = 0, double zOffset = 0)
        {
            XOffset = xOffset;
            YOffset = yOffset;
            ZOffset = zOffset;
        }

        /// <summary>
        /// 更新防御模式与所朝方向
        /// </summary>
        /// <param name="defense"></param>
        /// <param name="dir"></param>
        public void UpdateDefenseModeNDirection(DefenseMode defense, Directions dir)
        {
            DefenseMode = defense;
            Direction = dir;
        }
    }
}
