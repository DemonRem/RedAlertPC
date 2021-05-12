using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using DxCK.Utils;
using System.Collections.ObjectModel;

namespace Oref1
{
    public class AlertsSourcePoller : IAlertsSourcePoller
    {
        private IAlertsSource _source;
        private TimeSpan _regularPollingFrequency;
        private TimeSpan _fastPollingFrequency;

        private volatile bool _stop;
        private bool _connected;

        public event EventHandler<AlertsEventArgs> ActiveAlertsChanged;
        public event EventHandler Connecting;
        public event EventHandler Connected;

        private volatile bool _useFastPollingFrequency;

        public bool UseFastPollingFrequency
        {
            get { return _useFastPollingFrequency; }
            set { _useFastPollingFrequency = value; }
        }

        public AlertsSourcePoller(IAlertsSource source, TimeSpan regularPollingFrequency, TimeSpan fastPollingFrequency)
        {
            _source = source;
            _regularPollingFrequency = regularPollingFrequency;
            _fastPollingFrequency = fastPollingFrequency;
        }

        public void Start()
        {
            Thread thread = new Thread(AlertsThreadWork);
            thread.Name = "AlertsSourcePollerWorkerThread";
            thread.IsBackground = true;
            thread.Start();
        }

        public void Stop()
        {
            _stop = true;
        }

        private void AlertsThreadWork()
        {
            Stopwatch sw = new Stopwatch();
            TimeSpan pollingFrequency = _regularPollingFrequency;

            string[] previousAlerts = new string[0];

            char[] comma = new char[] { ',' };

            RaiseConnectingEvent();

            while (!_stop)
            {
                sw.Restart();
                try
                {
                    string[] currentAlerts = _source.GetCurrentAlerts().ToArray();

                    DateTime dateTime = DateTime.UtcNow;

                    if (!_connected)
                    {
                        _connected = true;
                        RaiseConnectedEvent();
                    }

                    if (!ArrayUtils.Equals(currentAlerts, previousAlerts))
                    {
                        RaiseActiveAlertsChangedEvent(currentAlerts, dateTime);

                        previousAlerts = currentAlerts;

                        if (UseFastPollingFrequency)
                        {
                            pollingFrequency = _fastPollingFrequency;
                        }
                        else
                        {
                            pollingFrequency = _regularPollingFrequency;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);

                    _connected = false;
                    RaiseConnectingEvent();
                }

                TimeSpan timeToSleep = pollingFrequency - sw.Elapsed;

                if (_connected && timeToSleep > TimeSpan.Zero)
                {
                    Thread.Sleep(timeToSleep);
                }
            }
        }

        private void RaiseActiveAlertsChangedEvent(string[] alerts, DateTime dateTime)
        {
            ReadOnlyCollection<string> readOnlyAlerts = new ReadOnlyCollection<string>(alerts);

            EventHandler<AlertsEventArgs> activeAlertsChanged = ActiveAlertsChanged;

            if (activeAlertsChanged != null)
            {
                activeAlertsChanged.Invoke(this, new AlertsEventArgs(readOnlyAlerts, dateTime));
            }
        }

        private void RaiseConnectedEvent()
        {
            EventHandler connected = Connected;

            if (connected != null)
            {
                connected.Invoke(this, EventArgs.Empty);
            }
        }

        private void RaiseConnectingEvent()
        {
            EventHandler connecting = Connecting;

            if (connecting != null)
            {
                connecting.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
