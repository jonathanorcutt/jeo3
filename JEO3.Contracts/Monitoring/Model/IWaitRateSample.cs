namespace JEO3.Contracts
{
    public interface IWaitRateSample
    {
        DateTime Timestamp { get; }
        double TotalWaitMsPerSec { get; }
        double TotalWaitingTasksPerSec { get; }
        double ElapsedSeconds { get; }

        // Abstracting the inner list to use the interface footprint
        IReadOnlyList<IWaitTypeRate> TopWaits { get; }
    }
}
