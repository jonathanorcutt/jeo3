namespace JEO3.Contracts
{
    public interface IMonitorSnapshot
    {
        DateTime ServerTimeUtc { get; set; }
        MonitorPollingStatus Status { get; init; }
        string? LastError { get; init; }

        // Abstracted Collections
        IReadOnlyList<IWaitRateSample> History { get; }
        IReadOnlyList<IActiveRequestRow> ActiveRequests { get; }
        IReadOnlyList<IBlockingRow> BlockingChain { get; }
        IReadOnlyList<IDeadlockEvent> RecentDeadlocks { get; }
        IReadOnlyList<ITopExpensiveQueryRow> TopExpensiveQueries { get; }
        IReadOnlyList<IPerformanceMetricRow> PerformanceMetrics { get; }
        IReadOnlyList<IMonitorHistorySample> PerformanceHistory { get; } // Assumed interface name based on pattern

        // Abstracted Calculated Lookups
        IWaitRateSample? LatestWaitRateSample { get; }
        IPerformanceMetricRow? LatestPerformanceHistory { get; }
        IDeadlockEvent? LatestRecentDeadlock { get; }
        IBlockingRow? LatestBlockingChain { get; }
        IActiveRequestRow? LatestActiveRequest { get; }
        ITopExpensiveQueryRow? LatestTopExpensiveQuery { get; }
    }
}
