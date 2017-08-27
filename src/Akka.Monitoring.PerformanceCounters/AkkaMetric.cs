using System.Diagnostics;

namespace Akka.Monitoring.PerformanceCounters
{
    internal abstract class AkkaMetric
    {
        private readonly string _name;
        private readonly string _categoryName;

        protected AkkaMetric(string name, string categoryName)
        {
            _name = name;
            _categoryName = categoryName;
        }

        public string Name
        {
            get { return _name; }
        }

        public string CategoryName
        {
            get { return _categoryName; }
        }

        abstract public void RegisterIn(CounterCreationDataCollection collection);
    }
}