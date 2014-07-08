using System;
using System.Threading;
using Akka.Actor;

namespace Akka.Monitoring.StatsD.Demo
{
    class Program
    {
        private static ActorSystem _system;

        static void Main(string[] args)
        {
            //var manualResetEvent = new AutoResetEvent(false);
            _system = ActorSystem.Create("akka-performance-demo");
            var registeredMonitor = ActorMonitoringExtension.RegisterMonitor(_system, new ActorStatsDMonitor());
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Starting up actor system...");
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
            
            Console.WriteLine("Press any key to shut down");
            Console.ReadKey();
            _system.Shutdown();
            Console.WriteLine("Shutting down...");
            //while (manualResetEvent.WaitOne())
            //{
            //    Console.WriteLine("Shutting down...");
            //    _system.Shutdown();
            //}
        }
    }
}
