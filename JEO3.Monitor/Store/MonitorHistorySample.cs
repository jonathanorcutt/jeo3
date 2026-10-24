using JEO3.Contracts;
using JEO3.Monitor.Models;

namespace JEO3.Monitor
{
    public sealed class MonitorHistorySample : IMonitorHistorySample
    {
        public DateTime Timestamp { get; set; }
        public PerformanceMetricRow? Performance { get; set; }
        public IReadOnlyList<TopExpensiveQueryRow> TopExpensiveQueries { get; set; } = [];

        IPerformanceMetricRow? IMonitorHistorySample.Performance => Performance;
        IReadOnlyList<ITopExpensiveQueryRow> IMonitorHistorySample.TopExpensiveQueries => TopExpensiveQueries;
    }
}
