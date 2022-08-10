using ArsLibrary.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilityExample.Model;

namespace UtilityExample
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region test
            //RadarExample radar = new RadarExample();
            //string json = JsonConvert.SerializeObject(radar);
            //RadarGroupType type = RadarGroupType.Bucket, type2 = RadarGroupType.Wheel;
            //bool result = type == type2;
            //type = (RadarGroupType)4;
            //DataFrameExample frame = new DataFrameExample(new RadarExample());
            //frame.Refresh();
            #endregion
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
