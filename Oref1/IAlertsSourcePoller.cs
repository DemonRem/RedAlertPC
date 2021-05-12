using System;
namespace Oref1
{
    public interface IAlertsSourcePoller
    {
        event EventHandler<AlertsEventArgs> ActiveAlertsChanged;
        event EventHandler Connected;
        event EventHandler Connecting;
        void Start();
        void Stop();
        bool UseFastPollingFrequency { get; set; }
    }
}
