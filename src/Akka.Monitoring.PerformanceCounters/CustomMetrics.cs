using System.Collections.Generic;

namespace Akka.Monitoring.PerformanceCounters
{
    public class CustomMetrics
    {
        public CustomMetrics()
        {
            Counters = new HashSet<string>();
            Gauges = new HashSet<string>();
            Timers = new HashSet<string>();
        }

        public HashSet<string> Counters { get; private set; }
        public HashSet<string> Gauges { get; private set; }
        public HashSet<string> Timers { get; private set; }
    }
}