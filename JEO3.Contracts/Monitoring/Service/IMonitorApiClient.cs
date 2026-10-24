namespace JEO3.Contracts
{
    public interface IMonitorApiClient
    {
        Task<IMonitorSnapshot?> GetInitialSnapshotAsync(CancellationToken ct = default);
        Task<IMonitorDeltaResponse?> GetDeltaAsync(DateTime sinceUtc, CancellationToken ct = default);
    }
}
