using System;
using Akka.Monitoring.Impl;
using StatsdClient;

namespace Akka.Monitoring.StatsD
{
    /// <summary>
    /// StatsD (https://github.com/etsy/statsd) implementation of a <see cref="IActorMonitoringClient"/>.
    /// Strong performance and low overhead. Only a single instance of this monitor can be active in a given process.
    /// </summary>
    public class ActorStatsDMonitor : AbstractActorMonitoringClient
    {
        public ActorStatsDMonitor(string host, int port = 8125, string prefix = "")
        {
            Metrics.Configure(new MetricsConfig
            {
                StatsdServerName = host,
                StatsdServerPort = port,
                Prefix = prefix
            });
        }

        public override void UpdateCounter(string metricName, int delta, double sampleRate)
        {
            Metrics.Counter(metricName, delta, sampleRate);
        }

        public override void UpdateTiming(string metricName, long time, double sampleRate)
        {
            Metrics.Timer(metricName, (int)time, sampleRate);
        }

        public override void UpdateGauge(string metricName, int value, double sampleRate)
        {
            Metrics.GaugeAbsoluteValue(metricName, value);
        }

        //Unique name used to enforce a single instance of this client
        private static readonly Guid MonitorName = new Guid("0AD90A54-DB5C-4CAB-B381-025CCEA6AA3B");

        public override int MonitoringClientId
        {
            get { return MonitorName.GetHashCode(); }
        }

        public override void DisposeInternal()
        {
            //Do nothing
        }
    }
}
