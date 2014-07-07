using Akka.Actor;
using Akka.Event;

namespace Akka.Monitoring
{
    /// <summary>
    /// <see cref="ActorSystem"/> extension that enables the developer to utilize helpful default performance counters for actor systems
    /// (such as the number <see cref="DeadLetter"/> instances) and define application-specific performance counters that might be relevant.
    /// 
    /// Only one instance of the <see cref="ActorPerformanceCounters"/> extension can be active for a given <see cref="ActorSystem"/>, but it
    /// can be safely used across all actors at any time.
    /// </summary>
    public class ActorPerformanceCounters : IExtension
    {
    }

    public class ActorPerformanceCountersExtension : ExtensionIdProvider<ActorPerformanceCounters>
    {
        public override ActorPerformanceCounters CreateExtension(ActorSystem system)
        {
            throw new System.NotImplementedException();
        }
    }
}
