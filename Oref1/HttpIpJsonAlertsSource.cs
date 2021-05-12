using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Oref1
{
    public class HttpIpJsonAlertsSource : JsonAlertsSource
    {
        private ConnectionManager _connectionManager;

        public HttpIpJsonAlertsSource(Uri uri, IPAddress ip)
        {
            _connectionManager = new ConnectionManager(uri, ip);
        }

        protected override string GetCurrentAlertsJson()
        {
            byte[] downloadedData = _connectionManager.DownloadData();

            return new StreamReader(new MemoryStream(downloadedData, false), true).ReadToEnd();
        }
    }
}
