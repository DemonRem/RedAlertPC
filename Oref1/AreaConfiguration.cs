using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    public class AreaConfiguration
    {
        public AreaConfiguration(string area, bool displayAlerts, bool soundAlerts)
        {
            Area = area;
            DisplayAlerts = displayAlerts;
            SoundAlerts = soundAlerts;
        }

        public string Area { get; set; }
        public bool DisplayAlerts { get; set; }
        public bool SoundAlerts { get; set; }
    }
}
