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
        /// <summary>
        /// the internal registry used to track individual monitoring instances
        /// </summary>
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
            Registry.UpdateCounter(CounterNames.ActorRestarts);
            if(context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ActorRestarts));
        }

        /// <summary>
        /// Increment the "Actors Created" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementActorCreated(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.ActorsCreated);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ActorsCreated));
        }

        /// <summary>
        /// Increment the "Actors Stopped" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementActorStopped(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.ActorsStopped, 1);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ActorsStopped));
        }

        /// <summary>
        /// Increment the "Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementMessagesReceived(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.UnhandledMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.UnhandledMessages));
        }

        /// <summary>
        /// Increment the "Unhandled Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementUnhandledMessage(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.UnhandledMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.UnhandledMessages));
        }

        /// <summary>
        /// Increment the "Deadletters" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementDeadLetters(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.DeadLetters);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.DeadLetters));
        }

        /// <summary>
        /// Increment the "Errors" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementErrorsLogged(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.ErrorMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ErrorMessages));
        }

        /// <summary>
        /// Increment the "Warnings" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementWarningsLogged(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.WarningMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.WarningMessages));
        }

        /// <summary>
        /// Increment the "Debugs" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementDebugsLogged(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.DebugMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.DebugMessages));
        }

        /// <summary>
        /// Increment the "Infos" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementInfosLogged(IActorContext context = null)
        {
            Registry.UpdateCounter(CounterNames.InfoMessages);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.InfoMessages));
        }
    }

    /// <summary>
    /// The extension class registered with an Akka.NET <see cref="ActorSystem"/>
    /// </summary>
    public class ActorMonitoringExtension : ExtensionIdProvider<ActorMonitor>
    {

        public override ActorMonitor CreateExtension(ActorSystem system)
        {
            try
            { //shouldn't be able to create this actor multiple times
                system.ActorOf<AkkaMonitoringLogger>(AkkaMonitoringLogger.LoggerName);
            }
            catch { }
            return new ActorMonitor();
        }

        #region Static methods

        public static ActorMonitor Monitors(ActorSystem system)
        {
            return system.WithExtension<ActorMonitor, ActorMonitoringExtension>();
        }

        /// <summary>
        /// Register a new <see cref="AbstractActorMonitoringClient"/> instance to use when monitoring Actor operations.
        /// </summary>
        /// <returns>true if the monitor was succeessfully registered, false otherwise.</returns>
        public static bool RegisterMonitor(ActorSystem system, AbstractActorMonitoringClient client)
        {
            return Monitors(system).RegisterMonitor(client);
        }

        /// <summary>
        /// Deregister an existing <see cref="AbstractActorMonitoringClient"/> instance so it no longer reports metrics to existing Actors.
        /// </summary>
        /// <returns>true if the monitor was succeessfully deregistered, false otherwise.</returns>
        public static bool DeregisterMonitor(ActorSystem system, AbstractActorMonitoringClient client)
        {
            return Monitors(system).DeregisterMonitor(client);
        }

        /// <summary>
        /// Terminates all existing monitors. You can add new ones after this call has been made.
        /// </summary>
        public static void TerminateMonitors(ActorSystem system)
        {
            Monitors(system).TerminateMonitors();
        }

        #endregion
    }
}
