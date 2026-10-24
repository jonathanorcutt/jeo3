namespace JEO3.Contracts
{
    public interface IMonitorChartDataService
    {
        IMonitorSnapshot? CurrentSnapshot { get; }
        event Action<IMonitorDeltaResponse>? OnDelta;
        Task EnsureStartedAsync();
    }
}
