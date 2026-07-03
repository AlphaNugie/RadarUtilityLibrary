using CommonLib.Extensions;
using CommonLib.Function;
using CommonLib.Function.MathUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.Model
{
    /// <summary>
    /// 雷达内部点的黑名单实体类
    /// </summary>
    public class RadarBlacklist
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 对应的雷达ID
        /// </summary>
        public int RadarId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        #region 雷达坐标系限制
        /// <summary>
        /// 是否限制雷达坐标系坐标
        /// </summary>
        public bool RadarCoorsLimited { get; set; }

        /// <summary>
        /// 雷达坐标系X轴最小值
        /// </summary>
        public double RadarxMin { get; set; }

        /// <summary>
        /// 雷达坐标系x轴最大值
        /// </summary>
        public double RadarxMax { get; set; }

        /// <summary>
        /// 雷达坐标系y轴最小值
        /// </summary>
        public double RadaryMin { get; set; }

        /// <summary>
        /// 雷达坐标系y轴最大值
        /// </summary>
        public double RadaryMax { get; set; }
        #endregion

        #region 单机坐标系限制
        /// <summary>
        /// 是否限制单机坐标系坐标
        /// </summary>
        public bool ClaimerCoorsLimited { get; set; }

        /// <summary>
        /// 单机坐标系X轴最小值
        /// </summary>
        public double ClaimerxMin { get; set; }

        /// <summary>
        /// 单机坐标系X轴最大值
        /// </summary>
        public double ClaimerxMax { get; set; }

        /// <summary>
        /// 单机坐标系y轴最小值
        /// </summary>
        public double ClaimeryMin { get; set; }

        /// <summary>
        /// 单机坐标系y轴最大值
        /// </summary>
        public double ClaimeryMax { get; set; }

        /// <summary>
        /// 单机坐标系z轴最小值
        /// </summary>
        public double ClaimerzMin { get; set; }

        /// <summary>
        /// 单机坐标系z轴最大值
        /// </summary>
        public double ClaimerzMax { get; set; }
        #endregion

        #region RCS限制
        /// <summary>
        /// 是否限制RCS值
        /// </summary>
        public bool RcsLimited { get; set; }

        /// <summary>
        /// RCS值最小值
        /// </summary>
        public double RcsMin { get; set; }

        /// <summary>
        /// RCS值最大值
        /// </summary>
        public double RcsMax { get; set; }
        #endregion

        #region 角度限制
        /// <summary>
        /// 是否限制角度
        /// </summary>
        public bool AngleLimited { get; set; }

        /// <summary>
        /// 角度最小值
        /// </summary>
        public double AngleMin { get; set; }

        /// <summary>
        /// 角度最大值
        /// </summary>
        public double AngleMax { get; set; }
        #endregion

        #region 单机姿态限制
        /// <summary>
        /// 是否限制单机坐标系坐标
        /// </summary>
        public bool ClaimerPostureLimited { get; set; }

        /// <summary>
        /// 走行位置最小值（单位：米，默认-100）
        /// </summary>
        public double WalkPosMin { get; set; } = -100;

        /// <summary>
        /// 走行位置最大值（单位：米，默认2000）
        /// </summary>
        public double WalkPosMax { get; set; } = 2000;

        /// <summary>
        /// 俯仰角最小值（单位：度，默认-90）
        /// </summary>
        public double PitchAngleMin { get; set; } = -90;

        /// <summary>
        /// 俯仰角最大值（单位：度，默认90）
        /// </summary>
        public double PitchAngleMax { get; set; } = 90;

        /// <summary>
        /// 回转角最小值（单位：度，默认-180）
        /// </summary>
        public double YawAngleMin { get; set; } = -180;

        /// <summary>
        /// 回转角最大值（单位：度，默认180）
        /// </summary>
        public double YawAngleMax { get; set; } = 180;

        /// <summary>
        /// 伸缩长度最小值（单位：米，默认-1）
        /// </summary>
        public double StretchLenMin { get; set; } = -1;

        /// <summary>
        /// 伸缩长度最大值（单位：米，默认20）
        /// </summary>
        public double StretchLenMax { get; set; } = 20;
        #endregion

        ///// <summary>
        ///// 是否处于RCS值限制范围内
        ///// </summary>
        //public bool WithinRcsLimits { get; private set; }

        ///// <summary>
        ///// 是否处于雷达坐标限制范围内
        ///// </summary>
        //public bool WithinRadarLimits { get; private set; }

        ///// <summary>
        ///// 是否处于单机坐标限制范围内
        ///// </summary>
        //public bool WithinClaimerLimits { get; private set; }

        ///// <summary>
        ///// 是否位于角度限制范围内
        ///// </summary>
        //public bool WithinAngleLimits { get; private set; }

        /// <summary>
        /// 构造器，默认值
        /// </summary>
        public RadarBlacklist()
        {
            Id = -1;
            RadarId = -1;
        }

        /// <summary>
        /// 构造器，从公共变量获取属性值，再用给定的DataRow对象覆盖各属性的值
        /// </summary>
        /// <param name="row"></param>
        public RadarBlacklist(DataRow row) : this()
        {
            Id = row.Convert<int>("record_id");
            RadarId = row.Convert<int>("radar_id");
            RadarCoorsLimited = row.Convert<int>("radar_coors_limited") == 1;
            RadarxMin = row.Convert<double>("radar_x_min");
            RadarxMax = row.Convert<double>("radar_x_max");
            RadaryMin = row.Convert<double>("radar_y_min");
            RadaryMax = row.Convert<double>("radar_y_max");
            ClaimerCoorsLimited = row.Convert<int>("claimer_coors_limited") == 1;
            ClaimerxMin = row.Convert<double>("claimer_x_min");
            ClaimerxMax = row.Convert<double>("claimer_x_max");
            ClaimeryMin = row.Convert<double>("claimer_y_min");
            ClaimeryMax = row.Convert<double>("claimer_y_max");
            ClaimerzMin = row.Convert<double>("claimer_z_min");
            ClaimerzMax = row.Convert<double>("claimer_z_max");
            RcsLimited = row.Convert<int>("rcs_limited") == 1;
            RcsMin = row.Convert<double>("rcs_min");
            RcsMax = row.Convert<double>("rcs_max");
            AngleLimited = row.Convert<int>("angle_limited") == 1;
            AngleMin = row.Convert<double>("angle_min");
            AngleMax = row.Convert<double>("angle_max");
            ClaimerPostureLimited = row.Convert<int>("claimer_posture_limited") == 1;
            WalkPosMin = row.Convert<double>("walk_pos_min");
            WalkPosMax = row.Convert<double>("walk_pos_max");
            PitchAngleMin = row.Convert<double>("pitch_angle_min");
            PitchAngleMax = row.Convert<double>("pitch_angle_max");
            YawAngleMin = row.Convert<double>("yaw_angle_min");
            YawAngleMax = row.Convert<double>("yaw_angle_max");
            StretchLenMin = row.Convert<double>("stretch_len_min");
            StretchLenMax = row.Convert<double>("stretch_len_max");
        }

        /// <summary>
        /// 判断某点的各项属性（反射率（RCS），设备内部坐标，空间坐标，角度）是否在限制范围中，同时还可以判断是否处于单机姿态限制范围内
        /// 核心逻辑：只要任意一种限制条件被启用，且当前状态处于该限制的范围内，就返回true
        /// </summary>
        /// <param name="point">待检测的三维点</param>
        /// <param name="walkPos">走行位置，假如为空则不限制</param>
        /// <param name="pitchAngle">俯仰角度，假如为空则不限制</param>
        /// <param name="yawAngle">回转角度，假如为空则不限制</param>
        /// <param name="stretchLen">伸缩长度，假如为空则不限制</param>
        /// <returns>只要任一启用的限制类型处于范围内，返回true；否则返回false</returns>
        public bool Contains(Point3D point, double? walkPos = null, double? pitchAngle = null, double? yawAngle = null, double? stretchLen = null)
        {
            // 假如没有任何一种限制启用，或point为空，则返回false
            if (point == null || (!RcsLimited && !RadarCoorsLimited && !ClaimerCoorsLimited && !AngleLimited && !ClaimerPostureLimited))
                return false;

            // 逐一检测每种限制类型：只要该限制启用且在范围内，立即返回true

            // 1. 检测RCS值限制
            if (RcsLimited && point.Reflectivity.Between(RcsMin, RcsMax))
                return true;

            // 2. 检测雷达坐标系限制（InterX/InterY均需在范围内）
            if (RadarCoorsLimited && point.InterX.Between(RadarxMin, RadarxMax) && point.InterY.Between(RadaryMin, RadaryMax))
                return true;

            // 3. 检测单机坐标系限制（X/Y/Z均需在范围内）
            if (ClaimerCoorsLimited && point.X.Between(ClaimerxMin, ClaimerxMax) && point.Y.Between(ClaimeryMin, ClaimeryMax) && point.Z.Between(ClaimerzMin, ClaimerzMax))
                return true;

            // 4. 检测角度限制
            if (AngleLimited && point.InterAngle.Between(AngleMin, AngleMax))
                return true;

            // 5. 检测单机姿态限制
            // 对于每个可空参数，若为null则视为"不限制该维度"（即该维度自动通过），否则检查是否在范围内
            // 当 ClaimerPostureLimited 启用时，所有传入的非null参数都必须同时在各自范围内，才算满足此限制类型
            if (ClaimerPostureLimited)
            {
                bool walkPosOk = !walkPos.HasValue || walkPos.Value.Between(WalkPosMin, WalkPosMax);
                bool pitchAngleOk = !pitchAngle.HasValue || pitchAngle.Value.Between(PitchAngleMin, PitchAngleMax);
                bool yawAngleOk = !yawAngle.HasValue || yawAngle.Value.Between(YawAngleMin, YawAngleMax);
                bool stretchLenOk = !stretchLen.HasValue || stretchLen.Value.Between(StretchLenMin, StretchLenMax);
                if (walkPosOk && pitchAngleOk && yawAngleOk && stretchLenOk)
                    return true;
            }

            // 所有启用的限制类型都不在范围内，返回false
            return false;
        }
    }
}
