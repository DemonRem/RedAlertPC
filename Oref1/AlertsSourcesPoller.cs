using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using DxCK.Utils;

namespace Oref1
{
    public class AlertsSourcesPoller : IAlertsSourcePoller
    {
        private List<IAlertsSourcePoller> _pollers;
        private Dictionary<IAlertsSourcePoller, bool> _isConnectedMultiple;
        private Dictionary<IAlertsSourcePoller, ReadOnlyCollection<string>> _alertsMultiple;
        private bool _isConnectedGlobal = true;
        private string[] _activeAlerts = new string[0];
        private bool _useFastPollingFrequency;
        private bool _started;

        private object _pollersLocker = new object();
        private object _isConnectedMultipleLocker = new object();
        private object _alertsMultipleLocker = new object();

        public AlertsSourcesPoller()
        {
            _pollers = new List<IAlertsSourcePoller>();
            _isConnectedMultiple = new Dictionary<IAlertsSourcePoller, bool>();

            _alertsMultiple = new Dictionary<IAlertsSourcePoller, ReadOnlyCollection<string>>();
        }

        public void Add(IAlertsSourcePoller poller)
        {
            lock (_pollersLocker)
            lock (_isConnectedMultipleLocker)
            lock (_alertsMultipleLocker)
            {
                poller.Connecting += new EventHandler(poller_Connecting);
                poller.Connected += new EventHandler(poller_Connected);
                poller.ActiveAlertsChanged += new EventHandler<AlertsEventArgs>(poller_ActiveAlertsChanged);

                _pollers.Add(poller);
                _alertsMultiple.Add(poller, new ReadOnlyCollection<string>(new string[0]));
                _isConnectedMultiple.Add(poller, false);

                if (_started)
                {
                    poller.Start();
                }
            }
        }

        public void Remove(IAlertsSourcePoller poller)
        {
            lock (_pollersLocker)
            lock (_isConnectedMultipleLocker)
            lock (_alertsMultipleLocker)
            {
                poller.Connecting -= new EventHandler(poller_Connecting);
                poller.Connected -= new EventHandler(poller_Connected);
                poller.ActiveAlertsChanged -= new EventHandler<AlertsEventArgs>(poller_ActiveAlertsChanged);

                _pollers.Remove(poller);
                _alertsMultiple.Remove(poller);
                _isConnectedMultiple.Remove(poller);

                if (_started)
                {
                    poller.Stop();
                }

                UpdateConnectedState();
                UpdateActiveAlerts();
            }
        }

        private void UpdateConnectedState()
        {
            bool isCollectedGlobal = _isConnectedMultiple.Values.Any(x => x);

            if (isCollectedGlobal != _isConnectedGlobal)
            {
                _isConnectedGlobal = isCollectedGlobal;

                if (isCollectedGlobal)
                {
                    RaiseConnectedEvent();
                }
                else
                {
                    RaiseConnectingEvent();
                }
            }
        }

        private void poller_Connecting(object sender, EventArgs e)
        {
            lock (_isConnectedMultipleLocker)
            {
                _isConnectedMultiple[sender as IAlertsSourcePoller] = false;

                UpdateConnectedState();
            }
        }

        private void poller_Connected(object sender, EventArgs e)
        {
            lock (_isConnectedMultipleLocker)
            {
                _isConnectedMultiple[sender as IAlertsSourcePoller] = true;

                UpdateConnectedState();
            }
        }

        private void poller_ActiveAlertsChanged(object sender, AlertsEventArgs e)
        {
            lock (_alertsMultipleLocker)
            {
                _alertsMultiple[sender as IAlertsSourcePoller] = e.Alerts;

                UpdateActiveAlerts();
            }
        }

        private void UpdateActiveAlerts()
        {
            string[] newActiveAlerts = _alertsMultiple.Values.SelectMany(alerts => alerts).Distinct().OrderBy(area => area).ToArray();

            if (!ArrayUtils.Equals<string>(_activeAlerts, newActiveAlerts))
            {
                _activeAlerts = newActiveAlerts;
                RaiseActiveAlertsChangedEvent(_activeAlerts, DateTime.UtcNow);

                UseFastPollingFrequency = _activeAlerts.Length > 0;
            }
        }

        #region IAlertsSourcePoller Members

        public event EventHandler<AlertsEventArgs> ActiveAlertsChanged;

        public event EventHandler Connected;

        public event EventHandler Connecting;

        public void Start()
        {
            lock (_pollersLocker)
            {
                _started = true;

                foreach (IAlertsSourcePoller poller in _pollers)
                {
                    poller.Start();
                }
            }
        }

        public void Stop()
        {
            lock (_pollersLocker)
            {
                _started = false;

                foreach (IAlertsSourcePoller poller in _pollers)
                {
                    poller.Stop();
                }
            }
        }

        public bool UseFastPollingFrequency
        {
            get
            {
                return _useFastPollingFrequency;
            }
            set
            {
                if (_useFastPollingFrequency != value)
                {
                    _useFastPollingFrequency = value;

                    lock (_pollersLocker)
                    {
                        foreach (IAlertsSourcePoller poller in _pollers)
                        {
                            poller.UseFastPollingFrequency = value;
                        }
                    }
                }
            }
        }

        #endregion

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
