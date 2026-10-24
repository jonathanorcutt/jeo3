namespace JEO3.Contracts
{
    public interface IMonitorDeltaResponse
    {
        DateTime ServerTimeUtc { get; set; }
        MonitorPollingStatus Status { get; init; }
        string? LastError { get; init; }

        // Time-series updates
        IReadOnlyList<IWaitRateSample> History { get; }
        IReadOnlyList<IMonitorHistorySample> PerformanceHistory { get; }
        IReadOnlyList<IDeadlockEvent> RecentDeadlocks { get; }

        // Current transient state
        IReadOnlyList<IActiveRequestRow> ActiveRequests { get; }
        IReadOnlyList<IBlockingRow> BlockingChain { get; }
        IReadOnlyList<ITopExpensiveQueryRow> TopExpensiveQueries { get; }
    }
}
