﻿using System;
using System.Collections.Generic;
using Akka.Monitoring.Impl;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Akka.Monitoring.ApplicationInsights
{
    public class ActorAppInsightsMonitor : AbstractActorMonitoringClient
    {
        private readonly TelemetryClient _client;

        public ActorAppInsightsMonitor(
            string instrumentationKey,
            bool developerMode = false)
        {
            TelemetryConfiguration.Active.InstrumentationKey = instrumentationKey;
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = developerMode;
            _client = new TelemetryClient(TelemetryConfiguration.Active)
            {
                InstrumentationKey = instrumentationKey
            };

#if NET45
            _client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
#else
            _client.Context.Device.OperatingSystem = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
#endif
    }

        public ActorAppInsightsMonitor(TelemetryClient client)
        {
            _client = client;
        }

        public override void UpdateCounter(string metricName, int delta, double sampleRate)
        {
            var properties = new Dictionary<string, string>
            {
                { "sampleRate", sampleRate.ToString() }
            };
           
            _client.TrackMetric(metricName, delta, properties);
        }

        public override void UpdateTiming(string metricName, long time, double sampleRate)
        {
            var properties = new Dictionary<string, string>
                {{ "sampleRate", sampleRate.ToString() }};

            _client.TrackMetric(metricName, time, properties);
        }

        public override void UpdateGauge(string metricName, int value, double sampleRate)
        {
            var properties = new Dictionary<string, string>
                {{ "sampleRate", sampleRate.ToString() }};

            _client.TrackMetric(metricName, value, properties);
        }

        //Unique name used to enforce a single instance of this client
        private static readonly Guid MonitorName = 
            new Guid("b5b96e2c-6d65-46f5-a9e4-8c1974a1e858");

        public override int MonitoringClientId
        {
            get { return MonitorName.GetHashCode(); }
        }

        public override void DisposeInternal()
        {
            _client.Flush();
        }
    }
}
