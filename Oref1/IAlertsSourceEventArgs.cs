using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    public class AlertsSourceEventArgs : EventArgs
    {
        public AlertsSourceEventArgs(IAlertsSource source)
        {
            Source = source;
        }

        public IAlertsSource Source { get; private set; }
    }
}
