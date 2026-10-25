namespace JEO3.Contracts
{
    public interface ITopExpensiveQueryRow
    {
        DateTime Timestamp { get; }
        string SqlHandleHex { get; }
        string PlanHandleHex { get; }
        int? StatementStartOffset { get; }
        int? StatementEndOffset { get; }
        string DatabaseName { get; }
        string QueryText { get; }
        long ExecutionCount { get; }

        // Duration / Elapsed Time 
        double TotalElapsedMs { get; }
        double AvgElapsedMs { get; }
        double MaxElapsedMs { get; }

        // CPU / Worker Time 
        double TotalCpuMs { get; }
        double AvgCpuMs { get; }
        double MaxCpuMs { get; }

        // I/O Metrics (Logical Reads & Writes)
        long TotalLogicalReads { get; }
        double AvgLogicalReads { get; }
        long MaxLogicalReads { get; }
        long TotalLogicalWrites { get; }
        double AvgLogicalWrites { get; }
        long MaxLogicalWrites { get; }

        // Combined I/O
        long TotalIo { get; }
        double AvgIo { get; }
        long MaxIo { get; }

        // Redgate Query Impact score
        double QueryImpact { get; }
        DateTime LastExecutionTime { get; }
    }
}
