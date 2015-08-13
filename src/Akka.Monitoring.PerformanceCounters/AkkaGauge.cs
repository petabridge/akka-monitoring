using System.Collections.Concurrent;
using System.Diagnostics;

namespace Akka.Monitoring.PerformanceCounters
{
    internal class AkkaGauge : AkkaMetric
    {
        readonly ConcurrentDictionary<string, PerformanceCounter> _performanceCounters =
            new ConcurrentDictionary<string, PerformanceCounter>();
        public AkkaGauge(string name) : base(name)
        {
        }

        public override void RegisterIn(CounterCreationDataCollection collection)
        {
            var numberOfItems = new CounterCreationData
            {
                CounterType = PerformanceCounterType.NumberOfItems64,
                CounterName = Name,
            };
            
            collection.Add(numberOfItems);            
        }

        public void Update(string instanceName, int value)
        {
            if (!_performanceCounters.ContainsKey(instanceName))
            {
                _performanceCounters.TryAdd(instanceName,
                    new PerformanceCounter(ActorPerformanceCountersMonitor.PerformanceCountersCategoryName, Name, instanceName,
                        false));
            }
            _performanceCounters[instanceName].RawValue = value;
        }
    }
}