using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    public class UserCityConfig
    {
        public UserCityConfig()
        {

        }

        public UserCityConfig(string city, bool displayAlerts, bool soundAlerts)
        {
            City = city;
            DisplayAlerts = displayAlerts;
            SoundAlerts = soundAlerts;
        }

        public string City { get; set; }
        public bool DisplayAlerts { get; set; }
        public bool SoundAlerts { get; set; }
    }
}
