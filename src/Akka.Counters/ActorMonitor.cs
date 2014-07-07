using Akka.Actor;
using Akka.Event;
using Akka.Monitoring.Impl;

namespace Akka.Monitoring
{
    /// <summary>
    /// <see cref="ActorSystem"/> extension that enables the developer to utilize helpful default performance counters for actor systems
    /// (such as the number <see cref="DeadLetter"/> instances) and define application-specific performance counters that might be relevant.
    /// 
    /// Only one instance of the <see cref="ActorMonitor"/> extension can be active for a given <see cref="ActorSystem"/>, but it
    /// can be safely used across all actors at any time.
    /// </summary>
    public class ActorMonitor : IExtension
    {
        internal MonitorRegistry Registry = new MonitorRegistry();

        /// <summary>
        /// Register a new <see cref="AbstractActorMonitoringClient"/> instance to use when monitoring Actor operations.
        /// </summary>
        /// <returns>true if the monitor was succeessfully registered, false otherwise.</returns>
        public bool RegisterMonitor(AbstractActorMonitoringClient client)
        {
            return Registry.AddMonitor(client);
        }

        /// <summary>
        /// Deregister an existing <see cref="AbstractActorMonitoringClient"/> instance so it no longer reports metrics to existing Actors.
        /// </summary>
        /// <returns>true if the monitor was succeessfully deregistered, false otherwise.</returns>
        public bool DeregisterMonitor(AbstractActorMonitoringClient client)
        {
            return Registry.RemoveMonitor(client);
        }

        /// <summary>
        /// Terminates all existing monitors. You can add new ones after this call has been made.
        /// </summary>
        public void TerminateMonitors()
        {
            Registry.DisposeAll();
        }

        /// <summary>
        /// Increment the "Actor Restarts" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementActorRestart(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.TotalActorRestarts);
            if(context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ActorRestartsPerSecond));
        }

        /// <summary>
        /// Increment the "Actors Created" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementActorCreated(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.TotalActors);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ActorsCreatedPerSecond));
        }

        /// <summary>
        /// Increment the "Actors Stopped" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementActorStopped(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.TotalActors, -1);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ActorsStoppedPerSecond));
        }

        /// <summary>
        /// Increment the "Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementMessagesReceived(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.MessagesPerSecond);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.MessagesPerSecond));
        }

        /// <summary>
        /// Increment the "Unhandled Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementUnhandledMessage(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.TotalUnhandledMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.UnhandledMessagesPerSecond));
        }

        /// <summary>
        /// Increment the "Deadletters" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementDeadLetters(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.TotalDeadletters);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.DeadlettersPerSecond));
        }

        /// <summary>
        /// Increment the "Errors" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementErrorsLogged(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.TotalErrorMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ErrorMessagesPerSecond));
        }

        /// <summary>
        /// Increment the "Warnings" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementWarningsLogged(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.TotalWarningMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.WarningMessagesPerSecond));
        }

        /// <summary>
        /// Increment the "Debugs" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementDebugsLogged(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.TotalDebugMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.DebugMessagesPerSecond));
        }

        /// <summary>
        /// Increment the "Infos" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementInfosLogged(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.TotalInfoMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.InfoMessagesPerSecond));
        }
    }

    public class ActorMonitoringExtension : ExtensionIdProvider<ActorMonitor>
    {
        public override ActorMonitor CreateExtension(ActorSystem system)
        {
            throw new System.NotImplementedException();
        }
    }
}
