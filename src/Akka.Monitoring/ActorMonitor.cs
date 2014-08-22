using Akka.Actor;
using Akka.Event;
using Akka.Monitoring.Impl;
using Akka.Util;

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
        /// The SampleRate used by default across all calls unless otherwise specified
        /// </summary>
        internal double GlobalSampleRate = 1.0d;

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
        /// Set a global sample rate for all counters
        /// </summary>
        /// <param name="sampleRate"></param>
        public void SetGlobalSampleRate(double sampleRate)
        {
            GlobalSampleRate = sampleRate;
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
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public void IncrementActorRestart(IActorContext context = null, int value = 1, double? sampleRate = null)
        {
            Registry.UpdateCounter(CounterNames.ActorRestarts, value, sampleRate ?? GlobalSampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ActorRestarts), value, sampleRate ?? GlobalSampleRate);
        }

        /// <summary>
        /// Increment the "Actors Created" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public void IncrementActorCreated(IActorContext context = null, int value = 1, double? sampleRate = null)
        {
            Registry.UpdateCounter(CounterNames.ActorsCreated, value, sampleRate ?? GlobalSampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ActorsCreated), value, sampleRate ?? GlobalSampleRate);
        }

        /// <summary>
        /// Increment the "Actors Stopped" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public void IncrementActorStopped(IActorContext context = null, int value = 1, double? sampleRate = null)
        {
            Registry.UpdateCounter(CounterNames.ActorsStopped, value, sampleRate ?? GlobalSampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ActorsStopped), value, sampleRate ?? GlobalSampleRate);
        }

        /// <summary>
        /// Increment the "Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public void IncrementMessagesReceived(IActorContext context = null, int value = 1, double? sampleRate = null)
        {
            Registry.UpdateCounter(CounterNames.ReceivedMessages, value, sampleRate ?? GlobalSampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ReceivedMessages), value, sampleRate ?? GlobalSampleRate);
        }

        /// <summary>
        /// Increment the "Unhandled Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public void IncrementUnhandledMessage(IActorContext context = null, int value = 1, double? sampleRate = null)
        {
            Registry.UpdateCounter(CounterNames.UnhandledMessages, value, sampleRate ?? GlobalSampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.UnhandledMessages), value, sampleRate ?? GlobalSampleRate);
        }

        /// <summary>
        /// Increment the "Deadletters" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public void IncrementDeadLetters(IActorContext context = null, int value = 1, double? sampleRate = null)
        {
            Registry.UpdateCounter(CounterNames.DeadLetters, value, sampleRate ?? GlobalSampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.DeadLetters), value, sampleRate ?? GlobalSampleRate);
        }

        /// <summary>
        /// Increment the "Errors" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public void IncrementErrorsLogged(IActorContext context = null, int value = 1, double? sampleRate = null)
        {
            Registry.UpdateCounter(CounterNames.ErrorMessages, value, sampleRate ?? GlobalSampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.ErrorMessages), value, sampleRate ?? GlobalSampleRate);
        }

        /// <summary>
        /// Increment the "Warnings" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public void IncrementWarningsLogged(IActorContext context = null, int value = 1, double? sampleRate = null)
        {
            Registry.UpdateCounter(CounterNames.WarningMessages, value, sampleRate ?? GlobalSampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.WarningMessages), value, sampleRate ?? GlobalSampleRate);
        }

        /// <summary>
        /// Increment the "Debugs" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public void IncrementDebugsLogged(IActorContext context = null, int value = 1, double? sampleRate = null)
        {
            Registry.UpdateCounter(CounterNames.DebugMessages, value, sampleRate ?? GlobalSampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.DebugMessages), value, sampleRate ?? GlobalSampleRate);
        }

        /// <summary>
        /// Increment the "Infos" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public void IncrementInfosLogged(IActorContext context = null, int value = 1, double? sampleRate = null)
        {
            Registry.UpdateCounter(CounterNames.InfoMessages, value, sampleRate ?? GlobalSampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, CounterNames.InfoMessages), value, sampleRate ?? GlobalSampleRate);
        }

        /// <summary>
        /// Increment a custom user-defined counter
        /// </summary>
        /// <param name="metricName">The name of the counter as it will appear in your monitoring system</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// /// <param name="context">The context of the actor making this call</param>
        public void IncrementCounter(string metricName, int value = 1, double sampleRate = 1, IActorContext context = null)
        {
            Registry.UpdateCounter(metricName, value, sampleRate);
            if (context != null)
                Registry.UpdateCounter(CounterNames.ActorSpecificCategory(context, metricName), value, sampleRate);
        }

        /// <summary>
        /// Increment a custom timing, used to measure the elapsed time of something
        /// </summary>
        /// <param name="metricName">The name of the timing as it will appear in your monitoring system</param>
        /// <param name="time">The amount of time that elapsed, in milliseconds</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="context">The context of the actor making this call</param>
        public void Timing(string metricName, long time, double sampleRate = 1, IActorContext context = null)
        {
            Registry.UpdateTimer(metricName, time, sampleRate);
            if (context != null)
                Registry.UpdateTimer(CounterNames.ActorSpecificCategory(context, metricName), time, sampleRate);
        }

        /// <summary>
        /// Increment a custom Gauge, used to measure arbitrary values (such as the size of messages, etc... non-counter measurements)
        /// </summary>
        /// <param name="metricName">The name of the timing as it will appear in your monitoring system</param>
        /// <param name="value">The value of the gauge</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="context">The context of the actor making this call</param>
        public void Gauge(string metricName, int value = 1, double sampleRate = 1, IActorContext context = null)
        {
            Registry.UpdateGauge(metricName, value, sampleRate);
            if (context != null)
                Registry.UpdateGauge(CounterNames.ActorSpecificCategory(context, metricName), value, sampleRate);
        }
    }

    /// <summary>
    /// The extension class registered with an Akka.NET <see cref="ActorSystem"/>
    /// </summary>
    public class ActorMonitoringExtension : ExtensionIdProvider<ActorMonitor>
    { 
        protected static ConcurrentSet<string> ActorSystemsWithLogger = new ConcurrentSet<string>();
        protected static object LoggerLock = new object();

        public override ActorMonitor CreateExtension(ExtendedActorSystem system)
        {
            try
            {
                //ActorSystem does not have a monitor logger yet
                if (!ActorSystemsWithLogger.Contains(system.Name))
                {
                    lock (LoggerLock)
                    {
                        if (ActorSystemsWithLogger.TryAdd(system.Name))
                        {
                            system.ActorOf<AkkaMonitoringLogger>(AkkaMonitoringLogger.LoggerName);
                        }
                    }
                }
                
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
