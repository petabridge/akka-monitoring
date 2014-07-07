using Akka.Actor;

namespace Akka.Monitoring.Impl
{
    /// <summary>
    /// A static list of names and naming schemes for built-in counters
    /// </summary>
    public static class CounterNames
    {

        #region Actor lifecycle counters

        public const string ActorLifeCycleCategory = "Akka.NET Actor Lifecycle";
        public const string TotalActorRestarts = "Actor Restarts";
        public const string ActorRestartsPerSecond = "Actor Restarts / sec";
        public const string TotalActors = "Total Actors";
        public const string ActorsCreatedPerSecond = "Actors Created / sec";
        public const string ActorsStoppedPerSecond = "Actors Stopped / sec";

        #endregion

        #region Actor message counters

        public const string ActorMessagesCategory = "Akka.NET Actor Messages";
        public const string TotalMessages = "Total Messages";
        public const string MessagesPerSecond = "Messages / sec";
        public const string TotalDeadletters = "Total Deadletters";
        public const string DeadlettersPerSecond = "Deadletters / sec";
        public const string TotalUnhandledMessages = "Total Unhandled Messages";
        public const string UnhandledMessagesPerSecond = "Unhandled Messages / sec";

        #endregion

        #region Actor logging counters

        public const string ActorLogsCategory = "Akka.NET Log Messages";
        public const string TotalDebugMessages = "Total Debug Messages";
        public const string DebugMessagesPerSecond = "Debug Messages / sec";

        public const string TotalInfoMessages = "Total Info Messages";
        public const string InfoMessagesPerSecond = "Info Messages / sec";

        public const string TotalWarningMessages = "Total Warning Messages";
        public const string WarningMessagesPerSecond = "Warning Messages / sec";

        public const string TotalErrorMessages = "Total Error Messages";
        public const string ErrorMessagesPerSecond = "Error Messages / sec";

        #endregion

        /// <summary>
        /// Gets the name of a counter collect for a specific group of actors
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string ActorSpecificCategory(IActorContext context)
        {
            return
                string.Format(string.Format("Akka.NET ({0} / {1}) Actor Metrics", context.System.Name,
                    context.Props.GetType()));
        }

    }
}
