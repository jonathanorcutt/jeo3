using System.Text.Json.Serialization;
using JEO3.Contracts;
using JEO3.Monitor.Models;

namespace JEO3.Monitor
{
    public sealed class MonitorSnapshot : IMonitorSnapshot
    {
        public DateTime ServerTimeUtc { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MonitorPollingStatus Status { get; init; }
        public string? LastError { get; init; }
        public IReadOnlyList<WaitRateSample> History { get; init; } = [];
        public IReadOnlyList<ActiveRequestRow> ActiveRequests { get; init; } = [];
        public IReadOnlyList<BlockingRow> BlockingChain { get; init; } = [];
        public IReadOnlyList<DeadlockEvent> RecentDeadlocks { get; init; } = [];
        public IReadOnlyList<TopExpensiveQueryRow> TopExpensiveQueries { get; init; } = [];
        public IReadOnlyList<PerformanceMetricRow> PerformanceMetrics { get; init; } = [];
        public IReadOnlyList<MonitorHistorySample> PerformanceHistory { get; init; } = [];

        IReadOnlyList<IWaitRateSample> IMonitorSnapshot.History => History;
        IReadOnlyList<IActiveRequestRow> IMonitorSnapshot.ActiveRequests => ActiveRequests;
        IReadOnlyList<IBlockingRow> IMonitorSnapshot.BlockingChain => BlockingChain;
        IReadOnlyList<IDeadlockEvent> IMonitorSnapshot.RecentDeadlocks => RecentDeadlocks;
        IReadOnlyList<ITopExpensiveQueryRow> IMonitorSnapshot.TopExpensiveQueries => TopExpensiveQueries;
        IReadOnlyList<IPerformanceMetricRow> IMonitorSnapshot.PerformanceMetrics => PerformanceMetrics;
        IReadOnlyList<IMonitorHistorySample> IMonitorSnapshot.PerformanceHistory => PerformanceHistory;

        public WaitRateSample? LatestWaitRateSample => History.Count > 0 ? History[^1] : null;
        public PerformanceMetricRow? LatestPerformanceHistory => PerformanceMetrics.Count > 0 ? PerformanceMetrics[0] : null;
        public DeadlockEvent? LatestRecentDeadlock => RecentDeadlocks.Count > 0 ? RecentDeadlocks[0] : null;
        public BlockingRow? LatestBlockingChain => BlockingChain.Count > 0 ? BlockingChain[0] : null;
        public ActiveRequestRow? LatestActiveRequest => ActiveRequests.Count > 0 ? ActiveRequests[0] : null;
        public TopExpensiveQueryRow? LatestTopExpensiveQuery => TopExpensiveQueries.Count > 0 ? TopExpensiveQueries[0] : null;

        IWaitRateSample? IMonitorSnapshot.LatestWaitRateSample => LatestWaitRateSample;
        IPerformanceMetricRow? IMonitorSnapshot.LatestPerformanceHistory => LatestPerformanceHistory;
        IDeadlockEvent? IMonitorSnapshot.LatestRecentDeadlock => LatestRecentDeadlock;
        IBlockingRow? IMonitorSnapshot.LatestBlockingChain => LatestBlockingChain;
        IActiveRequestRow? IMonitorSnapshot.LatestActiveRequest => LatestActiveRequest;
        ITopExpensiveQueryRow? IMonitorSnapshot.LatestTopExpensiveQuery => LatestTopExpensiveQuery;
    }
}
