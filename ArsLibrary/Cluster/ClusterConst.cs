using CommonLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArsLibrary.Cluster
{
    /// <summary>
    /// 聚类（DBSCAN）相关配置常量，来自配置文件的[Cluster]段
    /// 由业务项目在刷新配置时调用Refresh方法持续刷新，对应原Python防碰项目group_config.yaml中的滤波/聚类/判定参数
    /// </summary>
    public class ClusterConst
    {
        /// <summary>
        /// 配置文件中的段名
        /// </summary>
        public const string SectionName = "Cluster";

        #region 开关

        /// <summary>
        /// 聚类防碰总开关（全局），为false时所有雷达均不启用聚类（无论雷达自身开关如何）
        /// </summary>
        public static bool Enabled { get; set; } = false;

        /// <summary>
        /// 是否启用统计离群点去除（SOR）
        /// 启用后强制关闭雷达原有的排除离散点（RemoveDistantPoints），二者不可叠加
        /// </summary>
        public static bool SorEnabled { get; set; } = false;

        /// <summary>
        /// 是否启用本体（臂架）点剔除，即首帧自学习包围盒、后续帧剔除盒内的点
        /// 默认关闭；关闭时首帧不再用于预热，直接进入正常处理
        /// </summary>
        public static bool ArmBoxEnabled { get; set; } = false;

        /// <summary>
        /// 是否启用报警跳变防抖（等级直接跳至最高级时抑制输出）
        /// </summary>
        public static bool AntiBounceEnabled { get; set; } = true;

        /// <summary>
        /// 是否启用高度中心筛选（ARS408无高度信息，默认关闭）
        /// </summary>
        public static bool HeartZFilterEnabled { get; set; } = false;

        /// <summary>
        /// 是否输出聚类调试日志
        /// </summary>
        public static bool DebugEnabled { get; set; } = false;

        #endregion

        #region 滤波参数

        /// <summary>
        /// SOR近邻数量
        /// </summary>
        public static int SorK { get; set; } = 10;

        /// <summary>
        /// SOR标准差倍数，平均近邻距离超过 均值+nSigma*标准差 的点判为离群点
        /// </summary>
        public static double SorNSigma { get; set; } = 2.0;

        /// <summary>
        /// 参与聚类的特征列（对应RadarPoint的列号：0-X 1-Y 2-Z 3-Vx 4-Vy 5-Vz 6-Rcs）
        /// 默认仅用纵向与横向坐标；ARS408下第2列（Z）与第5列（Vz）恒为0，不应选用
        /// </summary>
        public static int[] ClusterCols { get; set; } = new int[] { 0, 1 };

        /// <summary>
        /// 特征权重，与ClusterCols一一对应，个数不一致时按1处理
        /// </summary>
        public static double[] FeatureWeights { get; set; } = new double[] { 1, 1.5 };

        #endregion

        #region 聚类参数

        /// <summary>
        /// 首帧聚类时的k近邻数量（用于自适应eps）
        /// </summary>
        public static int KInit { get; set; } = 5;

        /// <summary>
        /// 首帧聚类的最小样本数
        /// </summary>
        public static int MinSamplesInit { get; set; } = 5;

        /// <summary>
        /// 后续帧聚类的k近邻数量（用于自适应eps）
        /// </summary>
        public static int KDynamic { get; set; } = 4;

        /// <summary>
        /// 后续帧聚类的最小样本数
        /// </summary>
        public static int MinSamplesDynamic { get; set; } = 5;

        /// <summary>
        /// 稀疏簇过滤系数：簇内平均最近邻距离大于 SparseRatio*eps 时整簇判为噪声
        /// </summary>
        public static double SparseRatio { get; set; } = 1.5;

        /// <summary>
        /// 簇内离群点剔除系数：簇内最近邻距离大于 IntraRatio*eps 的点判回噪声
        /// </summary>
        public static double IntraRatio { get; set; } = 0.7;

        /// <summary>
        /// 单周期参与聚类的点数上限，超过则跳过本周期（防止O(N^2)开销过大）
        /// </summary>
        public static int MaxPoints { get; set; } = 300;

        #endregion

        #region 判定与跟踪参数

        /// <summary>
        /// 距离标定偏置，簇距离L减去此值（雷达自身的Offset已在Radar.CurrentDistance输出侧补偿，此处默认0）
        /// </summary>
        public static double LOffset { get; set; } = 0;

        /// <summary>
        /// 有效簇的最小点数
        /// </summary>
        public static int MinClusterPoints { get; set; } = 2;

        /// <summary>
        /// 高度中心下限
        /// </summary>
        public static double HeartZLow { get; set; } = -1;

        /// <summary>
        /// 高度中心上限
        /// </summary>
        public static double HeartZHigh { get; set; } = 0;

        /// <summary>
        /// 跨帧跟踪的关联距离阈值
        /// </summary>
        public static double TrackMaxDistance { get; set; } = 3.0;

        /// <summary>
        /// 跨帧跟踪的最大丢失周期数
        /// </summary>
        public static int TrackMaxLostFrames { get; set; } = 5;

        /// <summary>
        /// 跳变防抖所针对的报警等级（达到此等级且前一等级过低时抑制输出）
        /// </summary>
        public static int AntiBounceLevel { get; set; } = 3;

        /// <summary>
        /// 本体（臂架）盒判定：簇横向中心绝对值的上限
        /// </summary>
        public static double BodyBoxMeanYLimit { get; set; } = 2;

        /// <summary>
        /// 本体（臂架）盒判定：簇纵向最小值的上限
        /// </summary>
        public static double BodyBoxXMinLimit { get; set; } = 20;

        #endregion

        #region 结果显示（画面上的聚类包围框）

        /// <summary>
        /// 是否在雷达画面上绘制聚类结果的包围框（实际绘制还需Enabled与雷达自身的聚类开关同时为真）
        /// </summary>
        public static bool ClusterBoxEnabled { get; set; } = true;

        /// <summary>
        /// 框选形状
        /// </summary>
        public static ClusterBoxShape BoxShape { get; set; } = ClusterBoxShape.ConvexHull;

        /// <summary>
        /// 着色方式：0-按报警距离L的危险程度 1-按跨帧跟踪ID的稳定色轮
        /// </summary>
        public static int BoxColorMode { get; set; } = 0;

        /// <summary>
        /// 判定为危险（红色）的报警距离L上限，米
        /// </summary>
        public static double BoxDangerDistance { get; set; } = 3.0;

        /// <summary>
        /// 判定为注意（橙色）的报警距离L上限，米
        /// </summary>
        public static double BoxWarnDistance { get; set; } = 10.0;

        /// <summary>
        /// 框线宽度，像素（不随缩放变化）
        /// </summary>
        public static float BoxLineWidth { get; set; } = 2f;

        /// <summary>
        /// 框内半透明填充的不透明度，0-255，0为不填充
        /// </summary>
        public static int BoxFillAlpha { get; set; } = 30;

        /// <summary>
        /// 是否显示标签文字（T跟踪ID L报警距离 n点数）
        /// </summary>
        public static bool BoxShowLabel { get; set; } = true;

        /// <summary>
        /// 是否只绘制有效簇（IsValid为假的簇不绘制）
        /// </summary>
        public static bool BoxOnlyValid { get; set; } = false;

        /// <summary>
        /// 是否将未归入任何簇的噪声点以暗色小点标出
        /// </summary>
        public static bool BoxNoiseDots { get; set; } = false;

        /// <summary>
        /// 是否以加粗描边与外圈光晕突出显示最危险簇
        /// </summary>
        public static bool BoxEmphasizeBest { get; set; } = true;

        #endregion

        /// <summary>
        /// 获取与ClusterCols一一对应的特征权重数组，个数不一致时返回全1数组
        /// </summary>
        /// <returns>权重数组</returns>
        public static double[] GetAlignedWeights()
        {
            int[] cols = ClusterCols;
            double[] weights = FeatureWeights;
            if (cols == null || cols.Length == 0)
                return new double[0];
            if (weights == null || weights.Length != cols.Length)
                return Enumerable.Repeat(1.0, cols.Length).ToArray();
            return weights;
        }

        /// <summary>
        /// 判断给定列号是否为有效特征列
        /// </summary>
        /// <param name="col">列号</param>
        /// <returns>是否有效</returns>
        public static bool IsValidCol(int col)
        {
            return col >= 0 && col <= 6;
        }

        /// <summary>
        /// 从配置文件读取全部聚类相关参数，对应配置文件的[Cluster]段
        /// </summary>
        /// <param name="iniHelper">配置文件读取工具</param>
        public static void Refresh(IniFileHelper iniHelper)
        {
            if (iniHelper == null)
                return;

            try
            {
                int valuei;
                double valued;
                bool valueb;

                #region 开关
                if (iniHelper.TryReadValue(SectionName, "Enabled", false, out valueb))
                    Enabled = valueb;
                if (iniHelper.TryReadValue(SectionName, "SorEnabled", false, out valueb))
                    SorEnabled = valueb;
                if (iniHelper.TryReadValue(SectionName, "ArmBoxEnabled", false, out valueb))
                    ArmBoxEnabled = valueb;
                if (iniHelper.TryReadValue(SectionName, "AntiBounceEnabled", true, out valueb))
                    AntiBounceEnabled = valueb;
                if (iniHelper.TryReadValue(SectionName, "HeartZFilterEnabled", false, out valueb))
                    HeartZFilterEnabled = valueb;
                if (iniHelper.TryReadValue(SectionName, "DebugEnabled", false, out valueb))
                    DebugEnabled = valueb;
                #endregion

                #region 滤波参数
                if (iniHelper.TryReadValue(SectionName, "SorK", 10, out valuei))
                    SorK = valuei;
                if (iniHelper.TryReadValue(SectionName, "SorNSigma", 2.0, out valued))
                    SorNSigma = valued;
                #endregion

                #region 聚类参数
                if (iniHelper.TryReadValue(SectionName, "KInit", 5, out valuei))
                    KInit = valuei;
                if (iniHelper.TryReadValue(SectionName, "MinSamplesInit", 5, out valuei))
                    MinSamplesInit = valuei;
                if (iniHelper.TryReadValue(SectionName, "KDynamic", 4, out valuei))
                    KDynamic = valuei;
                if (iniHelper.TryReadValue(SectionName, "MinSamplesDynamic", 5, out valuei))
                    MinSamplesDynamic = valuei;
                if (iniHelper.TryReadValue(SectionName, "SparseRatio", 1.5, out valued))
                    SparseRatio = valued;
                if (iniHelper.TryReadValue(SectionName, "IntraRatio", 0.7, out valued))
                    IntraRatio = valued;
                if (iniHelper.TryReadValue(SectionName, "MaxPoints", 300, out valuei))
                    MaxPoints = valuei;
                #endregion

                #region 判定与跟踪参数
                if (iniHelper.TryReadValue(SectionName, "LOffset", 0.0, out valued))
                    LOffset = valued;
                if (iniHelper.TryReadValue(SectionName, "MinClusterPoints", 2, out valuei))
                    MinClusterPoints = valuei;
                if (iniHelper.TryReadValue(SectionName, "HeartZLow", -1.0, out valued))
                    HeartZLow = valued;
                if (iniHelper.TryReadValue(SectionName, "HeartZHigh", 0.0, out valued))
                    HeartZHigh = valued;
                if (iniHelper.TryReadValue(SectionName, "TrackMaxDistance", 3.0, out valued))
                    TrackMaxDistance = valued;
                if (iniHelper.TryReadValue(SectionName, "TrackMaxLostFrames", 5, out valuei))
                    TrackMaxLostFrames = valuei;
                if (iniHelper.TryReadValue(SectionName, "AntiBounceLevel", 3, out valuei))
                    AntiBounceLevel = valuei;
                if (iniHelper.TryReadValue(SectionName, "BodyBoxMeanYLimit", 2.0, out valued))
                    BodyBoxMeanYLimit = valued;
                if (iniHelper.TryReadValue(SectionName, "BodyBoxXMinLimit", 20.0, out valued))
                    BodyBoxXMinLimit = valued;
                #endregion

                #region 结果显示
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxEnabled", true, out valueb))
                    ClusterBoxEnabled = valueb;
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxShowLabel", true, out valueb))
                    BoxShowLabel = valueb;
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxOnlyValid", false, out valueb))
                    BoxOnlyValid = valueb;
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxNoiseDots", false, out valueb))
                    BoxNoiseDots = valueb;
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxEmphasizeBest", true, out valueb))
                    BoxEmphasizeBest = valueb;
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxShape", 1, out valuei))
                    BoxShape = (ClusterBoxShape)valuei;
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxColorMode", 0, out valuei))
                    BoxColorMode = valuei;
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxFillAlpha", 30, out valuei))
                    BoxFillAlpha = Math.Min(Math.Max(valuei, 0), 255);
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxLineWidth", 2.0, out valued))
                    BoxLineWidth = (float)Math.Max(valued, 0.5);
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxDangerDistance", 3.0, out valued))
                    BoxDangerDistance = valued;
                if (iniHelper.TryReadValue(SectionName, "ClusterBoxWarnDistance", 10.0, out valued))
                    BoxWarnDistance = valued;
                #endregion

                #region 特征列与权重
                string clusterCols = iniHelper.ReadData(SectionName, "ClusterCols");
                if (!string.IsNullOrWhiteSpace(clusterCols))
                    ClusterCols = clusterCols.Split(',').Select(p => int.Parse(p.Trim())).ToArray();
                string featureWeights = iniHelper.ReadData(SectionName, "FeatureWeights");
                if (!string.IsNullOrWhiteSpace(featureWeights))
                    FeatureWeights = featureWeights.Split(',').Select(p => double.Parse(p.Trim())).ToArray();
                #endregion
            }
            catch (Exception) { } //配置读取失败不应中断刷新线程
        }
    }
}
