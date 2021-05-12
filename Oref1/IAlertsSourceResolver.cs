using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    public interface IAlertsSourceResolver
    {
        event EventHandler<AlertsSourceEventArgs> AlertsSourceCreated;
        event EventHandler<AlertsSourceEventArgs> AlertsSourceRemoved;

        void Start();
        void Stop();
    }
}
