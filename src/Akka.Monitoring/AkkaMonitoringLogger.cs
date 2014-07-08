using System;
using Akka.Actor;
using Akka.Event;

namespace Akka.Monitoring
{
    /// <summary>
    /// Logging implementation used to report key events, such as <see cref="DeadLetter"/> and <see cref="UnhandledMessage"/> instances
    /// back to an appropriate monitoring service
    /// </summary>
    public class AkkaMonitoringLogger : TypedActor,
        IHandle<DeadLetter>, IHandle<UnhandledMessage>,
        IHandle<LogEvent>
    {
        public const string LoggerName = "AkkaMonitoringLogger";

        protected override void PreStart()
        {
            ActorMonitoringExtension.Monitors(Context.System).IncrementActorCreated(Context);
            Context.System.EventStream.Subscribe(Self, typeof(Error));
            Context.System.EventStream.Subscribe(Self, typeof(Warning));
            Context.System.EventStream.Subscribe(Self, typeof(Debug));
            Context.System.EventStream.Subscribe(Self, typeof(Info));
            Context.System.EventStream.Subscribe(Self, typeof(DeadLetter));
            Context.System.EventStream.Subscribe(Self, typeof(UnhandledMessage));
        }

        protected override void PreRestart(System.Exception reason, object message)
        {
            ActorMonitoringExtension.Monitors(Context.System).IncrementActorRestart(Context);
            Context.System.EventStream.Unsubscribe(Self, typeof(Error));
            Context.System.EventStream.Unsubscribe(Self, typeof(Warning));
            Context.System.EventStream.Unsubscribe(Self, typeof(Debug));
            Context.System.EventStream.Unsubscribe(Self, typeof(Info));
            Context.System.EventStream.Unsubscribe(Self, typeof(DeadLetter));
            Context.System.EventStream.Unsubscribe(Self, typeof(UnhandledMessage));

        }

        protected override void PostRestart(Exception reason)
        {
            //no-op
        }

        protected override void PostStop()
        {
            ActorMonitoringExtension.Monitors(Context.System).IncrementActorStopped(Context);
        }

        public void Handle(DeadLetter message)
        {
            ActorMonitoringExtension.Monitors(Context.System).IncrementDeadLetters();
        }

        public void Handle(UnhandledMessage message)
        {
            ActorMonitoringExtension.Monitors(Context.System).IncrementUnhandledMessage();
        }

        public void Handle(LogEvent message)
        {
            var monitor = ActorMonitoringExtension.Monitors(Context.System);
            message.Match()
                .With<Debug>(d => monitor.IncrementDebugsLogged())
                .With<Error>(e => monitor.IncrementErrorsLogged())
                .With<Warning>(w => monitor.IncrementWarningsLogged())
                .With<Info>(i => monitor.IncrementInfosLogged())
                .Default(d => monitor.IncrementUnhandledMessage());
        }
    }
}
