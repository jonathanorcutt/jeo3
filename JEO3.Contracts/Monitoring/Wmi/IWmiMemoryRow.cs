namespace JEO3.Contracts
{
    public interface IWmiMemoryRow
    {
        ulong AllocatedBytesPersec { get; }
        string Caption { get; }
        string Description { get; }
        ulong FinalizationSurvivors { get; }
        ulong Frequency_Object { get; }
        ulong Frequency_PerfTime { get; }
        ulong Frequency_Sys100NS { get; }
        ulong Gen0heapsize { get; }
        ulong Gen0PromotedBytesPerSec { get; }
        ulong Gen1heapsize { get; }
        ulong Gen1PromotedBytesPerSec { get; }
        ulong Gen2heapsize { get; }
        ulong LargeObjectHeapsize { get; }
        string Name { get; }
        ulong NumberBytesinallHeaps { get; }
        ulong NumberGCHandles { get; }
        ulong NumberGen0Collections { get; }
        ulong NumberGen1Collections { get; }
        ulong NumberGen2Collections { get; }
        ulong NumberInducedGC { get; }
        ulong NumberofPinnedObjects { get; }
        ulong NumberofSinkBlocksinuse { get; }
        ulong NumberTotalcommittedBytes { get; }
        ulong NumberTotalreservedBytes { get; }
        uint PercentTimeinGC { get; }
        uint PercentTimeinGC_Base { get; }
        ulong ProcessID { get; }
        ulong PromotedFinalizationMemoryfromGen0 { get; }
        ulong PromotedMemoryfromGen0 { get; }
        ulong PromotedMemoryfromGen1 { get; }
        ulong Timestamp_Object { get; }
        ulong Timestamp_PerfTime { get; }
        ulong Timestamp_Sys100NS { get; }
    }
}
