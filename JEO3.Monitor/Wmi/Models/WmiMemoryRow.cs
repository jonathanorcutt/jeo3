using JEO3.Contracts;

namespace JEO3.Monitor.Wmi
{
    public sealed class WmiMemoryRow : IWmiMemoryRow
    {
        public ulong AllocatedBytesPersec { get; set; }
        public string Caption { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ulong FinalizationSurvivors { get; set; }
        public ulong Frequency_Object { get; set; }
        public ulong Frequency_PerfTime { get; set; }
        public ulong Frequency_Sys100NS { get; set; }
        public ulong Gen0heapsize { get; set; }
        public ulong Gen0PromotedBytesPerSec { get; set; }
        public ulong Gen1heapsize { get; set; }
        public ulong Gen1PromotedBytesPerSec { get; set; }
        public ulong Gen2heapsize { get; set; }
        public ulong LargeObjectHeapsize { get; set; }
        public string Name { get; set; } = string.Empty;
        public ulong NumberBytesinallHeaps { get; set; }
        public ulong NumberGCHandles { get; set; }
        public ulong NumberGen0Collections { get; set; }
        public ulong NumberGen1Collections { get; set; }
        public ulong NumberGen2Collections { get; set; }
        public ulong NumberInducedGC { get; set; }
        public ulong NumberofPinnedObjects { get; set; }
        public ulong NumberofSinkBlocksinuse { get; set; }
        public ulong NumberTotalcommittedBytes { get; set; }
        public ulong NumberTotalreservedBytes { get; set; }
        public uint PercentTimeinGC { get; set; }
        public uint PercentTimeinGC_Base { get; set; }
        public ulong ProcessID { get; set; }
        public ulong PromotedFinalizationMemoryfromGen0 { get; set; }
        public ulong PromotedMemoryfromGen0 { get; set; }
        public ulong PromotedMemoryfromGen1 { get; set; }
        public ulong Timestamp_Object { get; set; }
        public ulong Timestamp_PerfTime { get; set; }
        public ulong Timestamp_Sys100NS { get; set; }
    }
}
