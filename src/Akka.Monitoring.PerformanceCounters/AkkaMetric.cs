using System.Diagnostics;

namespace Akka.Monitoring.PerformanceCounters
{
    internal abstract class AkkaMetric
    {
        protected AkkaMetric(string name, string categoryName)
        {
            Name = name;
            CategoryName = categoryName;
        }

        public string Name { get; }

        public string CategoryName { get; }

        abstract public void RegisterIn(CounterCreationDataCollection collection);
    }
}