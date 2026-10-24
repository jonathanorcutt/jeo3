namespace JEO3.Contracts
{
    public interface IActiveRequestRow
    {
        DateTime Timestamp { get; }
        int SessionId { get; }
        int BlockingSessionId { get; }
        string WaitType { get; }
        double WaitTimeSec { get; }
        string Status { get; }
        string Command { get; }
        string DatabaseName { get; }
        string LoginName { get; }
        string HostName { get; }
        string ProgramName { get; }
        long CpuTimeMs { get; }
        long LogicalReads { get; }
        long PhysicalReads { get; }
        long Writes { get; }
        int OpenTxnCount { get; }
        double PercentComplete { get; }
        string StatementText { get; }

        // Read-only calculated property
        bool IsBlocked { get; }
    }
}
