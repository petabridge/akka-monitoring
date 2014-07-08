using Akka.Actor;
using Akka.Monitoring.Impl;

namespace Akka.Monitoring
{
    /// <summary>
    /// Extension methods to make it easier to capture performance metrics inside Actors
    /// </summary>
    public static class AkkaMonitoringExtensions
    {
        /// <summary>
        /// Fetches the monitors for the provided <see cref="IActorContext"/>
        /// </summary>
        private static ActorMonitor GetMonitor(IActorContext context)
        {
            return ActorMonitoringExtension.Monitors(context.System);
        }

        /// <summary>
        /// Increment the "Actor Restarts" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementActorRestart(this IActorContext context)
        {
            GetMonitor(context).IncrementActorRestart(context);
        }

        /// <summary>
        /// Increment the "Actors Created" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementActorCreated(this IActorContext context)
        {
            GetMonitor(context).IncrementActorCreated(context);
        }

        /// <summary>
        /// Increment the "Actors Stopped" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementActorStopped(this IActorContext context)
        {
            GetMonitor(context).IncrementActorStopped(context);
        }

        /// <summary>
        /// Increment the "Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementMessagesReceived(this IActorContext context)
        {
            GetMonitor(context).IncrementMessagesReceived(context);
        }

        /// <summary>
        /// Increment the "Unhandled Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementUnhandledMessage(this IActorContext context)
        {
            GetMonitor(context).IncrementUnhandledMessage(context);
        }

        /// <summary>
        /// Increment the "Deadletters" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementDeadLetters(this IActorContext context)
        {
            GetMonitor(context).IncrementDeadLetters(context);
        }

        /// <summary>
        /// Increment the "Errors" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementErrorsLogged(this IActorContext context)
        {
            GetMonitor(context).IncrementErrorsLogged(context);
        }

        /// <summary>
        /// Increment the "Warnings" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementWarningsLogged(this IActorContext context)
        {
            GetMonitor(context).IncrementWarningsLogged(context);
        }

        /// <summary>
        /// Increment the "Debugs" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementDebugsLogged(this IActorContext context)
        {
            GetMonitor(context).IncrementDebugsLogged(context);
        }

        /// <summary>
        /// Increment the "Infos" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementInfosLogged(this IActorContext context)
        {
            GetMonitor(context).IncrementInfosLogged(context);
        }

        /// <summary>
        /// Increment a custom user-defined counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="metricName">The name of the counter as it will appear in your monitoring system</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void IncrementCounter(this IActorContext context, string metricName, int value = 1, double sampleRate = 1)
        {
            GetMonitor(context).IncrementCounter(metricName, value, sampleRate, context);
        }

        /// <summary>
        /// Increment a custom timing, used to measure the elapsed time of something
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="metricName">The name of the timing as it will appear in your monitoring system</param>
        /// <param name="time">The amount of time that elapsed, in milliseconds</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void Timing(this IActorContext context, string metricName, long time, double sampleRate = 1)
        {
            GetMonitor(context).Timing(metricName, time, sampleRate, context);
        }

        /// <summary>
        /// Increment a custom Gauge, used to measure arbitrary values (such as the size of messages, etc... non-counter measurements)
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="metricName">The name of the timing as it will appear in your monitoring system</param>
        /// <param name="value">The value of the gauge</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void Gauge(this IActorContext context,string metricName, int value = 1, double sampleRate = 1)
        {
            GetMonitor(context).Gauge(metricName, value, sampleRate, context);
        }
    }
}
