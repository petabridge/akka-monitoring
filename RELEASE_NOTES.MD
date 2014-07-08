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