using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 雷达基础类
    /// </summary>
    [ProtoContract]
    public abstract class RadarBase : IComparable<RadarBase>
    {
        /// <inheritdoc/>
        public int CompareTo(RadarBase other)
        {
            throw new NotImplementedException();
        }

        //#region 对象比较
        ///// <summary>
        ///// 返回此实例的哈希代码
        ///// </summary>
        ///// <returns></returns>
        //public override int GetHashCode()
        //{
        //    return CurrentDistance.GetHashCode() | ThreatLevel.GetHashCode();
        //}

        //#region 是否相等的比较
        ///// <summary>
        ///// 判断当前实例与某对象是否相等
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public override bool Equals(object obj)
        //{
        //    return obj is RadarBase radar && CurrentDistance == radar.CurrentDistance && ThreatLevel == radar.ThreatLevel;
        //}

        ///// <summary>
        ///// 重新定义的相等符号
        ///// </summary>
        ///// <param name="left">左侧实例</param>
        ///// <param name="right">右侧实例</param>
        ///// <returns></returns>
        //public static bool operator ==(RadarBase left, RadarBase right)
        //{
        //    //return (object)left == null ? (object)right == null : left.Equals(right);
        //    return left is null ? right is null : left.Equals(right);
        //}

        ///// <summary>
        ///// 重新定义的不等符号
        ///// </summary>
        ///// <param name="left">左侧实例</param>
        ///// <param name="right">右侧实例</param>
        ///// <returns></returns>
        //public static bool operator !=(RadarBase left, RadarBase right)
        //{
        //    return !(left == right);
        //}
        //#endregion

        //#region 大小的比较
        ///// <summary>
        ///// 将当前实例与另一实例相比较，并返回比较结果符号：-1 小于，0 相等，1 大于
        ///// </summary>
        ///// <param name="other">与当前实例比较的另一实例</param>
        ///// <returns></returns>
        //public int CompareTo(RadarBase other)
        //{
        //    int d = CurrentDistance.CompareTo(other.CurrentDistance), a = ThreatLevel.CompareTo(other.ThreatLevel);
        //    int result;
        //    if (d == 0 && a == 0)
        //        result = 0;
        //    else
        //        result = (d == -1 && a != -1) || (d != -1 && a == 1) ? -1 : 1;
        //    return result;
        //}

        ///// <summary>
        ///// 重新定义的小于符号
        ///// </summary>
        ///// <param name="left">左侧实例</param>
        ///// <param name="right">右侧实例</param>
        ///// <returns></returns>
        //public static bool operator <(RadarBase left, RadarBase right)
        //{
        //    return left.CompareTo(right) < 0;
        //}

        ///// <summary>
        ///// 重新定义的小于等于符号
        ///// </summary>
        ///// <param name="left">左侧实例</param>
        ///// <param name="right">右侧实例</param>
        ///// <returns></returns>
        //public static bool operator <=(RadarBase left, RadarBase right)
        //{
        //    return left.CompareTo(right) <= 0;
        //}

        ///// <summary>
        ///// 重新定义的大于符号
        ///// </summary>
        ///// <param name="left">左侧实例</param>
        ///// <param name="right">右侧实例</param>
        ///// <returns></returns>
        //public static bool operator >(RadarBase left, RadarBase right)
        //{
        //    return left.CompareTo(right) > 0;
        //}

        ///// <summary>
        ///// 重新定义的大于等于符号
        ///// </summary>
        ///// <param name="left">左侧实例</param>
        ///// <param name="right">右侧实例</param>
        ///// <returns></returns>
        //public static bool operator >=(RadarBase left, RadarBase right)
        //{
        //    return left.CompareTo(right) >= 0;
        //}
        //#endregion
        //#endregion
    }
}
