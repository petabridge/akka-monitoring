using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Akka.Monitoring.Impl;

namespace Akka.Monitoring.PerformanceCounters
{   
    public class ActorPerformanceCountersMonitor : AbstractActorMonitoringClient
    {
        private const string PerformanceCountersCategoryName = "Akka";
        private const string TotalCounterInstanceName = "_Total";
        private static readonly Guid MonitorName = new Guid("F651B9F8-AA38-45BD-BFB9-C5595519C23C");        
        private static readonly HashSet<string> CounterNames = new HashSet<string>(new[]
        {
            Impl.CounterNames.ActorRestarts,
            Impl.CounterNames.ActorsCreated,
            Impl.CounterNames.ActorsStopped,
            Impl.CounterNames.ReceivedMessages,
            Impl.CounterNames.DeadLetters,
            Impl.CounterNames.UnhandledMessages,
            Impl.CounterNames.DebugMessages,
            Impl.CounterNames.InfoMessages,
            Impl.CounterNames.WarningMessages,
            Impl.CounterNames.ErrorMessages,
        });

        readonly ConcurrentDictionary<string, PerformanceCounter> _performanceCounters =
            new ConcurrentDictionary<string, PerformanceCounter>();

        public ActorPerformanceCountersMonitor()
        {
            Init();
        }

        public override void UpdateCounter(string metricName, int delta, double sampleRate)
        {
            string counterName  = null;
            string instanceName = null;
            if (CounterNames.Contains(metricName))
            {
                counterName = metricName;
                instanceName = TotalCounterInstanceName;
            }
            else
            {
                var counterNamePartIndex = metricName.LastIndexOf("akka.",StringComparison.InvariantCulture);

                var counterNamePart = metricName.Substring(counterNamePartIndex);

                if (CounterNames.Contains(counterNamePart))
                {
                    counterName = counterNamePart;
                    instanceName = metricName.Substring(0, counterNamePartIndex-1);
                }
            }
            if (counterName!=null)
            {
                if (!_performanceCounters.ContainsKey(metricName))
                {
                    _performanceCounters.TryAdd(metricName,
                        new PerformanceCounter(PerformanceCountersCategoryName, counterName, instanceName,
                            false));                    
                }                               
                
                var rateMetricKey = RatePerformanceCounterName(metricName);
                if (!_performanceCounters.ContainsKey(rateMetricKey))
                {
                    _performanceCounters.TryAdd(rateMetricKey,
                        new PerformanceCounter(PerformanceCountersCategoryName, RatePerformanceCounterName(counterName), instanceName,false)); 
                }

                _performanceCounters[metricName].IncrementBy(delta);
                _performanceCounters[rateMetricKey].IncrementBy(delta);
            }
            else
            {
                //unkown metric name
            }
        }

        public override void UpdateTiming(string metricName, long time, double sampleRate)
        {
            
        }

        public override void UpdateGauge(string metricName, int value, double sampleRate)
        {
            
        }

        public override int MonitoringClientId
        {
            get { return MonitorName.GetHashCode(); }
        }

        public override void DisposeInternal()
        {            
        }

        private void Init()
        {
            var ccdc = new CounterCreationDataCollection();

            foreach (var name in CounterNames)
            {
                var numberOfItems = new CounterCreationData
                {
                    CounterType = PerformanceCounterType.NumberOfItems64,
                    CounterName = name,
                };

                var rates = new CounterCreationData
                {
                    CounterType = PerformanceCounterType.RateOfCountsPerSecond64,
                    CounterName = RatePerformanceCounterName(name),
                };
                ccdc.Add(numberOfItems);
                ccdc.Add(rates);
            }
            const string categoryName = PerformanceCountersCategoryName;
            if (PerformanceCounterCategory.Exists(categoryName))
                PerformanceCounterCategory.Delete(categoryName);
            PerformanceCounterCategory.Create(categoryName, "",
                PerformanceCounterCategoryType.MultiInstance, ccdc);
        }

        private string RatePerformanceCounterName(string counterName)
        {
            return string.Format("{0} /sec", counterName);
        }
    }
}
