using JEO3.Contracts;

namespace JEO3.Monitor.Wmi
{
    public sealed class WmiWebServiceRow : IWmiWebServiceRow
    {
        public uint AnonymousUsersPersec { get; set; }
        public ulong BytesReceivedPersec { get; set; }
        public ulong BytesSentPersec { get; set; }
        public ulong BytesTotalPersec { get; set; }
        public string Caption { get; set; } = string.Empty;
        public uint CGIRequestsPersec { get; set; }
        public uint ConnectionAttemptsPersec { get; set; }
        public uint CopyRequestsPersec { get; set; }
        public uint CurrentAnonymousUsers { get; set; }
        public uint CurrentBlockedAsyncIORequests { get; set; }
        public uint Currentblockedbandwidthbytes { get; set; }
        public uint CurrentCALcountforauthenticatedusers { get; set; }
        public uint CurrentCALcountforSSLconnections { get; set; }
        public uint CurrentCGIRequests { get; set; }
        public uint CurrentConnections { get; set; }
        public uint CurrentISAPIExtensionRequests { get; set; }
        public uint CurrentNonAnonymousUsers { get; set; }
        public uint DeleteRequestsPersec { get; set; }
        public string Description { get; set; } = string.Empty;
        public uint FilesPersec { get; set; }
        public uint FilesReceivedPersec { get; set; }
        public uint FilesSentPersec { get; set; }
        public ulong Frequency_Object { get; set; }
        public ulong Frequency_PerfTime { get; set; }
        public ulong Frequency_Sys100NS { get; set; }
        public uint GetRequestsPersec { get; set; }
        public uint HeadRequestsPersec { get; set; }
        public uint ISAPIExtensionRequestsPersec { get; set; }
        public uint LockedErrorsPersec { get; set; }
        public uint LockRequestsPersec { get; set; }
        public uint LogonAttemptsPersec { get; set; }
        public uint MaximumAnonymousUsers { get; set; }
        public uint MaximumCALcountforauthenticatedusers { get; set; }
        public uint MaximumCALcountforSSLconnections { get; set; }
        public uint MaximumCGIRequests { get; set; }
        public uint MaximumConnections { get; set; }
        public uint MaximumISAPIExtensionRequests { get; set; }
        public uint MaximumNonAnonymousUsers { get; set; }
        public uint MeasuredAsyncIOBandwidthUsage { get; set; }
        public uint MkcolRequestsPersec { get; set; }
        public uint MoveRequestsPersec { get; set; }
        public string Name { get; set; } = string.Empty;
        public uint NonAnonymousUsersPersec { get; set; }
        public uint NotFoundErrorsPersec { get; set; }
        public uint OptionsRequestsPersec { get; set; }
        public uint OtherRequestMethodsPersec { get; set; }
        public uint PostRequestsPersec { get; set; }
        public uint PropfindRequestsPersec { get; set; }
        public uint ProppatchRequestsPersec { get; set; }
        public uint PutRequestsPersec { get; set; }
        public uint SearchRequestsPersec { get; set; }
        public uint ServiceUptime { get; set; }
        public ulong Timestamp_Object { get; set; }
        public ulong Timestamp_PerfTime { get; set; }
        public ulong Timestamp_Sys100NS { get; set; }
        public uint TotalAllowedAsyncIORequests { get; set; }
        public uint TotalAnonymousUsers { get; set; }
        public uint TotalBlockedAsyncIORequests { get; set; }
        public uint Totalblockedbandwidthbytes { get; set; }
        public ulong TotalBytesReceived { get; set; }
        public ulong TotalBytesSent { get; set; }
        public ulong TotalBytesTransferred { get; set; }
        public uint TotalCGIRequests { get; set; }
        public uint TotalConnectionAttemptsallinstances { get; set; }
        public uint TotalCopyRequests { get; set; }
        public uint TotalcountoffailedCALrequestsforauthenticatedusers { get; set; }
        public uint TotalcountoffailedCALrequestsforSSLconnections { get; set; }
        public uint TotalDeleteRequests { get; set; }
        public uint TotalFilesReceived { get; set; }
        public uint TotalFilesSent { get; set; }
        public uint TotalFilesTransferred { get; set; }
        public uint TotalGetRequests { get; set; }
        public uint TotalHeadRequests { get; set; }
        public uint TotalISAPIExtensionRequests { get; set; }
        public uint TotalLockedErrors { get; set; }
        public uint TotalLockRequests { get; set; }
        public uint TotalLogonAttempts { get; set; }
        public uint TotalMethodRequests { get; set; }
        public uint TotalMethodRequestsPersec { get; set; }
        public uint TotalMkcolRequests { get; set; }
        public uint TotalMoveRequests { get; set; }
        public uint TotalNonAnonymousUsers { get; set; }
        public uint TotalNotFoundErrors { get; set; }
        public uint TotalOptionsRequests { get; set; }
        public uint TotalOtherRequestMethods { get; set; }
        public uint TotalPostRequests { get; set; }
        public uint TotalPropfindRequests { get; set; }
        public uint TotalProppatchRequests { get; set; }
        public uint TotalPutRequests { get; set; }
        public uint TotalRejectedAsyncIORequests { get; set; }
        public uint TotalSearchRequests { get; set; }
        public uint TotalTraceRequests { get; set; }
        public uint TotalUnlockRequests { get; set; }
        public uint TraceRequestsPersec { get; set; }
        public uint UnlockRequestsPersec { get; set; }
    }
}
