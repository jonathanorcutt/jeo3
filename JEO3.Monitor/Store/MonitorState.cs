using JEO3.Contracts;

namespace JEO3.Monitor
{
    /// <summary>
    /// Live monitoring state for the SQL Server performance monitor panel.
    /// Added to DashboardWorkspace so components subscribe via OnMonitorStateChanged.
    /// </summary>
    public sealed class MonitorState
    {
        public MonitorPollingStatus Status { get; set; } = MonitorPollingStatus.Stopped;

        /// <summary>
        /// Non-null when a transient query failure occurred.
        /// The service keeps polling; this is cleared on next successful sample.
        /// </summary>
        public string? LastError { get; set; }
        public IWaitRateSample? Latest => History.Count > 0 ? History[^1] : null;
        /// <summary>
        /// Rolling ring buffer of the last N wait-rate samples, oldest first.
        /// Capacity controlled by SqlServerMonitorService.HistoryCapacity.
        /// </summary>
        public IReadOnlyList<IWaitRateSample> History { get; set; } = [];
        public IReadOnlyList<IActiveRequestRow> ActiveRequests { get; set; } = [];
        public IReadOnlyList<IBlockingRow> BlockingChain { get; set; } = [];
        public IReadOnlyList<IDeadlockEvent> Deadlocks { get; set; } = [];
        public IReadOnlyList<IPerformanceMetricRow> PerformanceMetrics { get; set; } = [];
        public IReadOnlyList<ITopExpensiveQueryRow> TopExpensiveQueries { get; set; } = [];
        public List<double> SparklinePoints { get; set; } = new();
    }
}
