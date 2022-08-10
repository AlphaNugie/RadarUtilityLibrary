using ArsLibrary.Model;
using CommonLib.Extensions;
using CommonLib.Function.Fitting;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// 坐标系图像文件原图
        /// </summary>
        public static Image OriginalImageEmpty = new Bitmap(new MemoryStream(Convert.FromBase64String(ImageBase64String.ARSCoorEmptyBase64)));

        /// <summary>
        /// 坐标系图像文件原图
        /// </summary>
        public static Image OriginalImageDouble = new Bitmap(new MemoryStream(Convert.FromBase64String(ImageBase64String.ARSCoorDoubleBase64)));

        ///// <summary>
        ///// 图片缩小时（或鼠标滑轮向上滑动时）的缩小比例系数，放大时则是倒数（-1次幂）
        ///// </summary>
        //public static float ScrollRatio = (float)0.9;

        ///// <summary>
        ///// 像素与雷达坐标系单位（m）的比值
        ///// </summary>
        //public static double R = 4.12; //实际比例应为8.32 : 2

        ///// <summary>
        ///// 绘点时点的粗细（直径，像素）
        ///// </summary>
        //public static float T = 2;

        /// <summary>
        /// 是否显示已被过滤掉的点
        /// </summary>
        public static bool ShowDesertedPoints { get; set; } = true;
        #endregion

        #region 检测
        /// <summary>
        /// 是否使用公共RCS范围
        /// </summary>
        public static bool UsePublicRcsRange { get; set; } = true;

        /// <summary>
        /// RCS最小值
        /// </summary>
        public static int RcsMinimum { get; set; } = -64;

        /// <summary>
        /// RCS最大值
        /// </summary>
        public static int RcsMaximum { get; set; } = 64;

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

        ///// <summary>
        ///// 迭代距离差的上限，距离差超过此上限则绝不用新值代替当前值（此上限不受迭代是否启用、是否使用TCP模式影响）
        ///// </summary>
        //public static double IteDistUpperLimit { get; set; } = 1000;
        #endregion
    }

    /// <summary>
    /// 公共方法
    /// </summary>
    public class ArsFunc
    {
        #region 数据处理与转换
        #region 平面角度方法版本V2
        /// <summary>
        /// 将所有雷达点拟合为平面，拟合斜率
        /// </summary>
        /// <param name="points">数据源</param>
        /// <param name="xybias">xy坐标校正值，分别为xy</param>
        /// <param name="rangle">左右雷达倾角，分别为左右，向下为正</param>
        /// <param name="dist_ex_count">根据距其它点的距离和来排除的点数目，假如大于等于0小于1，则为排除点的比例（可以不排除，不可以全排除）</param>
        /// <param name="filterCoeff">离群点过滤系数，标准值为1，越小越严格，为0将过滤掉所有点</param>
        /// <param name="extraProc">是否采取额外处理措施（重心斜率相关操作）</param>
        /// <param name="message">输出的消息</param>
        /// <returns>返回处理的角度，角度越大越倾向于出垛</returns>
        public static double GetSurfaceAngleV2(IEnumerable<SensorGeneral> points, double[] xybias, double rangle, double dist_ex_count, double filterCoeff, bool extraProc, out string message)
        {
            double def_angle = 0/*, def_rad = 0*/; //默认角度，默认半径，默认在垛内
            //rad_avr = def_rad;
            if (points == null || points.Count() == 0 || xybias == null || xybias.Length < 2)
            {
                message = "未提供任何点的数据";
                return def_angle;
            }
            double xbias = xybias[0], ybias = xybias[1];
            List<SensorGeneral> source, sourceList = new List<SensorGeneral>();
            source = points.ToList();
            #region 坐标范围过滤
            //排除不在特定坐标范围内以及不在特定角度和半径范围内的点
            foreach (var g in source)
            {
                //纵坐标直接校正，横坐标乘以-1再校正，角度加上俯仰角再加上倾角
                if (!g.HelpFlag)
                {
                    g.HelpFlag = true;
                    g.DistLong += xbias;
                    g.DistLat = g.DistLat * -1 + ybias;
                    double angle = g.Angle + rangle;
                    double x = g.Radius * Math.Cos(angle * Math.PI / 180), y = g.Radius * Math.Sin(angle * Math.PI / 180);
                    g.DistLong = x;
                    g.DistLat = y;
                }
                double xceil = double.MaxValue, yfloor = -5, yceil = 5;
                //假如XY坐标不在特定范围内则排除，坐标范围根据是否在底层而有所区别
                if (!g.DistLong.Between(0, xceil) || !g.DistLat.Between(yfloor, yceil))
                    continue;
                sourceList.Add(g);
            }
            #endregion
            List<SensorGeneral> finalList = new List<SensorGeneral>(); //最终保留点列表
            List<double> listx = new List<double>(), listy = new List<double>(), listz = new List<double>();
            List<Anonymous> checkList = new List<Anonymous>();
            #region 排除距离其它点太远的点
            foreach (var gi in sourceList)
            {
                List<double> listDists = new List<double>();
                foreach (var gj in sourceList)
                    //排除同一个点
                    if (gi != gj)
                        listDists.Add(Math.Sqrt(Math.Pow(gj.DistLong - gi.DistLong, 2) + Math.Pow(gj.DistLat - gi.DistLat, 2) + Math.Pow(gj.PushfCounter - gi.PushfCounter, 2))); //计算与其它所有点的距离并储存
                //找出排序靠前若干位的距离值并取平均值，记为单点距离平均值
                listDists.Sort();
                int ex_count = dist_ex_count >= 0 && dist_ex_count < 1 ? (int)(listDists.Count * dist_ex_count) : (int)dist_ex_count;
                ex_count = ex_count == 0 ? 1 : ex_count; //至少为1
                listDists = listDists.Take(ex_count).ToList();
                if (listDists.Count == 0)
                    continue;
                double dist = listDists.Average();
                checkList.Add(new Anonymous() { Id = gi.Id, Dist = dist });
            }
            double dist_avr = checkList.Count == 0 ? 0 : checkList.Select(a => a.Dist).Average() * filterCoeff; //求所有点的单点距离平均值的平均值，记为全局平均值
            double xsum = 0, ysum = 0, zsum = 0; //所有点XYZ坐标和，以求平均值
            checkList.RemoveAll(a => a.Dist <= dist_avr); //排除所有距离不超过全局平均值的点，剩下的点则为距离较远的点
            //checkList = checkList.OrderByDescending(a => a.Dist).Take(ex_count).ToList();
            foreach (var g in sourceList)
            {
                //排除距离其它点太远的点
                if (checkList.Count(c => c.Id == g.Id) > 0)
                    continue;
                //x列向量为纵向坐标，y列向量为帧序号（清算次数），z列向量为横向坐标
                if (extraProc)
                {
                    finalList.Add(g);
                    xsum += g.DistLong;
                    zsum += g.DistLat;
                    ysum += g.PushfCounter;
                }
                listx.Add(g.DistLong);
                listz.Add(g.DistLat);
                listy.Add(g.PushfCounter);
            }
            #endregion
            //#region 计算平均半径
            ////按半径排序，去除前后各10%的点
            //int part = (int)(sourceList.Count * 0.1);
            //rad_avr = sourceList.Count == 0 ? def_rad : sourceList.OrderBy(g => g.Radius).Select(g => g.Radius).Skip(part).Take(sourceList.Count - part * 2).Average();
            //#endregion
            sourceList.Clear();
            //假如不采取额外处理措施或者所剩点数为0，直接进入最终阶段
            if (!extraProc || listx.Count == 0)
                goto FINAL_STEP;
            #region 额外处理措施
            ////重心坐标，开始寻找距离重心坐标最近的点
            //double xa = xsum / finalList.Count, za = zsum / finalList.Count, ya = ysum / finalList.Count;
            ////将第一个点作为默认最近点，计算初始最近距离
            //SensorGeneral nearest = finalList.First();
            //double distMin = Math.Sqrt(Math.Pow(nearest.DistLong - xa, 2) + Math.Pow(nearest.DistLat - za, 2) + Math.Pow(nearest.PushfCounter - ya, 2));
            ////循环最终列表，各个点之间比较距离重心最近的点
            //foreach (SensorGeneral g in finalList)
            //{
            //    double dist = Math.Sqrt(Math.Pow(g.DistLong - xa, 2) + Math.Pow(g.DistLat - za, 2) + Math.Pow(g.PushfCounter - ya, 2));
            //    if (dist < distMin)
            //    {
            //        distMin = dist;
            //        nearest = g;
            //    }
            //}
            ////List<SensorGeneral> finalList2 = new List<SensorGeneral>();
            //listx.Clear();
            //listz.Clear();
            //listy.Clear();
            //foreach (SensorGeneral g in finalList)
            //{
            //    double k = (g.DistLat - nearest.DistLat) / (g.DistLong - nearest.DistLong); //计算最终列表中每个点
            //    if (k < 0)
            //        continue;
            //    //finalList2.Add(g);
            //    listx.Add(g.DistLong);
            //    listz.Add(g.DistLat);
            //    listy.Add(g.PushfCounter);
            //}
            #endregion

            FINAL_STEP:
            double[] results = SurfaceFitting.GetSurceCoefficients(listx, listy, listz, out message);
            return results == null || results.Length == 0 ? def_angle : Math.Atan(1 / Math.Abs(results[0])) * 180 / Math.PI; //越高代表离垛边越靠近
        }
        #endregion
        #endregion

        #region 功能
        /// <summary>
        /// 根据给定的行走方向，俯仰方向，回转方向判断臂架运动方向
        /// </summary>
        /// <param name="walkDir">行走方向</param>
        /// <param name="yawDir">回转方向</param>
        /// <returns></returns>
        public static Directions GetMovingDirection(Directions walkDir, /*Directions pitchDir, */Directions yawDir)
        {
            Directions result = Directions.None;
            //只当行走与回转方向不相反时给出一个确定的左右方向
            if ((walkDir == Directions.Left && yawDir == Directions.None) || (walkDir == Directions.Left && yawDir == Directions.Left) || (walkDir == Directions.None && yawDir == Directions.Left))
                result = Directions.Left;
            else if ((walkDir == Directions.Right && yawDir == Directions.None) || (walkDir == Directions.Right && yawDir == Directions.Right) || (walkDir == Directions.None && yawDir == Directions.Right))
                result = Directions.Right;
            ////假如
            //if (result != Directions.Left && result != Directions.Right)
            //    result = pitchDir;
            return result;
        }

        /// <summary>
        /// 通过消息ID获取传感器ID与实际的MessageId_0
        /// </summary>
        /// <param name="messageid">消息ID</param>
        /// <param name="messageid_0">实际消息ID（排除传感器ID）</param>
        /// <returns></returns>
        public static byte GetSensorIdByMessageId(int messageid, out int messageid_0)
        {
            byte result = (byte)((messageid % 0x100) / 0x10);
            messageid_0 = messageid - result * 0x10;
            return result;
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
        /// 根据雷达散射截面属性获取颜色
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
        /// 根据目标动态属性获取颜色
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
        #endregion
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
