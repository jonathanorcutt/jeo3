namespace JEO3.Contracts
{
    public interface IBlockingRow
    {
        DateTime Timestamp { get; }
        int SessionId { get; }
        int BlockingSessionId { get; }
        bool IsBlocked { get; }
        string Status { get; }
        string LoginName { get; }
        string HostName { get; }
        string ProgramName { get; }
        string Command { get; }
        string WaitType { get; }
        double WaitTimeSec { get; }
        DateTime? RequestStartTime { get; }
        DateTime? TransactionBeginTime { get; }
        double? OpenTranDurationSec { get; }
        string QueryText { get; }
        bool IsVictim { get; }
        string? WaitResource { get; }
        string? IsolationLevel { get; }
        double? MemoryGrantMb { get; } // Changed to double? because FLOAT maps to double in C#

        string? SqlHandle { get; }
        string? PlanHandle { get; }

        // Allow nulls for safe fallback when processing idle sessions
        int? StatementStartOffset { get; }
        int? StatementEndOffset { get; }
    }
}
