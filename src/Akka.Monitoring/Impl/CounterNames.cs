using Akka.Actor;

namespace Akka.Monitoring.Impl
{
    /// <summary>
    /// A static list of names and naming schemes for built-in counters
    /// </summary>
    public static class CounterNames
    {

        #region Actor lifecycle counters

        public const string ActorRestarts = "akka.actor.restarts";
        public const string ActorsCreated = "akka.actor.created";
        public const string ActorsStopped = "akka.actor.stopped";

        #endregion

        #region Actor message counters
        public const string ReceivedMessages = "akka.messages.received";
        public const string DeadLetters = "akka.messages.deadletters";
        public const string UnhandledMessages = "akka.messages.unhandled";

        #endregion

        #region Actor logging counters

        public const string DebugMessages = "akka.logging.debug";
        public const string InfoMessages = "akka.logging.info";
        public const string WarningMessages = "akka.logging.warning";
        public const string ErrorMessages = "akka.logging.error";

        #endregion

        /// <summary>
        /// Gets the name of a counter collect for a specific group of actors
        /// </summary>
        /// <param name="context">The context of the actor reporting the problem</param>
        /// <param name="metricName">The metric being reported</param>
        /// <returns>A fully qualified metric name</returns>
        public static string ActorSpecificCategory(IActorContext context, string metricName)
        {
            return
                string.Format("{0}.{1}.{2}", context.System.Name, context.Props.Type.Name, metricName);
        }

    }
}
