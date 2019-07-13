using System;
using Akka.Actor;
using Akka.Event;

namespace Akka.Monitoring
{
    /// <summary>
    /// Logging implementation used to report key events, such as <see cref="DeadLetter"/> and <see cref="UnhandledMessage"/> instances
    /// back to an appropriate monitoring service
    /// </summary>
    public class AkkaMonitoringLogger : ActorBase
    {
        public const string LoggerName = "AkkaMonitoringLogger";

        private readonly ActorMonitor _monitor = ActorMonitoringExtension.Monitors(Context.System);

        protected override bool Receive(object message)
        {
            switch (message)
            {
                case Debug _:
                    _monitor.IncrementDebugsLogged();
                    return true;
                case Error _:
                    _monitor.IncrementErrorsLogged();
                    return true;
                case Warning _:
                    _monitor.IncrementWarningsLogged();
                    return true;
                case Info _:
                    _monitor.IncrementInfosLogged();
                    return true;
                case DeadLetter _:
                    _monitor.IncrementDeadLetters();
                    return true;
                case UnhandledMessage _:
                    _monitor.IncrementUnhandledMessage();
                    return true;
                default: return false;
            }
        }

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
            _monitor.IncrementActorStopped(Context);
        }
    }
}
