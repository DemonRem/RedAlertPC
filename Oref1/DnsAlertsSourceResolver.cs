using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace Oref1
{
    public class DnsAlertsSourceResolver : IAlertsSourceResolver
    {
        private Uri _uri;
        private volatile bool _stop;

        public AlertsSourceConfig Config { get; private set; }

        public DnsAlertsSourceResolver(AlertsSourceConfig config)
        {
            _uri = new Uri(config.Uri);
            Config = config;
        }

        #region IAlertsSourceResolver Members

        public event EventHandler<AlertsSourceEventArgs> AlertsSourceCreated;

        public event EventHandler<AlertsSourceEventArgs> AlertsSourceRemoved;

        public void Start()
        {
            Thread thread = new Thread(DnsResolverWorker);
            thread.Name = "DnsAlertsSourceResolverWorkerThread";
            thread.IsBackground = true;
            thread.Start();
        }

        public void Stop()
        {
            _stop = true;
        }

        #endregion

        private void DnsResolverWorker()
        {
            //Dictionary<IPAddress, IAlertsSource> ipsToSource = new Dictionary<IPAddress, IAlertsSource>();
            IPAddress currentIp = null;
            IAlertsSource currentSource = null;

            Random random = new Random();

            while (!_stop)
            {
                try
                {
                    IPAddress[] newIps = Dns.GetHostAddresses(_uri.DnsSafeHost);

                    if (newIps.Length > 0 && !newIps.Contains(currentIp))
                    {
                        IPAddress newIp = newIps[random.Next(newIps.Length)];
                        IAlertsSource newSource;

                        if (Config.Format == AlertsFormat.Ynet)
                        {
                            newSource = new YnetJsonAlertsSource(_uri, newIp);
                        }
                        else
                        {
                            newSource = new HttpIpJsonAlertsSource(_uri, newIp);
                        }

                        RaiseAlertsSourceCreatedEvent(newSource);

                        if (currentSource != null)
                        {
                            RaiseAlertsSourceRemovedEvent(currentSource);
                        }

                        currentIp = newIp;
                        currentSource = newSource;
                    }

                    Thread.Sleep(TimeSpan.FromDays(1));
                    //Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }

            //Dictionary<IPAddress, IAlertsSource> ipsToSource = new Dictionary<IPAddress, IAlertsSource>();

            //while (!_stop)
            //{
            //    try
            //    {
            //        IPAddress[] newIps = Dns.GetHostAddresses(_uri.DnsSafeHost);

            //        IPAddress[] removedIps = ipsToSource.Keys.Except(newIps).ToArray();
            //        IPAddress[] createdIps = newIps.Except(ipsToSource.Keys).ToArray();

            //        foreach (IPAddress ip in createdIps)
            //        {
            //            IAlertsSource source;

            //            if (Config.Format == AlertsFormat.Ynet)
            //            {
            //                source = new YnetJsonAlertsSource(_uri, ip);
            //            }
            //            else
            //            {
            //                source = new HttpIpJsonAlertsSource(_uri, ip);
            //            }

            //            ipsToSource.Add(ip, source);

            //            RaiseAlertsSourceCreatedEvent(source);
            //        }

            //        foreach (IPAddress ip in removedIps)
            //        {
            //            RaiseAlertsSourceRemovedEvent(ipsToSource[ip]);

            //            ipsToSource.Remove(ip);
            //        }

            //        Thread.Sleep(TimeSpan.FromMinutes(15));
            //        //Thread.Sleep(1000);
            //    }
            //    catch (Exception ex)
            //    {
            //        Trace.WriteLine(ex.ToString());
            //    }
            //}
        }

        private void RaiseAlertsSourceCreatedEvent(IAlertsSource source)
        {
            EventHandler<AlertsSourceEventArgs> alertsSourceCreated = AlertsSourceCreated;

            if (alertsSourceCreated != null)
            {
                alertsSourceCreated.Invoke(this, new AlertsSourceEventArgs(source));
            }
        }

        private void RaiseAlertsSourceRemovedEvent(IAlertsSource source)
        {
            EventHandler<AlertsSourceEventArgs> alertsSourceRemoved = AlertsSourceRemoved;

            if (alertsSourceRemoved != null)
            {
                alertsSourceRemoved.Invoke(this, new AlertsSourceEventArgs(source));
            }
        }
    }
}
