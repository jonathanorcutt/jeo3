namespace JEO3.Monitor.Wmi
{
    public sealed class WmiLockRow
    {
        public UInt64 AverageWaitTimems { get; set; }
        public UInt32 AverageWaitTimems_Base { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public UInt64 Frequency_Object { get; set; }
        public UInt64 Frequency_PerfTime { get; set; }
        public UInt64 Frequency_Sys100NS { get; set; }
        public UInt64 LockRequestsPersec { get; set; }
        public UInt64 LockTimeoutsPersec { get; set; }
        public UInt64 LockTimeoutstimeout0Persec { get; set; }
        public UInt64 LockWaitsPersec { get; set; }
        public UInt64 LockWaitTimems { get; set; }
        public String Name { get; set; }
        public UInt64 NumberofDeadlocksPersec { get; set; }
        public UInt64 Timestamp_Object { get; set; }
        public UInt64 Timestamp_PerfTime { get; set; }
        public UInt64 Timestamp_Sys100NS { get; set; }
    }

    public interface IWmiLockRow
    {
        ulong AverageWaitTimems { get; set; }
        uint AverageWaitTimems_Base { get; set; }
        string Caption { get; set; }
        string Description { get; set; }
        ulong Frequency_Object { get; set; }
        ulong Frequency_PerfTime { get; set; }
        ulong Frequency_Sys100NS { get; set; }
        ulong LockRequestsPersec { get; set; }
        ulong LockTimeoutsPersec { get; set; }
        ulong LockTimeoutstimeout0Persec { get; set; }
        ulong LockWaitsPersec { get; set; }
        ulong LockWaitTimems { get; set; }
        string Name { get; set; }
        ulong NumberofDeadlocksPersec { get; set; }
        ulong Timestamp_Object { get; set; }
        ulong Timestamp_PerfTime { get; set; }
        ulong Timestamp_Sys100NS { get; set; }
    }
}
