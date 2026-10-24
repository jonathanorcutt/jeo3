using JEO3.Contracts;

namespace JEO3.Monitor.Wmi
{
    public sealed class WmiQueryStoreRow : IWmiQueryStoreRow
    {
        public string Caption { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ulong Frequency_Object { get; set; }
        public ulong Frequency_PerfTime { get; set; }
        public ulong Frequency_Sys100NS { get; set; }
        public string Name { get; set; } = string.Empty;
        public ulong QueryStoreCPUusage { get; set; }
        public ulong QueryStorelogicalreads { get; set; }
        public ulong QueryStorelogicalwrites { get; set; }
        public ulong QueryStorephysicalreads { get; set; }
        public ulong Timestamp_Object { get; set; }
        public ulong Timestamp_PerfTime { get; set; }
        public ulong Timestamp_Sys100NS { get; set; }
    }
}
