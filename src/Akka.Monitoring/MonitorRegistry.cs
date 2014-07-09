using System.Linq;
using Akka.Monitoring.Impl;
using Akka.Util;

namespace Akka.Monitoring
{
    /// <summary>
    /// A thread-safe registry for tracking all of the monitor implementations used for any particular
    /// instance of Akka.NET
    /// </summary>
    internal class MonitorRegistry
    {
        /// <summary>
        /// The list of active clients who are available for broadcast
        /// </summary>
        private readonly ConcurrentSet<IActorMonitoringClient> _activeClients = new ConcurrentSet<IActorMonitoringClient>();

        /// <summary>
        /// Add a new <see cref="IActorMonitoringClient"/> monitoring implementation to be used
        /// when reporting Actor metrics
        /// </summary>
        /// <returns>true if the client was successfully added, false otherwise</returns>
        public bool AddMonitor(AbstractActorMonitoringClient client)
        {
            return _activeClients.TryAdd(client);
        }

        /// <summary>
        /// Remove a <see cref="IActorMonitoringClient"/> implementation from the active set
        /// </summary>
        /// <returns>true if the client was successfully removed, false otherwise</returns>
        public bool RemoveMonitor(AbstractActorMonitoringClient client)
        {
            return _activeClients.TryRemove(client);
        }

        /// <summary>
        /// Update a counter across all active monitoring clients
        /// </summary>
        public void UpdateCounter(string metricName, int delta = 1, double sampleRate = 1.0)
        {
            foreach(var client in _activeClients)
                client.UpdateCounter(metricName,delta,sampleRate);
        }

        /// <summary>
        /// Update a timer across all active monitoring clients
        /// </summary>
        public void UpdateTimer(string metricName, long time, double sampleRate = 1.0)
        {
            foreach(var client in _activeClients)
                client.UpdateTiming(metricName, time, sampleRate);
        }

        /// <summary>
        /// Update a gauge across all active monitoring clients
        /// </summary>
        public void UpdateGauge(string metricName, int value, double sampleRate = 1.0)
        {
            foreach(var client in _activeClients)
                client.UpdateGauge(metricName, value, sampleRate);
        }

        /// <summary>
        /// Dispose all of the monitoring clients
        /// </summary>
        public void DisposeAll()
        {
            var clients = _activeClients.ToArray();
            _activeClients.Clear();
            foreach (var client in clients)
                client.Dispose();
        }
    }
}
