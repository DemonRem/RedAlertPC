using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    public class ConfigurationChangedEventArgs : EventArgs
    {
        public ConfigurationChangedEventArgs(UserConfigJson userConfig)
        {
            UserConfig = userConfig;
        }

        public UserConfigJson UserConfig { get; private set; }
    }
}
