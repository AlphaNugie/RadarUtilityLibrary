using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 帧消息是否通过各项限制范围的过滤
    /// </summary>
    public class FrameFilterFlags
    {
        #region 装船机专用
        /// <summary>
        /// 溜筒雷达水平方向是否在范围内
        /// </summary>
        public bool BucketHorizInBound { get; set; } = true;

        /// <summary>
        /// 溜筒雷达竖直方向是否在范围内
        /// </summary>
        public bool BucketVertInBound { get; set; } = true;
        #endregion

        /// <summary>
        /// RCS值是否在范围内
        /// </summary>
        public bool RcsInBound { get; set; } = true;

        /// <summary>
        /// 是否通过雷达坐标范围的限制
        /// </summary>
        public bool RadarCoorsInBound { get;set; } = true;

        /// <summary>
        /// 是否通过单机坐标范围的限制
        /// </summary>
        public bool ClaimerCoorsInBound { get;set; } = true;

        /// <summary>
        /// 是否通过角度范围的限制
        /// </summary>
        public bool AngleInBound { get;set; } = true;

        /// <summary>
        /// 是否全部通过限制
        /// </summary>
        /// <returns></returns>
        public bool IsAllInBound()
        {
            bool[] props = typeof(FrameFilterFlags).GetProperties().Where(prop => prop.PropertyType == typeof(bool)).Select(prop => (bool)prop.GetValue(this)).ToArray();
            return props.Min(); //只有全为true时才返回true
        }
    }
}
