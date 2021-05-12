using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    public class AlertsSourceConfig
    {
        public TimeSpan RegularPollingFrequency { get; set; }
        public TimeSpan FastPollingFrequency { get; set; }
        public string Uri { get; set; }
        public AlertsFormat Format { get; set; }
    }
}
