using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.IO;

namespace Oref1
{
    public abstract class JsonAlertsSource : IAlertsSource
    {
        private JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public JsonAlertsSource()
        {
            
        }

        #region IAlertsSource Members

        public IEnumerable<string> GetCurrentAlerts()
        {
            string jsonString = GetCurrentAlertsJson();
            //string jsonString = File.ReadAllText(@"D:\OrefHistory\www.oref.org.il\2014_08_10 23_45_03.1931467.json");

            Trace.WriteLine(jsonString);

            AlertsJson alertsJson = _serializer.Deserialize<AlertsJson>(jsonString);

            return alertsJson.data.ProcessAreaStrings();
        }

        protected abstract string GetCurrentAlertsJson();

        #endregion
    }
}
