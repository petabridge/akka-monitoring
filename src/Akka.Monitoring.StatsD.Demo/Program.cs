using System;
using System.Threading;
using Akka.Actor;

namespace Akka.Monitoring.StatsD.Demo
{
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
                Context.Self.Stop();
                Program.ManualResetEvent.Set(); //allow the program to exit
            }
            else
                Sender.Tell("Hello!");
        }
    }

    class GoodbyeActor : TypedActor, IHandle<Tuple<ActorRef,string>>, IHandle<string>
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
            Sender.Tell("Goodbye");
            Context.Self.Stop();
        }

        public void Handle(Tuple<ActorRef, string> message)
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
            var registeredMonitor = ActorMonitoringExtension.RegisterMonitor(_system, new ActorStatsDMonitor());
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Starting up actor system...");
            var goodbye = _system.ActorOf<GoodbyeActor>();
            var hello = _system.ActorOf<HelloActor>();
            if(registeredMonitor)
                Console.WriteLine("Successfully registered StatsD monitor");
            else
                Console.WriteLine("Failed to register StatsD monitor");
            Console.WriteLine("Incrementing debug log once every 10 ms for 2 seconds...");
            var count = 20;
            while (count >= 0)
            {
                ActorMonitoringExtension.Monitors(_system).IncrementDebugsLogged();
                Console.WriteLine("Logging debug...");
                Thread.Sleep(100);
                count--;
            }
            Console.WriteLine("Starting a conversation between actors");
            goodbye.Tell(new Tuple<ActorRef,string>(hello,"Start"));
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
