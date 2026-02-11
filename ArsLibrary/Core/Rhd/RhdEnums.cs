using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Core.Rhd
{
    /// <summary>
    /// HURYS基数据协议版本号
    /// </summary>
    public enum ProtocolVersion
    {
        /// <summary>
        /// HURYS基数据协议-v2.1.3
        /// </summary>
        [EnumDescription("HURYS基数据协议-v2.1.3")]
        v213 = 1,

        /// <summary>
        /// HURYS基数据协议-v2.1.2
        /// </summary>
        [EnumDescription("HURYS基数据协议-v2.1.2")]
        v212 = 2,
    }

    /// <summary>
    /// 目标类型
    /// </summary>
    public enum ObjectType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EnumDescription("未知")]
        Unknown = 0,

        /// <summary>
        /// 机动车
        /// </summary>
        [EnumDescription("机动车")]
        MotorVehicle = 1,

        /// <summary>
        /// 行人
        /// </summary>
        [EnumDescription("行人")]
        Pedestrian = 2,

        /// <summary>
        /// 非机动车
        /// </summary>
        [EnumDescription("")]
        NonMotorVehicle = 3,
    }
}
