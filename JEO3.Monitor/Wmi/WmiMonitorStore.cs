namespace JEO3.Monitor.Wmi
{
    public sealed class WmiMonitorStore
    {
        private readonly object _lock = new();

        private WmiMonitorSnapshot _current = new();

        public WmiMonitorSnapshot Current
        {
            get
            {
                lock (_lock)
                    return _current;
            }
        }

        public event Action? Changed;

        public void Update(WmiMonitorSnapshot snapshot)
        {
            lock (_lock)
                _current = snapshot;

            Changed?.Invoke();
        }
    }
}
