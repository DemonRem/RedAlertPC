using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    public interface IAlertsSource
    {
        IEnumerable<string> GetCurrentAlerts();
    }
}
