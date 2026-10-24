using JEO3.Contracts;
using JEO3.Monitor.Models;

namespace JEO3.Monitor
{
    public sealed class MonitorDeltaResponse : IMonitorDeltaResponse
    {
        public DateTime ServerTimeUtc { get; set; }
        public MonitorPollingStatus Status { get; init; }
        public string? LastError { get; init; }

        // Time-series updates (only samples > sinceUtc)
        public IReadOnlyList<WaitRateSample> History { get; init; } = [];
        public IReadOnlyList<MonitorHistorySample> PerformanceHistory { get; init; } = [];
        public IReadOnlyList<DeadlockEvent> RecentDeadlocks { get; init; } = [];

        // Current transient state (always fresh)
        public IReadOnlyList<ActiveRequestRow> ActiveRequests { get; init; } = [];
        public IReadOnlyList<BlockingRow> BlockingChain { get; init; } = [];
        public IReadOnlyList<TopExpensiveQueryRow> TopExpensiveQueries { get; init; } = [];

        IReadOnlyList<IWaitRateSample> IMonitorDeltaResponse.History => History;
        IReadOnlyList<IMonitorHistorySample> IMonitorDeltaResponse.PerformanceHistory => PerformanceHistory;
        IReadOnlyList<IDeadlockEvent> IMonitorDeltaResponse.RecentDeadlocks => RecentDeadlocks;
        IReadOnlyList<IActiveRequestRow> IMonitorDeltaResponse.ActiveRequests => ActiveRequests;
        IReadOnlyList<IBlockingRow> IMonitorDeltaResponse.BlockingChain => BlockingChain;
        IReadOnlyList<ITopExpensiveQueryRow> IMonitorDeltaResponse.TopExpensiveQueries => TopExpensiveQueries;
    }
}
