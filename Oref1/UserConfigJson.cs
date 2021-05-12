using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    public class UserConfigJson
    {
        public string Version { get; set; }
        public UserCityConfig[] CityConfigs { get; set; }
        public bool? ShowConnectionNotifications { get; set; }
        public bool? ShowAlertsFromUnknownAreas { get; set; }
    }
}
