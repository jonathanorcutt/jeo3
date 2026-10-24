namespace JEO3.Contracts
{
    public interface IWaitTypeRate
    {
        DateTime Timestamp { get; }
        string WaitType { get; }
        double WaitMsPerSec { get; }
        double WaitingTasksPerSec { get; }
        double SignalWaitMsPerSec { get; }

        // Read-only calculated property
        double ResourceWaitMsPerSec { get; }
    }
}
