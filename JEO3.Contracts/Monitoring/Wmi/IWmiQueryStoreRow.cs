namespace JEO3.Contracts
{
    public interface IWmiQueryStoreRow
    {
        string Caption { get; }
        string Description { get; }
        ulong Frequency_Object { get; }
        ulong Frequency_PerfTime { get; }
        ulong Frequency_Sys100NS { get; }
        string Name { get; }
        ulong QueryStoreCPUusage { get; }
        ulong QueryStorelogicalreads { get; }
        ulong QueryStorelogicalwrites { get; }
        ulong QueryStorephysicalreads { get; }
        ulong Timestamp_Object { get; }
        ulong Timestamp_PerfTime { get; }
        ulong Timestamp_Sys100NS { get; }
    }
}
