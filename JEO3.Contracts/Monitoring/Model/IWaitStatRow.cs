namespace JEO3.Contracts
{
    public interface IWaitStatRow
    {
        DateTime Timestamp { get; }
        string WaitType { get; }
        long WaitingTasksCount { get; }
        long WaitTimeMs { get; }
        long MaxWaitTimeMs { get; }
        long SignalWaitTimeMs { get; }
    }
}
