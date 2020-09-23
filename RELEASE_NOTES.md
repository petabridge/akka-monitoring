#### 1.0.1 September 23 2020
* Upgraded to Akka.NET v1.3.18
* Migrated all packages to .NET Standard 2.0
* Switched StatsD component to use [JustEat.StatsD](https://www.nuget.org/packages/JustEat.StatsD/)

**Phobos vs. Akka.Monitoring**
In 2018 [Petabridge](https://petabridge.com/), the makers of Akka.Monitoring, [published a propreitary product for Akka.NET monitoring and tracing called Phobos](https://phobos.petabridge.com/).

Phobos is much more fully-featured than Akka.Monitoring, as it:

1. Doesn't require users to decorate their actors with any code - monitoring and tracing happens automatically;
2. Supports a much larger number of metric systems and types out of the box;
3. Includes traceability with actors, including over the network with Akka.Remote, and other parts of your system such as ASP.NET Core; and
4. Comes with 1 business day SLA support for Phobos and installing it into your Akka.NET applications.

That said, [a Phobos license costs your company $4,000 per year](https://phobos.petabridge.com/articles/setup/request.html).

It's the belief of Petabridge that there should be a low-cost alternative to that in order to make Akka.NET accessible to students, startups, DIY developers, and anyone who doesn't want to pay for that license. That's what Akka.Monitoring is - a simple, straightforward alternative that is free and open source. We will continue to support Akka.Monitoring and respond to end-user requests, but you should expect the majority of our development efforts to go into Phobos.

Please open an issue or [contact Petabridge](https://petabridge.com/contact/) if you have any questions.