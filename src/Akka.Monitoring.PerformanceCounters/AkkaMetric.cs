using System.Diagnostics;

namespace Akka.Monitoring.PerformanceCounters
{
    internal abstract class AkkaMetric
    {
        private readonly string _name;

        protected AkkaMetric(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        abstract public void RegisterIn(CounterCreationDataCollection collection);
    }
}