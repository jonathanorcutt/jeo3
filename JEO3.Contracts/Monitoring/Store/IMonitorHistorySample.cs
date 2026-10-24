namespace JEO3.Contracts
{
    public interface IMonitorHistorySample
    {
        DateTime Timestamp { get; set; }
        IPerformanceMetricRow? Performance { get; }
        IReadOnlyList<ITopExpensiveQueryRow> TopExpensiveQueries { get; }
    }
}
