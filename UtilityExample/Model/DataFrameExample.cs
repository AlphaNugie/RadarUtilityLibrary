using ArsLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityExample.Model
{
    internal class DataFrameExample : DataFrameBase
    {
        ///// <summary>
        ///// 雷达信息对象
        ///// </summary>
        //public new RadarExample Radar { get; set; }

        public DataFrameExample(RadarBase radar) : base(radar)
        {
        }

        public void Refresh()
        {
            int id = Radar.Id;
            Radar.Id = id + 1;
            ((RadarExample)Radar).RefreshId();
            id = Radar.Id;
        }

        public override void UpdateRadarRcsMaxValue()
        {
            throw new NotImplementedException();
        }

        public override void UpdateRadarRcsMinValue()
        {
            throw new NotImplementedException();
        }

        protected override double GetDistCoeff()
        {
            throw new NotImplementedException();
        }

        protected override int GetIteLimitFactor()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessDistanceValue(double value)
        {
            throw new NotImplementedException();
        }

        protected override void ProcessThreatLevelValue(int value)
        {
            throw new NotImplementedException();
        }

        protected override int GetThreatLevel()
        {
            throw new NotImplementedException();
        }

        protected override void CheckFilterFlags(SensorGeneral general)
        {
            throw new NotImplementedException();
        }

        protected override double GetSurfaceAngle()
        {
            throw new NotImplementedException();
        }
    }
}
