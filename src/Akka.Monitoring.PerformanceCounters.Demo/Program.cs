using System;
using System.Diagnostics;
using System.Threading;
using Akka.Actor;

namespace Akka.Monitoring.PerformanceCounters.Demo
{
    class WorkerActor : ReceiveActor
    {
        public WorkerActor()
        {
            Receive<string>(_ =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                
                            
                Thread.Sleep(650);//add some work to simulate message box growth, and see some timing metrics
                Console.WriteLine("work done");
                Context.Gauge("akka.messageboxsize", ((ActorCell)Context).NumberOfMessages);
                Context.IncrementCounter("akka.custom.metric1");
                Context.IncrementMessagesReceived();

                stopwatch.Stop();    
                Context.Timing("akka.handlertime",stopwatch.ElapsedTicks);                
            });            
        }

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(0, 300, Self, "do work", Self);
            base.PreStart();
        }
    }

    class HelloActor : TypedActor, IHandle<string>
    {
        protected override void PreStart()
        {
            Context.IncrementActorCreated();
            base.PreStart();
        }

        protected override void PostStop()
        {
            Context.IncrementActorStopped();
            base.PostStop();
        }

        public void Handle(string message)
        {
            Context.IncrementMessagesReceived();
            Console.WriteLine("Received: {0}", message);
            if (message == "Goodbye")
            {
                Context.Self.Tell(PoisonPill.Instance);
                Program.ManualResetEvent.Set(); //allow the program to exit
            }
            else
                Sender.Tell("Hello!");
        }
    }

    class GoodbyeActor : TypedActor, IHandle<Tuple<IActorRef, string>>, IHandle<string>
    {
        protected override void PreStart()
        {
            Context.IncrementActorCreated();
            Context.IncrementUnhandledMessage();
            base.PreStart();
        }

        protected override void PostStop()
        {
            Context.IncrementActorStopped();
            base.PostStop();
        }

        public void Handle(string message)
        {
            Context.IncrementMessagesReceived();
            Console.WriteLine("Received: {0}", message);
            Sender.Tell("Goodbye");
            Context.Self.Tell(PoisonPill.Instance);
        }

        public void Handle(Tuple<IActorRef, string> message)
        {
            Context.IncrementMessagesReceived();
            message.Item1.Tell("Starting");
        }
    }

    class Program
    {
        public static AutoResetEvent ManualResetEvent = new AutoResetEvent(false);

        private static ActorSystem _system;

        static void Main(string[] args)
        {

            _system = ActorSystem.Create("akka-performance-demo");
            var registeredMonitor = ActorMonitoringExtension.RegisterMonitor(_system,
                new ActorPerformanceCountersMonitor(
                    new CustomMetrics
                    {
                        Counters = { "akka.custom.metric1", "akka.custom.metric2" },
                        Gauges = { "akka.messageboxsize"},
                        Timers = { "akka.handlertime" }
                    }));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Starting up actor system...");
            var goodbye = _system.ActorOf<GoodbyeActor>();
            var hello = _system.ActorOf<HelloActor>();

            for (int i = 0; i < 11; i++)
            {
                _system.ActorOf<HelloActor>();
            }

            _system.ActorOf<WorkerActor>();

            if (registeredMonitor)
                Console.WriteLine("Successfully registered Performance Counters monitor");
            else
                Console.WriteLine("Failed to register Performance Counters monitor");
            Console.WriteLine("Incrementing debug log once every 10 ms for 2 seconds...");
            var count = 20;
            while (count >= 0)
            {
                ActorMonitoringExtension.Monitors(_system).IncrementDebugsLogged();
                Console.WriteLine("Logging debug...");
                Thread.Sleep(2000);
                count--;
            }
            Console.WriteLine("Starting a conversation between actors");
            goodbye.Tell(new Tuple<IActorRef, string>(hello, "Start"));
            while (ManualResetEvent.WaitOne())
            {
                Console.WriteLine("Shutting down...");
                _system.Shutdown();
                Console.WriteLine("Shutdown complete");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }
        }
    }
}
