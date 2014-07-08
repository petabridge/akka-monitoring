using System;
using Akka.Monitoring.Impl;
using NStatsD;

namespace Akka.Monitoring.StatsD
{
    /// <summary>
    /// StatsD (https://github.com/etsy/statsd) implementation of a <see cref="IActorMonitoringClient"/>.
    /// 
    /// Pushes instances data to a single StatsD instance determined via an NStatsD (https://github.com/robbihun/NStatsD.Client) configuration file.
    /// 
    /// Strong performance and low overhead. Only a single instance of this monitor can be active in a given process.
    /// </summary>
    public class ActorStatsDMonitor : AbstractActorMonitoringClient
    {
        public override void UpdateCounter(string metricName, int delta, double sampleRate)
        {
            Client.Current.UpdateStats(metricName, delta, sampleRate);
        }

        public override void UpdateTiming(string metricName, long time, double sampleRate)
        {
            Client.Current.Timing(metricName,time,sampleRate);
        }

        public override void UpdateGauge(string metricName, int value, double sampleRate)
        {
            Client.Current.Gauge(metricName,value,sampleRate);
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
