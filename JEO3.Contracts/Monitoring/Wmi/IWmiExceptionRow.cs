namespace JEO3.Contracts
{
    public interface IWmiExceptionRow
    {
        string Caption { get; }
        string Description { get; }
        ulong Frequency_Object { get; }
        ulong Frequency_PerfTime { get; }
        ulong Frequency_Sys100NS { get; }
        string Name { get; }
        uint NumberofExcepsThrown { get; }
        uint NumberofExcepsThrownPersec { get; }
        uint NumberofFiltersPersec { get; }
        uint NumberofFinallysPersec { get; }
        uint ThrowToCatchDepthPersec { get; }
        ulong Timestamp_Object { get; }
        ulong Timestamp_PerfTime { get; }
        ulong Timestamp_Sys100NS { get; }
    }
}
