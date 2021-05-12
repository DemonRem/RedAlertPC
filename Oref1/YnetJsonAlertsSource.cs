using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace Oref1
{
    public class YnetJsonAlertsSource : IAlertsSource
    {
        private static readonly Regex _jsonpRegex = new Regex(@"^jsonCallback\((.+)\);$", RegexOptions.Singleline);

        private JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private ConnectionManager _connectionManager;

        public YnetJsonAlertsSource(Uri uri, IPAddress ip)
        {
            _connectionManager = new ConnectionManager(uri, ip);
        }

        #region IAlertsSource Members

        public IEnumerable<string> GetCurrentAlerts()
        {
            string jsonString = GetCurrentAlertsJson();

            Trace.WriteLine(jsonString);

            if (jsonString == @"{""alerts"":""""}")
            {
                return Enumerable.Empty<string>();
            }
            else
            {
                try
                {
                    YnetAlertJson alertsJson = _serializer.Deserialize<YnetAlertJson>(jsonString);

                    return alertsJson.alerts.items
                        .Select(wrapper => wrapper.item.title)
                        .ProcessAreaStrings();

                }
                catch (Exception ex)
                {

                }

                try
                {
                    YnetAlertJson2 alertsJson2 = _serializer.Deserialize<YnetAlertJson2>(jsonString);

                    return alertsJson2.alerts.items.item.title.ProcessAreaString();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());

                    throw;
                }
            }
        }

        protected string GetCurrentAlertsJson()
        {
            byte[] downloadedData = _connectionManager.DownloadData();

            string jsonpString = new StreamReader(new MemoryStream(downloadedData, false), true).ReadToEnd();

            //jsonpString = File.ReadAllText(@"Z:\danny\OrefYnet\22_07_2014 18_36_15.0394770.json");

            return _jsonpRegex.Replace(jsonpString, "$1");
        }

        #endregion
    }
}
