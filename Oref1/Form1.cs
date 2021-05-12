using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace Oref1
{
    public partial class Form1 : Form
    {
        private static readonly List<string> _emptyList = new List<string>();

        private AlertsSourcesPoller _alertsSourcesPoller;
        private Dictionary<DnsAlertsSourceResolver, AlertsSourceConfig> _resolvers;
        private Dictionary<IAlertsSource, IAlertsSourcePoller> _sources;
        private SoundPlayer _soundPlayer;
        private bool _playing;
        private Dictionary<string, AreaConfiguration> _areaConfigs;
        private Dictionary<string, UserCityConfig> _cityConfigs;
        private bool _lastNoAlerts = true;
        private UserConfigJson _userCityConfigs;

        private bool _wasConnectedBefore;
        private bool _alertActivated;
        private bool _disconnectionNotificationShowed;

        public Form1()
        {
            InitializeComponent();

            _soundPlayer = new SoundPlayer(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "red_color.wav"));
            //_soundPlayer = new SoundPlayer(@"C:\Windows\Media\Windows Critical Stop.wav"); //debug, to avoid panic around.

            this.Icon = Resources.Logo;
            this.notifyIcon1.Icon = Resources.Logo;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _soundPlayer.LoadAsync();
            
            AlertsSourceConfig[] configs = new AlertsSourceConfig[]
                {
                    new AlertsSourceConfig() 
                    {
                        Uri = "http://www.oref.org.il/WarningMessages/alerts.json",
                        Format = AlertsFormat.Oref,
                        RegularPollingFrequency = TimeSpan.FromMilliseconds(500),
                        FastPollingFrequency = TimeSpan.FromMilliseconds(200)
                    },
                    new AlertsSourceConfig() 
                    {
                        Uri = "http://www.mako.co.il/Collab/amudanan/adom.txt",
                        Format = AlertsFormat.Oref,
                        RegularPollingFrequency = TimeSpan.FromMilliseconds(500),
                        FastPollingFrequency = TimeSpan.FromMilliseconds(200)
                    },
                    new AlertsSourceConfig() 
                    {
                        Uri = "http://alerts.ynet.co.il/alertsrss/YnetPicodeHaorefAlertFiles.js?callback=jsonCallback",
                        Format = AlertsFormat.Ynet,
                        RegularPollingFrequency = TimeSpan.FromMilliseconds(500),
                        FastPollingFrequency = TimeSpan.FromMilliseconds(200)
                    },
                };

            _resolvers = configs.ToDictionary(config => new DnsAlertsSourceResolver(config), config => config);
            _sources = new Dictionary<IAlertsSource, IAlertsSourcePoller>();

            foreach (DnsAlertsSourceResolver resolver in _resolvers.Keys)
            {
                resolver.AlertsSourceCreated += new EventHandler<AlertsSourceEventArgs>(resolver_AlertsSourceCreated);
                resolver.AlertsSourceRemoved += new EventHandler<AlertsSourceEventArgs>(resolver_AlertsSourceRemoved);

                resolver.Start();
            }

            _alertsSourcesPoller = new AlertsSourcesPoller();
            _alertsSourcesPoller.ActiveAlertsChanged += new EventHandler<AlertsEventArgs>(_alerts_ActiveAlertsChanged);
            _alertsSourcesPoller.Connected += new EventHandler(_alerts_Connected);
            _alertsSourcesPoller.Connecting += new EventHandler(_alerts_Connecting);
            _alertsSourcesPoller.Start();

            UserConfiguration.ConfigurationChanged += new EventHandler<ConfigurationChangedEventArgs>(UserConfiguration_ConfigurationChanged);
            LoadUserConfiguration();

            //startup minimized
            this.WindowState = FormWindowState.Minimized;
        }

        private void resolver_AlertsSourceCreated(object sender, AlertsSourceEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeAsync(() => resolver_AlertsSourceCreated(sender, e));
                return;
            }

            AlertsSourceConfig config = _resolvers[sender as DnsAlertsSourceResolver];

            AlertsSourcePoller poller = new AlertsSourcePoller(e.Source, config.RegularPollingFrequency, config.FastPollingFrequency);

            _sources.Add(e.Source, poller);
            _alertsSourcesPoller.Add(poller);
        }

        private void resolver_AlertsSourceRemoved(object sender, AlertsSourceEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeAsync(() => resolver_AlertsSourceRemoved(sender, e));
                return;
            }

            _alertsSourcesPoller.Remove(_sources[e.Source]);
            _sources.Remove(e.Source);
        }

        private void _alerts_Connecting(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeAsync(() => _alerts_Connecting(sender, e));
                return;
            }

            if (!_alertActivated)
            {
                if (_wasConnectedBefore)
                {
                    //NotificationWindowsManager.ShowNotification(TimeSpan.FromSeconds(-1), "אזהרה " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), "החיבור אבד. מתחבר למקור מידע...", NotificationType.Warning);

                    if (!_disconnectionNotificationShowed)
                    {
                        _disconnectionTimer.Enabled = true;
                    }
                }
                else
                {
                    _disconnectionNotificationShowed = true;

                    if (true == _userCityConfigs.ShowConnectionNotifications)
                    {
                        NotificationWindowsManager.ShowNotification(TimeSpan.FromSeconds(-1), "אזהרה " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), "מתחבר למקור מידע...", NotificationType.Warning);
                    }
                }
            }
            else
            {
                _alertUpdateTimer.Enabled = true;
            }
        }

        private void _alerts_Connected(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeAsync(() => _alerts_Connected(sender, e));
                return;
            }

            _alertUpdateTimer.Enabled = false;
            _disconnectionTimer.Enabled = false;

            if (!_alertActivated && _disconnectionNotificationShowed)
            {
                if (true == _userCityConfigs.ShowConnectionNotifications)
                {
                    NotificationWindowsManager.ShowNotification(TimeSpan.FromSeconds(5), "מידע " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), "מחובר למקור מידע.", NotificationType.Info);
                }
            }

            _disconnectionNotificationShowed = false;

            _wasConnectedBefore = true;
        }

        private void UserConfiguration_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            LoadUserConfiguration(e.UserConfig);
        }

        private void LoadUserConfiguration()
        {
            LoadUserConfiguration(UserConfiguration.GetConfiguration());
        }

        private void LoadUserConfiguration(UserConfigJson userCityConfigs)
        {
            if (InvokeRequired)
            {
                InvokeAsync(() => LoadUserConfiguration(userCityConfigs));
                return;
            }

            _userCityConfigs = userCityConfigs;

            Dictionary<string, CityEntry> citiesDictionary = Cities.ListOfCities.ToDictionary(cityEntry => cityEntry.City);

            _areaConfigs = userCityConfigs.CityConfigs.Where(userCityConfig => citiesDictionary.ContainsKey(userCityConfig.City))
                .GroupBy(userCityConfig => citiesDictionary[userCityConfig.City].Id)
                .Select(group => new AreaConfiguration(group.Key,
                                                       group.Any(userCityConfig => userCityConfig.DisplayAlerts),
                                                       group.Any(userCityConfig => userCityConfig.SoundAlerts)))
                .ToDictionary(areaConfig => areaConfig.Area);

            _cityConfigs = userCityConfigs.CityConfigs.ToDictionary(cityConfig => cityConfig.City);

            if (userCityConfigs.Version == "1.0")
            {
                MessageBox.Show("רשימת היישובים עודכנה, אנא וודה נכונות ההגדרות.", "צבע אדום 1.7 by DxCK", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                UserSettings.ShowUserSettings();
            }
        }

        private void _alerts_ActiveAlertsChanged(object sender, AlertsEventArgs e)
        {
            if (InvokeRequired)
            {
                InvokeAsync(() => _alerts_ActiveAlertsChanged(sender, e));
                return;
            }

            AreaConfiguration[] currentAreaConfigs = e.Alerts.Where(area => _areaConfigs.ContainsKey(area))
                .Select(area => _areaConfigs[area])
                .ToArray();

            List<string> unknownAreas;

            if (true == _userCityConfigs.ShowAlertsFromUnknownAreas)
            {
                unknownAreas = e.Alerts.Where(area => !_areaConfigs.ContainsKey(area)).ToList();
            }
            else
            {
                unknownAreas = _emptyList;
            }

            if (currentAreaConfigs.Any(areaConfig => areaConfig.SoundAlerts))
            {
                if (!_playing)
                {
                    _soundPlayer.PlayLooping();
                    _playing = true;
                }
            }
            else
            {
                if (_playing)
                {
                    _soundPlayer.Stop();
                    _playing = false;
                }
            }

            if (currentAreaConfigs.Any(areaConfig => areaConfig.DisplayAlerts) ||
                unknownAreas.Count > 0) // _userCityConfigs.ShowAlertsFromUnknownAreas checked before to create unknownAreas.
            {
                //string alertsText = string.Join("\r\n",
                //    e.Alerts.Select(alert => new
                //    {
                //        Id = alert.Id,
                //        Cities = alert.Cities.Where(city => _cityConfigs.ContainsKey(city) && _cityConfigs[city].DisplayAlerts)
                //                             .OrderByDescending(city => _cityConfigs[city].SoundAlerts).ToList()
                //    })
                //    .Where(alert => alert.Cities.Any(city => _cityConfigs.ContainsKey(city) && _cityConfigs[city].DisplayAlerts))
                //    .OrderByDescending(alert => alert.Cities.Any(city => _cityConfigs[city].SoundAlerts))
                //    .Select(alert => alert.Id + "\r\n" + string.Join(", ", alert.Cities) + "\r\n"));

                string alertsText = string.Join("\r\n",
                    e.Alerts.Select(area => new
                    {
                        Id = area,
                        Cities = Cities.GetCitiesById(area)
                                       .Where(city => _cityConfigs.ContainsKey(city) && _cityConfigs[city].DisplayAlerts)
                                       .OrderByDescending(city => _cityConfigs[city].SoundAlerts).ToList()
                    })
                    .Where(alert => alert.Cities.Any(city => _cityConfigs.ContainsKey(city) && _cityConfigs[city].DisplayAlerts))
                    .OrderByDescending(alert => alert.Cities.Any(city => _cityConfigs[city].SoundAlerts))
                    .Concat(unknownAreas.Select(area => new { Id = area, Cities = _emptyList }))
                    .Select(alert => "*** " + alert.Id + " ***" + ((alert.Cities.Count > 0) ? (" : " + string.Join(", ", alert.Cities) + "\r\n") : "")));

                if (!string.IsNullOrWhiteSpace(alertsText))
                {
                    _alertActivated = true;
                    NotificationWindowsManager.ShowNotification(TimeSpan.FromSeconds(-1), "אזעקות פעילות " + e.DateTime.ToLocalTime().ToString("HH:mm:ss dd/MM/yyyy"), alertsText, NotificationType.Critical);
                    _lastNoAlerts = false;
                }
                else
                {
                    _alertActivated = false;

                    if (!_lastNoAlerts)
                    {
                        NotificationWindowsManager.ShowNotification(TimeSpan.FromSeconds(5), "מידע " + e.DateTime.ToLocalTime().ToString("HH:mm:ss dd/MM/yyyy"), "אין אזעקות רלוונטיות כרגע", NotificationType.Info);
                    }

                    _lastNoAlerts = true;
                }
            }
            else
            {
                _alertActivated = false;

                if (!_lastNoAlerts)
                {
                    NotificationWindowsManager.ShowNotification(TimeSpan.FromSeconds(5), "מידע " + e.DateTime.ToLocalTime().ToString("HH:mm:ss dd/MM/yyyy"), "אין אזעקות רלוונטיות כרגע", NotificationType.Info);
                }

                _lastNoAlerts = true;
            }
        }

        private void InvokeAsync(Action action)
        {
            BeginInvoke((Delegate)action);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Visible = false;
            //notifyIcon1.ShowBalloonTip(1000, "צבע אדום 1.7 by DxCK", "התוכנה פועלת.", ToolTipIcon.Info);
        }

        private void יציאהToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _alertsSourcesPoller.ActiveAlertsChanged -= new EventHandler<AlertsEventArgs>(_alerts_ActiveAlertsChanged);
            this.Close();
        }

        private void הגדרותToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserSettings.ShowUserSettings();
        }

        private void _alertUpdateTimer_Tick(object sender, EventArgs e)
        {
            _alertUpdateTimer.Enabled = false;

            if (_playing)
            {
                _soundPlayer.Stop();
            }

            if (true == _userCityConfigs.ShowConnectionNotifications)
            {
                NotificationWindowsManager.ShowNotification(TimeSpan.FromSeconds(-1), "אזהרה " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), "החיבור אבד. מתחבר למקור מידע...", NotificationType.Warning);
            }
        }

        private void _disconnectionTimer_Tick(object sender, EventArgs e)
        {
            _disconnectionTimer.Enabled = false;
            _disconnectionNotificationShowed = true;

            if (true == _userCityConfigs.ShowConnectionNotifications)
            {
                NotificationWindowsManager.ShowNotification(TimeSpan.FromSeconds(-1), "אזהרה " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), "החיבור אבד. מתחבר למקור מידע...", NotificationType.Warning);
            }
        }
    }
}
