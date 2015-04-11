#### 0.3.3 Apr 10 2015
* Akka.Monitoring for Akka.NET v1.0!

#### 0.3.2 Aug 22 2014
* Upgraded to latest version of Akka.NET due to breaking changes made to extensions interface...

#### 0.3.1 Jul 15 2014
* Upgraded NStatsD.HighPerformance dependency (again)

#### 0.3.0 Jul 15 2014
* Upgraded NStatsD.HighPerformance dependency
* Removed Helios dependency
* Added ability to set sample rates on all calls
* Added `SetGlobalSampleRate` method to enable setting a sample rate globally across all calls

#### 0.2.5 Jul 09 2014
* Upgraded Helios and Akka dependency

#### 0.2.4 Jul 08 2014
* Upgraded Helios and removed dependency on JSON.NET


#### 0.2.3 Jul 08 2014
* Totally replaced our StatsD NuGet package

#### 0.2.2 Jul 08 2014
* Fixed the namespacing for StatsD monitoring so system-wide metrics aren't accidentally hidden

#### 0.2.1 Jul 08 2014
* Fixed the name of the `IncrementMessagesReceived` counter so it now correctly reads as `messages.received`


#### 0.2.0 Jul 08 2014
* Made it so custom metrics now prepend their names with the appropriate actor system / actor name
* Fixed some bugs with reporting custom gauges
* Updated FAKE build file

#### 0.1.1 Jul 07 2014
* Fixed a bug with FAKE build system.
* Fixed some bugs with logger initialization.

#### 0.1.0 Jul 07 2014
* Initial release - contains pluggable support for future monitoring systems and Esty's StatsD right out of the box.