using System;
using Akka.Monitoring.Impl;
using JustEat.StatsD;

namespace Akka.Monitoring.StatsD
{
    /// <summary>
    /// StatsD (https://github.com/etsy/statsd) implementation of a <see cref="IActorMonitoringClient"/>.
    /// Strong performance and low overhead. Only a single instance of this monitor can be active in a given process.
    /// </summary>
    public class ActorStatsDMonitor : AbstractActorMonitoringClient
    {
        private readonly StatsDPublisher _publisher;

        public ActorStatsDMonitor(string host, int port = 8125, string prefix = "")
        {
            _publisher = new StatsDPublisher(new StatsDConfiguration
            {
                Host = host,
                Port = port,
                Prefix = prefix
            });
        }

        public override void UpdateCounter(string metricName, int delta, double sampleRate)
        {
            _publisher.Increment(delta, sampleRate, metricName);
        }

        public override void UpdateTiming(string metricName, long time, double sampleRate)
        {
            _publisher.Timing(time, sampleRate, metricName);
        }

        public override void UpdateGauge(string metricName, int value, double sampleRate)
        {
            _publisher.Gauge(value, metricName);
        }

        //Unique name used to enforce a single instance of this client
        private static readonly Guid MonitorName = new Guid("0AD90A54-DB5C-4CAB-B381-025CCEA6AA3B");

        public override int MonitoringClientId => MonitorName.GetHashCode();

        public override void DisposeInternal()
        {
            _publisher.Dispose();
        }
    }
}
