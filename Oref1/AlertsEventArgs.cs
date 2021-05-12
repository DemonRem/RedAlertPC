using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Oref1
{
    public class AlertsEventArgs : EventArgs
    {
        public ReadOnlyCollection<string> Alerts { get; set; }
        public DateTime DateTime { get; private set; }

        public AlertsEventArgs(ReadOnlyCollection<string> alerts, DateTime dateTime)
        {
            Alerts = alerts;
            DateTime = dateTime;
        }
    }
}
