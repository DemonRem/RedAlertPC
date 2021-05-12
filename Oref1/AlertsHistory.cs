using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

namespace Oref1
{
    //binary file structure: area,area,area,area...,"",begin/end,datetime

    public class AlertsHistory
    {
        private BinaryWriter _writer;
        private BinaryReader _reader;


        public void WriteAlertsStarted(AlertsEventArgs alertsEventArgs)
        {
            WriteAlerts(alertsEventArgs, true);
        }

        public void WriteAlertsEnded(AlertsEventArgs alertsEventArgs)
        {
            WriteAlerts(alertsEventArgs, false);
        }

        private void WriteAlerts(AlertsEventArgs alertsEventArgs, bool isStarted)
        {
            foreach (string area in alertsEventArgs.Alerts.Where(area2 => !string.IsNullOrEmpty(area2)))
            {
                _writer.Write(area);
            }

            _writer.Write(string.Empty);

            _writer.Write(isStarted);
            _writer.Write(alertsEventArgs.DateTime.ToBinary());
        }

        public IEnumerable<AlertsEventArgs> ReadAlerts()
        {
            while (true)
            {
                yield return ReadAlertsRecord();
            }
        }

        private AlertsEventArgs ReadAlertsRecord()
        {
            List<string> list = new List<string>();

            string id;

            do
            {
                id = _reader.ReadString();

                if (id != string.Empty)
                {
                    list.Add(id);
                }
            }
            while (id != string.Empty);

            bool isStarted = _reader.ReadBoolean();
            DateTime dateTime = DateTime.FromBinary(_reader.ReadInt64());

            return new AlertsEventArgs(new ReadOnlyCollection<string>(list), dateTime);
        }
    }
}
