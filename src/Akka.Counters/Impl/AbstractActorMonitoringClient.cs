using System;

namespace Akka.Monitoring.Impl
{
    /// <summary>
    /// Abstract base class for monitoring clients
    /// </summary>
    public abstract class AbstractActorMonitoringClient : IActorMonitoringClient
    {
        public abstract void UpdateCounter(string metricName, int delta, double sampleRate);
        public abstract void UpdateTimer(string metricName, long time);
        public abstract void UpdateGauge(string metricName, int value);
        public abstract int MonitoringClientId { get; }
        

        public override int GetHashCode()
        {
            return MonitoringClientId;
        }

        #region IDisposable members

        public bool WasDisposed { get; private set; }
        public void Dispose(bool isDisposing)
        {
            if (!WasDisposed)
            {
                WasDisposed = true;
                GC.SuppressFinalize(this);
                try
                {
                    DisposeInternal();
                }
                catch { }
            }
        }

        /// <summary>
        /// The internal disposal method, to be filled in by a child-class
        /// </summary>
        public abstract void DisposeInternal();

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}