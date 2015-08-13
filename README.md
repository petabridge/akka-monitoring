Akka.Monitoring
===============================
Pluggable monitoring system instrumentation for [Akka.NET](https://github.com/akkadotnet/akka.net "Port of Akka actors for .NET") actor systems.

## What it is
Akka.Monitoring is an **ActorSystem extension** for Akka.NET that exposes a pluggable layer for reporting performance metrics from actors back to a monitoring system such as [Etsy's StatsD](https://github.com/etsy/statsd) or [Graphite](http://graphite.readthedocs.org/en/latest/) or [Microsoft AppInsights](https://www.visualstudio.com/features/application-insights-vs).

Akka.Monitoring can report to multiple monitoring systems simultaneously and it can report different metrics for different discrete ActorSystems inside the same process without any collisions. It offers high-performance, a simple interface, and the ability to be easily extended in order to support new monitoring systems.

### What does Akka.Monitoring monitor?

Akka.Monitoring collects the following pieces of data about your applications:

1. **Actor lifecycle** - actor starts / stops / restarts per second.
2. **Akka log events** - errors, warnings, debug messages, and info messages.
3. **Messaging metrics** - messages received, unhandled messages, and dead letters.
4. **Custom counters, gauges, and timers** - if you want to know how long it takes an actor to process a message inside its mailbox, Akka.Monitoring can time it.

**Akka.Monitoring automatically breaks out all of these metrics  by actor system totals AND 
metrics for individual actor types.** That way you can identify overly chatty or unresponsive actors without having to dig through extensive logs.

### Supported monitoring systems

Currently Akka.Monitoring supports the following monitoring systems out of the box:

1. **[StatsD](https://github.com/etsy/statsd)** - via a lightweight dependency on [NStatsD](https://github.com/robbihun/NStatsD.Client "A .NET 4.0 client for Etsy's StatsD server.").
2. **[Microsoft AppInsights](https://www.visualstudio.com/features/application-insights-vs)**
3. **[Performance Counters](https://msdn.microsoft.com/pl-pl/library/windows/desktop/aa373083%28v=vs.85%29.aspx)**

If you don't have any monitoring systems configured to run with Akka.Monitoring, no problem - the extension will no-op all stat collection calls by default.

## How you use it

First thing you want to do is [install the Akka.Monitoring packages via NuGet](https://www.nuget.org/packages/Akka.Monitoring/):

    Install-Package Akka.Monitoring

And then install a specific implementation

    Install-Package Akka.Monitoring.StatsD
    Install-Package Akka.Monitoring.ApplicationInsights

### Register monitors with your `ActorSystem`

Once that's done, programmatically register your monitoring agent with the `ActorMonitoringExtension` object like this:

    using Akka;
	using Akka.Monitoring;
	using Akka.Monitoring.StatsD;

	_system = ActorSystem.Create("akka-performance-demo");
    var registeredMonitor = ActorMonitoringExtension.RegisterMonitor(_system, new ActorStatsDMonitor());

This will automatically register the `AkkaStatsDMonitor` monitoring agent with your `ActorSystem`, and it will be available for use immediately. 

### Record metrics inside your actors
Now that you have your monitor registered, you can begin recording metrics - there are two ways of doing this:

**Record metrics via the `ActorContext` extension methods**
You can record all of your metrics directly off of the `Context` object inside each of your actors, like this:

	class HelloActor : TypedActor, IHandle<string>
	{
	    protected override void PreStart()
	    {
	        Context.IncrementActorCreated();
	    }
	
	    protected override void PostStop()
	    {
	        Context.IncrementActorStopped();
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

Alternatively, if you need to be able to record some custom metrics without the `Context` object, you can use the `ActorMonitoringExtension` object directly.

**Record metrics via the `ActorMonitoringExtension` methods**

	using Akka;
	using Akka.Monitoring;
	using Akka.Monitoring.StatsD;

	_system = ActorSystem.Create("akka-performance-demo");
    var registeredMonitor = ActorMonitoringExtension.RegisterMonitor(_system, new ActorStatsDMonitor());
	ActorMonitoringExtension.Monitors(_system).IncrementDebugsLogged();
    Console.WriteLine("Logging debug...");

**Metric capture methods**

Both of these techniques expose the following methods available for capturing metrics:

* `IncrementActorsCreated`
* `IncrementActorsRestarted`
* `IncrementActorsStopped`
* `IncrementMessagesReceived`
* `IncrementUnhandledMessages` - logged automatically
* `IncrementDeadLetters`  - logged automatically
* `IncrementErrorsLogged` - logged automatically
* `IncrementWarningsLogged` - logged automatically
* `IncrementDebugsLogged` - logged automatically
* `IncrementInfosLogged` - logged automatically
* `IncrementCounter` - for custom counters.
* `Timing` - for timing custom events, such as the amount of time needed to process a mailbox.
* `Gauge` - for recording arbitrary non-counter metrics, such as the average size of a message. 

All of these methods have extensive Intellisense documentation.

### Using Performance Counters

**Akka.Monitoring.PerformanceCounters** allows to track all metrics in perfmon. Once `ActorPerformanceCountersMonitor` is registered via `ActorMonitoringExtension` it automatically creates Performance Counters Category named **Akka** which contains all metrics related with agents events. There is one performance counter created for every metric (built-in or custom). Also, for every actor type there is performance counter instance which allows to track metrics per single actor class:

![alt performance counters selection](https://raw.github.com/MaciekLesiczka/akka-monitoring/dev/images/perfmance_counters_category.png)

An example of perfmon chart displaying metrics from **Akka.Monitoring.PerformanceCounters.Demo**:

![alt perfmon chart](https://raw.github.com/MaciekLesiczka/akka-monitoring/dev/images/performance_counters.gif)

**Custom metrics registration**

In order to create custom counters, timing metrics and gauges, you need to provide `CustomMetrics` with metric names  to `ActorPerformanceCountersMonitor` constructor:

    var registeredMonitor = ActorMonitoringExtension.RegisterMonitor(_system,
        new ActorPerformanceCountersMonitor(
            new CustomMetrics
            {
                Counters = { "akka.custom.metric1", "akka.custom.metric2" },
                Gauges = { "akka.messageboxsize"},
                Timers = { "akka.handlertime" }
            }));

Metric names will become performance counter names, so make sure they are unique among Akka Pefmormance Counters Category.

## Extending Akka.Monitoring
Have a different monitoring system you want to use? It's easy to integrate into Akka.Monitoring - just implement your own subclass from `AbstractActorMonitoringClient` ([source](https://github.com/Aaronontheweb/akka-monitoring/blob/master/src/Akka.Monitoring/Impl/AbstractActorMonitoringClient.cs)) and follow the registration steps above.

### Building Akka.Monitoring
Akka.Monitoring uses the [F# FAKE build system](http://fsharp.github.io/FAKE/). To build our NuGet packages locally, just execute [**build.cmd**](https://github.com/Aaronontheweb/akka-monitoring/blob/master/build.cmd) locally.

## FAQ

**What happens if I call Akka.Monitoring without having any monitors registered?**

Nothing - all calls are automatically no-oped if no monitors have been registered, so you're safe to release instrumented code even if you don't have monitoring configured yet.

**What happens if I call Akka.Monitoring with multiple monitors registered?**

Akka.Monitoring will automatically pipe its counter / timer / gauge updates to all attached monitoring systems, so you get that data everywhere.

**Any plans to automatically collect actor lifecycle and message received data?**

That depends largely on how much traction Akka.Monitoring gets - we're considering subclassing `UntypedActor` and `TypedActor` to automatically provide lifecycle and receive instrumentation, but we want to see how other people use it first.

**How in the hell do I configure StatsD and Graphite?**

These may help you:

* [Installing StatsD and Graphite on Ubuntu 12.04 LTS](https://www.digitalocean.com/community/tutorials/installing-and-configuring-graphite-and-statsd-on-an-ubuntu-12-04-vps)
* [Install Graphite and statsd on Ubuntu 12.04 LTS (Precise Pangolin)](https://gist.github.com/bhang/2703599)
* [Installing Graphite on Ubuntu 14.04](http://digitalronin.github.io/2014/04/29/installing-graphite-on-ubuntu-1404/)

## License
See [License](https://github.com/Aaronontheweb/akka-monitoring/blob/master/LICENSE) for details.

