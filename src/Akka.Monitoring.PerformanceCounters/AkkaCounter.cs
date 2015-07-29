using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Akka.Monitoring.PerformanceCounters
{
    internal class AkkaCounter : AkkaMetric
    {
        readonly ConcurrentDictionary<string, Tuple<PerformanceCounter, PerformanceCounter>> _performanceCounters =
            new ConcurrentDictionary<string, Tuple<PerformanceCounter, PerformanceCounter>>();
        public AkkaCounter(string name)
            :base(name)
        {            
        }        

        public override void RegisterIn(CounterCreationDataCollection collection)
        {
            var numberOfItems = new CounterCreationData
            {
                CounterType = PerformanceCounterType.NumberOfItems64,
                CounterName = Name,
            };

            var rates = new CounterCreationData
            {
                CounterType = PerformanceCounterType.RateOfCountsPerSecond64,
                CounterName = RatePerformanceCounterName(Name),
            };
            collection.Add(numberOfItems);
            collection.Add(rates);
        }

        public void Update(string instanceName, int delta)
        {
            if (!_performanceCounters.ContainsKey(instanceName))
            {
                _performanceCounters.TryAdd(instanceName, Tuple.Create(
                    new PerformanceCounter(ActorPerformanceCountersMonitor.PerformanceCountersCategoryName, Name,
                        instanceName, false),
                    new PerformanceCounter(ActorPerformanceCountersMonitor.PerformanceCountersCategoryName,
                        RatePerformanceCounterName(Name), instanceName, false)));
            }

            _performanceCounters[instanceName].Item1.IncrementBy(delta);
            _performanceCounters[instanceName].Item2.IncrementBy(delta);
        }

        private static string RatePerformanceCounterName(string counterName)
        {
            return string.Format("{0} /sec", counterName);
        }
    }
}