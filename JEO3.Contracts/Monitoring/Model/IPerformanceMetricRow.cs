namespace JEO3.Contracts
{
    public interface IPerformanceMetricRow
    {
        DateTime Timestamp { get; }
        double CpuPercent { get; }
        double MemoryUsagePercent { get; }
        double TotalServerMemoryMb { get; }
        double TargetServerMemoryMb { get; }
        double SqlMemoryUsageMb { get; }
        double PageLifeExpectancy { get; }
        double IoStallReadMs { get; }
        double IoStallWriteMs { get; }
        double BatchRequestsPerSec { get; }
        double UserConnections { get; }
    }
}
