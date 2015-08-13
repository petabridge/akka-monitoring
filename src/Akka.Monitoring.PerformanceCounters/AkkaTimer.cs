using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Akka.Monitoring.PerformanceCounters
{
    internal class AkkaTimer : AkkaMetric
    {
        private readonly ConcurrentDictionary<string, Tuple<PerformanceCounter, PerformanceCounter>>
            _performanceCounters = new ConcurrentDictionary<string, Tuple<PerformanceCounter, PerformanceCounter>>();

        public AkkaTimer(string name)
            : base(name)
        {
        }

        public override void RegisterIn(CounterCreationDataCollection collection)
        {
            var averageTimer = new CounterCreationData
            {
                CounterType = PerformanceCounterType.AverageTimer32,
                CounterName = Name,
            };

            var averageTimerBase = new CounterCreationData
            {
                CounterType = PerformanceCounterType.AverageBase,
                CounterName = Name + "Base",
            };
            collection.Add(averageTimer);
            collection.Add(averageTimerBase);
        }

        public void Update(string instanceName, long value)
        {
            if (!_performanceCounters.ContainsKey(instanceName))
            {
                _performanceCounters.TryAdd(instanceName, Tuple.Create(
                    new PerformanceCounter(ActorPerformanceCountersMonitor.PerformanceCountersCategoryName, Name,
                        instanceName, false),
                    new PerformanceCounter(ActorPerformanceCountersMonitor.PerformanceCountersCategoryName,
                        Name + "Base", instanceName, false)
                    ));
            }
            _performanceCounters[instanceName].Item1.IncrementBy(value);
            _performanceCounters[instanceName].Item2.Increment();
        }
    }
}