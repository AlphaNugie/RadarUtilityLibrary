using ArsLibrary.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityExample.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class RadarExample : RadarBase
    {
        //public new int Id { get; set; }

        public void RefreshId()
        {
            //base.Id = 3;
            Id = 44;
        }

        public override void RefreshRcsLimits()
        {
            //throw new NotImplementedException();
        }

        protected override DataFrameBase GetDataFrameEntity()
        {
            return new DataFrameExample(this);
            //throw new NotImplementedException();
        }

        protected override string GetRadarSignals()
        {
            return string.Empty;
            //throw new NotImplementedException();
        }

        protected override void InitFieldValues()
        {
            //throw new NotImplementedException();
        }

        protected override bool ProcessDistanceValue(ref double value)
        {
            return false;
            //throw new NotImplementedException();
        }
    }
}
