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
        /// Set a global sample rate for all counters
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="sampleRate">The sample rate to use across all monitoring calls. 100% by default.</param>
        public static void SetGlobalSampleRate(this IActorContext context, double sampleRate)
        {
            GetMonitor(context).SetGlobalSampleRate(sampleRate);
        }

        /// <summary>
        /// Increment the "Actor Restarts" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void IncrementActorRestart(this IActorContext context, int value = 1, double? sampleRate = null)
        {
            GetMonitor(context).IncrementActorRestart(context, value, sampleRate);
        }

        /// <summary>
        /// Increment the "Actors Created" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void IncrementActorCreated(this IActorContext context, int value = 1, double? sampleRate = null)
        {
            GetMonitor(context).IncrementActorCreated(context, value, sampleRate);
        }

        /// <summary>
        /// Increment the "Actors Stopped" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void IncrementActorStopped(this IActorContext context, int value = 1, double? sampleRate = null)
        {
            GetMonitor(context).IncrementActorStopped(context, value, sampleRate);
        }

        /// <summary>
        /// Increment the "Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void IncrementMessagesReceived(this IActorContext context, int value = 1, double? sampleRate = null)
        {
            GetMonitor(context).IncrementMessagesReceived(context, value, sampleRate);
        }

        /// <summary>
        /// Increment the "Unhandled Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void IncrementUnhandledMessage(this IActorContext context, int value = 1, double? sampleRate = null)
        {
            GetMonitor(context).IncrementUnhandledMessage(context, value, sampleRate);
        }

        /// <summary>
        /// Increment the "Deadletters" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void IncrementDeadLetters(this IActorContext context, int value = 1, double? sampleRate = null)
        {
            GetMonitor(context).IncrementDeadLetters(context, value, sampleRate);
        }

        /// <summary>
        /// Increment the "Errors" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void IncrementErrorsLogged(this IActorContext context, int value = 1, double? sampleRate = null)
        {
            GetMonitor(context).IncrementErrorsLogged(context, value, sampleRate);
        }

        /// <summary>
        /// Increment the "Warnings" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void IncrementWarningsLogged(this IActorContext context, int value = 1, double? sampleRate = null)
        {
            GetMonitor(context).IncrementWarningsLogged(context, value, sampleRate);
        }

        /// <summary>
        /// Increment the "Debugs" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        public static void IncrementDebugsLogged(this IActorContext context, int value = 1, double? sampleRate = null)
        {
            GetMonitor(context).IncrementDebugsLogged(context, value, sampleRate);
        }

        /// <summary>
        /// Increment the "Infos" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        public static void IncrementInfosLogged(this IActorContext context, int value = 1, double? sampleRate = null)
        {
            GetMonitor(context).IncrementInfosLogged(context, value, sampleRate);
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
        public static void Gauge(this IActorContext context, string metricName, int value = 1, double sampleRate = 1)
        {
            GetMonitor(context).Gauge(metricName, value, sampleRate, context);
        }
    }
}
