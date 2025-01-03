//using ArsLibrary.Model;
using CommonLib.Extensions;
using CommonLib.Function.Fitting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArsLibrary.Core
{
    /// <summary>
    /// 公共变量
    /// </summary>
    public struct ArsConst
    {
        /// <summary>
        /// 默认测距距离
        /// </summary>
        public const double DEF_DIST = 50;

        /// <summary>
        /// 空白报文
        /// </summary>
        public const string EmptyMessage = "05 00 00 06 00 00 00 C9 91 10 00 00 00 08 00 00 02 01 40 20 80 01 80 D8 00 00 04 00 00 07 00 5E 22 0D 02 00 00 00 00 05 00 00 06 00 00 00 C9 93 10 00 00 00";

        /// <summary>
        /// 报文分发服务的本地端口号
        /// </summary>
        public const int ContentServerPort = 46443;

        #region 连接
        /// <summary>
        /// 本地IP地址
        /// </summary>
        public static string IpAddress_Local { get; set; } = string.Empty;
        #endregion

        #region 正则表达式
        /// <summary>
        /// 传感器信息的正则表达式
        /// </summary>
        public const string Pattern_SensorMessage = @"0[1-8]\s00\s00\s0[0-7]\s[0-7][0-9a-fA-F](\s[0-9a-fA-F]{2}){8}";

        /// <summary>
        /// 集群列表(600,cluster list status)，目标列表(60A,object list status)消息的正则表达式
        /// </summary>
        public const string Pattern_ClusterObjectStatus = @"0[1-8]\s00\s00\s06\s[0-7][0aA](\s[0-9a-fA-F]{2}){8}";

        /// <summary>
        /// 除集群列表(600,cluster list status)，目标列表(60A,object list status)之外所有消息的正则表达式
        /// </summary>
        public const string Pattern_NotClusterObjectStatus = @"0[1-8]\s00\s00\s(0[0-57]\s[0-7][0-9a-fA-F]|06\s[0-7][1-9b-fB-F])(\s[0-9a-fA-F]{2}){8}";

        /// <summary>
        /// 被集群列表(600,cluster list status)，目标列表(60A,object list status)包裹住的消息集合的正则表达式（被包裹消息的帧ID非600或60A，消息之间可存在任意空白符）
        /// </summary>
        public const string Pattern_WrappedStatus = @"0[1-8]\s00\s00\s06\s[0-7][0aA](\s[0-9a-fA-F]{2}){8}(\s)*(0[1-8]\s00\s00\s(0[0-57]\s[0-7][0-9a-fA-F]|06\s[0-7][1-9b-fB-F])(\s[0-9a-fA-F]{2}){8}(\s)*)+0[1-8]\s00\s00\s06\s[0-7][0aA](\s[0-9a-fA-F]{2}){8}";

        /// <summary>
        /// 被集群列表(600,cluster list status)，目标列表(60A,object list status)包裹住的消息集合的正则表达式的模板（被包裹消息的帧ID非600或60A，消息之间可存在任意空白符）
        /// </summary>
        public const string Pattern_WrappedStatusTemplate = @"{0}(\s)*((?!{0}).(\s)*)+";

        /// <summary>
        /// 正则处理类，模式为被集群列表(600,cluster list status)，目标列表(60A,object list status)包裹住的消息集合的正则表达式（被包裹消息的帧ID非600或60A，消息之间可存在任意空白符）
        /// </summary>
        public static Regex RegWrapped { get; } = new Regex(Pattern_WrappedStatus, RegexOptions.Compiled);
        //public static Regex RegWrapped { get; } = new Regex(string.Format(Pattern_WrappedStatusTemplate, Pattern_ClusterObjectStatus), RegexOptions.Compiled);
        #endregion

        #region 图形
        /// <summary>
        /// 坐标系图像文件原图
        /// </summary>
        public static Image OriginalImage = new Bitmap(new MemoryStream(Convert.FromBase64String(ImageBase64String.ARSCoorBase64)));

        /// <summary>
        /// 坐标系图像文件原图（空白）
        /// </summary>
        public static Image OriginalImageEmpty = new Bitmap(new MemoryStream(Convert.FromBase64String(ImageBase64String.ARSCoorEmptyBase64)));

        /// <summary>
        /// 坐标系图像文件原图（double）
        /// </summary>
        public static Image OriginalImageDouble = new Bitmap(new MemoryStream(Convert.FromBase64String(ImageBase64String.ARSCoorDoubleBase64)));

        /// <summary>
        /// 坐标系图像文件原图（triple）
        /// </summary>
        public static Image OriginalImageTriple = new Bitmap(new MemoryStream(Convert.FromBase64String(ImageBase64String.ARSCoorTripleBase64)));

        /// <summary>
        /// 图片缩小时（或鼠标滑轮向上滑动时）的缩小比例系数，放大时则是倒数（-1次幂）
        /// </summary>
        public static float ScrollRatio { get; set; } = (float)0.9;

        /// <summary>
        /// 像素与雷达坐标系单位（m）的比值
        /// </summary>
        public static double R { get; set; } = 4.12; //实际比例应为8.32 : 2

        /// <summary>
        /// 绘点时点的粗细（直径，像素）
        /// </summary>
        public static float T { get; set; } = 2;

        /// <summary>
        /// 是否显示已被过滤掉的点
        /// </summary>
        public static bool ShowDesertedPoints { get; set; } = true;
        #endregion

        #region 检测
        /// <summary>
        /// RCS最小值
        /// </summary>
        public static int RcsMinimum { get; set; } = -64;

        /// <summary>
        /// RCS最大值
        /// </summary>
        public static int RcsMaximum { get; set; } = 64;

        /// <summary>
        /// 是否使用公共RCS范围
        /// </summary>
        public static bool UsePublicRcsRange { get; set; } = true;

        /// <summary>
        /// 集群过滤器是否启用
        /// </summary>
        public static bool ClusterFilterEnabled { get; set; } = false;

        /// <summary>
        /// 目标过滤器是否启用
        /// </summary>
        public static bool ObjectFilterEnabled { get; set; } = false;
        #endregion

        #region 迭代
        /// <summary>
        /// 距离迭代是否启用，启用则在收到新值时先进行检定再决定是否替代当前值，未启用则直接取代
        /// </summary>
        public static bool IterationEnabled { get; set; } = false;

        /// <summary>
        /// 距离差限定值（不超过此值则以新值替代当前值）
        /// </summary>
        public static double IteDistLimit { get; set; } = 0.5;

        /// <summary>
        /// 新值检定次数限定值（超过此值则以假定值替代当前值）
        /// </summary>
        public static int IteCountLimit { get; set; } = 5;

        /// <summary>
        /// 迭代距离差的上限（单位米），距离差超过此上限则绝不用新值代替当前值（此上限不受迭代是否启用、是否使用TCP模式影响）
        /// </summary>
        public static double IteDistUpperLimit { get; set; } = 1000;
        #endregion
    }

    /// <summary>
    /// 公共方法
    /// </summary>
    public static class ArsFunc
    {
        /// <summary>
        /// 根据输入的参数判断走行的方向
        /// </summary>
        /// <param name="walkSpeed">走行速度（米/秒），为null时不参与判断，有值时大于0向前、小于0向后、等于0停止</param>
        /// <param name="walkFor">走行是否向前的布尔量，为null与是否向后一起不参与判断，有值时与是否向后均为false代表停止</param>
        /// <param name="walkBack">走行是否向后的布尔量，为null与是否向前一起不参与判断，有值时与是否向前均为false代表停止</param>
        /// <param name="yawAngle">回转角度值（角度°），大于0时在走行正向右侧，小于0在走行正向左侧，等于0回正</param>
        /// <param name="armInField">大臂是否在场地内的布尔量，通常判断方法为回转角度的绝对值是否大于某特定值</param>
        /// <returns></returns>
        public static Directions GetWalkingDirection(double? walkSpeed, bool? walkFor, bool? walkBack, double yawAngle, bool armInField)
        {
            var dir = Directions.None;

            if (walkSpeed == null) goto PART2;
            //假如走行速度为0，则停止
            if (walkSpeed == 0)
                dir = Directions.None;
            //假如大臂不在堆场范围内（一般因为回转角绝对值过小），则判断是向前还是向后
            else if (!armInField)
                dir = walkSpeed > 0 ? Directions.Front : Directions.Back;
            //大臂在堆场范围内的情况下，假如走行速度与回转角度同符号（回转为正的场在走行正向右侧），则在向大臂左侧运动，否则在向大臂右侧运动
            else
                dir = walkSpeed * yawAngle > 0 ? Directions.Left : Directions.Right;

            PART2:
            if (walkFor == null || walkBack == null) goto END;
            //假如既没有向前走也没有向后走，则停止
            if (!walkFor.Value && !walkBack.Value)
                dir = Directions.None;
            //假如大臂不在堆场范围内（一般因为回转角绝对值过小），则判断是向前还是向后
            else if (!armInField)
                dir = walkSpeed > 0 ? Directions.Front : Directions.Back;
            //当行走向前时，回转角小于0相当于向左侧靠近，否则相当于向右侧靠近
            else if (walkFor.Value)
                dir = yawAngle > 0 ? Directions.Left : Directions.Right;
            //当行走向后时，回转角小于0相当于向左侧靠近，否则相当于向右侧靠近
            else if (walkBack.Value)
                dir = yawAngle < 0 ? Directions.Left : Directions.Right;

            END:
            return dir;
        }

        /// <summary>
        /// 根据给定的行走方向，俯仰方向，回转方向判断臂架运动方向（只当行走与回转方向不相反时）
        /// </summary>
        /// <param name="walkDir">行走方向</param>
        /// <param name="pitchDir">俯仰方向</param>
        /// <param name="yawDir">回转方向</param>
        /// <returns></returns>
        public static Directions GetMovingDirection(Directions walkDir, Directions pitchDir, Directions yawDir)
        {
            Directions result = Directions.None;
            //左右和上下的方向范围，假如行走、俯仰、回转不在这些范围内，则不再继续
            List<Directions>
                walkdirs = new List<Directions>() { Directions.None, Directions.Front, Directions.Back, Directions.Left, Directions.Right },
                yawdirs = new List<Directions>() { Directions.None, Directions.Left, Directions.Right },
                pitchdirs = new List<Directions>() { Directions.None, Directions.Up, Directions.Down };
            if (!walkdirs.Contains(walkDir) || !yawdirs.Contains(yawDir) || !pitchdirs.Contains(pitchDir))
                goto END;
            ////ver1
            //if ((walkDir == Directions.Left && yawDir == Directions.None) || (walkDir == Directions.Left && yawDir == Directions.Left) || (walkDir == Directions.None && yawDir == Directions.Left))
            //    result = Directions.Left;
            //else if ((walkDir == Directions.Right && yawDir == Directions.None) || (walkDir == Directions.Right && yawDir == Directions.Right) || (walkDir == Directions.None && yawDir == Directions.Right))
            //    result = Directions.Right;

            ////ver2
            //if (walkDir == Directions.None && yawDir == Directions.None)
            //    result = pitchDir == Directions.None ? Directions.None : Directions.Mixed;
            //else if ((walkDir == Directions.Left && yawDir == Directions.None) || (walkDir == Directions.Left && yawDir == Directions.Left) || (walkDir == Directions.None && yawDir == Directions.Left))
            //    result = Directions.Left;
            //else if ((walkDir == Directions.Right && yawDir == Directions.None) || (walkDir == Directions.Right && yawDir == Directions.Right) || (walkDir == Directions.None && yawDir == Directions.Right))
            //    result = Directions.Right;
            //else if ((walkDir == Directions.Left && yawDir == Directions.Right) || (walkDir == Directions.Right && yawDir == Directions.Left))
            //    result = Directions.Mixed;

            //ver3
            //假如走行向前或向后，则运动方向与走行方向一致（不论俯仰、回转如何运动）
            if (walkDir == Directions.Front || walkDir == Directions.Back)
                result = walkDir;
            //假如走行与回转无动作，则根据俯仰方向判断：俯仰不动时运动方向为None，俯仰动作时运动方向为混合方向
            else if (walkDir == Directions.None && yawDir == Directions.None)
                result = pitchDir == Directions.None ? Directions.None : Directions.Mixed;
            //假如走行与回转方向无矛盾，且至少有一方向左，则运动方向向左
            else if ((walkDir == Directions.Left && yawDir == Directions.None) || (walkDir == Directions.Left && yawDir == Directions.Left) || (walkDir == Directions.None && yawDir == Directions.Left))
                result = Directions.Left;
            //假如走行与回转方向无矛盾，且至少有一方向右，则运动方向向右
            else if ((walkDir == Directions.Right && yawDir == Directions.None) || (walkDir == Directions.Right && yawDir == Directions.Right) || (walkDir == Directions.None && yawDir == Directions.Right))
                result = Directions.Right;
            //假如走行与回转方向相反，则运动方向为混合方向
            else if ((walkDir == Directions.Left && yawDir == Directions.Right) || (walkDir == Directions.Right && yawDir == Directions.Left))
                result = Directions.Mixed;
            END:
            return result;
        }

        /// <summary>
        /// 获取相反方向
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Directions GetOppositeDir(this Directions dir)
        {
            var other_dir = Directions.None;
            switch (dir)
            {
                case Directions.Front:
                    other_dir = Directions.Back;
                    break;
                case Directions.Back:
                    other_dir = Directions.Front;
                    break;
                case Directions.Left:
                    other_dir = Directions.Right;
                    break;
                case Directions.Right:
                    other_dir = Directions.Left;
                    break;
                case Directions.Up:
                    other_dir = Directions.Down;
                    break;
                case Directions.Down:
                    other_dir = Directions.Up;
                    break;
                case Directions.Mixed:
                    other_dir = Directions.Mixed;
                    break;
            }
            return other_dir;
        }

        /// <summary>
        /// 返回接收信息中包含的包裹消息集合
        /// </summary>
        /// <param name="input">接收信息</param>
        /// <returns></returns>
        public static bool GetWrappedMessage(ref string input)
        {
            MatchCollection matches = ArsConst.RegWrapped.Matches(input);
            bool result = matches != null && matches.Count > 0;
            input = !result ? string.Empty : matches.Cast<Match>().Last().Value;
            return result;
        }

        /// <summary>
        /// 根据目标动态属性获取颜色，超出范围默认为黄色
        /// </summary>
        /// <param name="prop">目标动态属性</param>
        /// <returns></returns>
        public static Color GetColorByDynProp(DynProp prop)
        {
            return GetColorByDynProp(prop, Color.Yellow);
        }

        /// <summary>
        /// 根据目标动态属性获取颜色，超出范围使用默认颜色
        /// </summary>
        /// <param name="prop">目标动态属性</param>
        /// <param name="_default">默认颜色</param>
        /// <returns></returns>
        public static Color GetColorByDynProp(DynProp prop, Color _default)
        {
            switch (prop)
            {
                #region 忽略
                //case DynProp.Stationary:
                //    return Color.FromArgb(211, 211, 211);
                //case DynProp.StationaryCandidate:
                //    return Color.FromArgb(255, 255, 224);
                //case DynProp.CrossingStationary:
                //    return Color.FromArgb(255, 255, 0);
                //case DynProp.Unknown:
                //    return Color.FromArgb(245, 245, 220);
                //case DynProp.CrossingMoving:
                //    return Color.FromArgb(173, 255, 47);
                #endregion
                case DynProp.Moving:
                    return Color.FromArgb(154, 205, 50);
                case DynProp.Oncoming:
                    return Color.FromArgb(0, 255, 127);
                case DynProp.Stopped:
                    return Color.FromArgb(255, 69, 0);
                default:
                    return _default;
            }
        }

        /// <summary>
        /// 根据雷达散射截面属性获取颜色，超出范围默认为黄色
        /// </summary>
        /// <param name="r">雷达散射截面</param>
        /// <returns></returns>
        public static Color GetColorByRcs(double r)
        {
            return GetColorByRcs(r, Color.Yellow);
        }

        /// <summary>
        /// 根据雷达散射截面属性获取颜色，超出范围使用默认颜色
        /// </summary>
        /// <param name="r">雷达散射截面</param>
        /// <param name="_default">默认颜色</param>
        /// <returns></returns>
        public static Color GetColorByRcs(double r, Color _default)
        {
            Color color = _default;
            int thres = (int)Math.Ceiling(r / 10);
            switch (thres)
            {
                case -6:
                case -5:
                case -4:
                    color = Color.FromArgb(0, 245, 255);
                    break;
                case -3:
                    color = Color.FromArgb(95, 158, 160);
                    break;
                case -2:
                    color = Color.FromArgb(34, 139, 34);
                    break;
                case -1:
                    color = Color.FromArgb(255, 255, 0);
                    break;
                case 0:
                    color = Color.FromArgb(255, 130, 71);
                    break;
                case 1:
                    color = Color.FromArgb(255, 20, 147);
                    break;
                case 2:
                    color = Color.FromArgb(139, 101, 139);
                    break;
                case 3:
                    color = Color.FromArgb(139, 0, 139);
                    break;
                case 4:
                    color = Color.FromArgb(191, 62, 255);
                    break;
                case 5:
                case 6:
                case 7:
                    color = Color.FromArgb(0, 0, 238);
                    break;
            }
            return color;
        }

        /// <summary>
        /// 通过消息ID获取传感器ID与实际的MessageId_0
        /// </summary>
        /// <param name="messageid">输入的消息ID</param>
        /// <param name="messageid_0">实际的MessageId_0（排除掉传感器ID）</param>
        /// <returns>返回传感器ID（0~7）</returns>
        public static byte GetSensorIdByMessageId(int messageid, out int messageid_0)
        {
            byte result = (byte)((messageid % 0x100) / 0x10);
            messageid_0 = messageid - result * 0x10;
            return result;
        }

        /// <summary>
        /// 获取给定文件地址的数据采集文件内容（目前仅支持ARS404/ARS408），将所有符合格式的帧梳理出来并写入到一个新的文件中
        /// <para/>文件名添加后缀_new，假如有时间格式为yyyy-MM-dd HH:mm:ss.fff的字符串则原样保留
        /// </summary>
        /// <param name="filePath">包含雷达数据的采集文件的完整路径</param>
        /// <exception cref="ArgumentNullException">给定的文件路径为null或空白字符串</exception>
        /// <exception cref="ArgumentException">给定的文件路径所对应的文件不存在</exception>
        public static void FilterRadarDataInFile(string filePath)
        {
            FilterRadarDataInFile(filePath, out _);
        }

        /// <summary>
        /// 获取给定文件地址的数据采集文件内容（目前仅支持ARS404/ARS408），将所有符合格式的帧梳理出来并写入到一个新的文件中
        /// <para/>文件名添加后缀_new，假如有时间格式为yyyy-MM-dd HH:mm:ss.fff的字符串则原样保留
        /// </summary>
        /// <param name="filePath">包含雷达数据的采集文件的完整路径</param>
        /// <param name="newFilePath">整理后的采集数据所保存到的新文件路径</param>
        /// <exception cref="ArgumentNullException">给定的文件路径为null或空白字符串</exception>
        /// <exception cref="ArgumentException">给定的文件路径所对应的文件不存在</exception>
        public static void FilterRadarDataInFile(string filePath, out string newFilePath)
        {
            newFilePath = string.Empty;
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath), "给定的文件地址为空");
            if (!File.Exists(filePath))
                throw new ArgumentException(nameof(filePath), "给定的文件不存在");
            var fileInfo = new FileInfo(filePath);
            var lines = File.ReadAllLines(filePath);
            newFilePath = fileInfo.FullName.Remove(fileInfo.FullName.Length - fileInfo.Extension.Length) + "_new" + fileInfo.Extension;
            Regex pattern = new Regex(ArsConst.Pattern_SensorMessage, RegexOptions.Compiled);
            foreach (var line in lines)
            {
                bool isDateTime = DateTime.TryParseExact(line, "yyyy-MM-dd HH:mm:ss.fff", new CultureInfo("zh-CN"), DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out DateTime result);
                string toWrite = line;
                if (isDateTime)
                    goto END_OF_LOOPCONTENT;
                MatchCollection col = pattern.Matches(line);
                toWrite = string.Join(" ", col.Cast<Match>().Select(match => match.Value));
            END_OF_LOOPCONTENT:
                File.AppendAllLines(newFilePath, new string[] { toWrite });
            }
        }
    }

    /// <summary>
    /// 匿名类
    /// </summary>
    public class Anonymous
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Dist { get; set; }
    }
}
