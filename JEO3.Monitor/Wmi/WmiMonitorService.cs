using System.Management;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace JEO3.Monitor.Wmi
{
    public sealed class WmiMonitorService(
        WmiMonitorStore store,
        ILogger<WmiMonitorService> logger) : BackgroundService
    {
        private const string Namespace = @"root\cimv2";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    store.Update(new WmiMonitorSnapshot
                    {
                        SampledAtUtc = DateTime.UtcNow,
                        Exceptions = Query("Win32_PerfRawData_NETFramework_NETCLRExceptions"),
                        Memory = Query("Win32_PerfRawData_NETFramework_NETCLRMemory"),
                        Locks = Query("Win32_PerfRawData_MSSQLSERVER_SQLServerLocks"),
                        Databases = Query("Win32_PerfRawData_MSSQLSERVER_SQLServerDatabases"),
                        QueryStore = Query("Win32_PerfRawData_MSSQLSERVER_SQLServerQueryStore"),
                        WebService = Query("Win32_PerfRawData_W3SVC_WebService")
                    });
                    //store.Update(new WmiMonitorSnapshot
                    //{
                    //    SampledAtUtc = DateTime.UtcNow,

                    //    Exceptions = Query("Win32_PerfRawData_NETFramework_NETCLRExceptions"),
                    //    Memory = Query("Win32_PerfRawData_PerfOS_Memory"),
                    //    Locks = Query("Win32_PerfRawData_MSSQLSERVER_SQLServerLocks"),
                    //    Databases = Query("Win32_PerfRawData_MSSQLSERVER_SQLServerDatabases"),
                    //    QueryStore = Query("Win32_PerfRawData_MSSQLSERVER_SQLServerQueryStore"),
                    //    WebService = Query("Win32_PerfRawData_W3SVC_WebService"),

                    //    Processor = Query("Win32_PerfRawData_PerfOS_Processor"),
                    //    ProcessorInformation = Query("Win32_PerfRawData_PerfOS_ProcessorInformation"),
                    //    Process = Query("Win32_PerfRawData_PerfProc_Process"),
                    //    ProcessV2 = Query("Win32_PerfRawData_PerfProc_ProcessV2"),
                    //    Thread = Query("Win32_PerfRawData_PerfProc_Thread"),
                    //    System = Query("Win32_PerfRawData_PerfOS_System"),

                    //    LogicalDisk = Query("Win32_PerfRawData_PerfDisk_LogicalDisk"),
                    //    PhysicalDisk = Query("Win32_PerfRawData_PerfDisk_PhysicalDisk"),
                    //    PagingFile = Query("Win32_PerfRawData_PerfOS_PagingFile"),
                    //    Cache = Query("Win32_PerfRawData_PerfOS_CacheStore"),

                    //    NetworkInterface = Query("Win32_PerfRawData_Tcpip_NetworkInterface"),
                    //    TCPv4 = Query("Win32_PerfRawData_Tcpip_TCPv4"),
                    //    TCPv6 = Query("Win32_PerfRawData_Tcpip_TCPv6"),
                    //    UDPv4 = Query("Win32_PerfRawData_Tcpip_UDPv4"),
                    //    UDPv6 = Query("Win32_PerfRawData_Tcpip_UDPv6"),

                    //    Server = Query("Win32_PerfRawData_PerfNet_Server"),
                    //    ServerWorkQueues = Query("Win32_PerfRawData_PerfNet_ServerWorkQueues"),
                    //    Redirector = Query("Win32_PerfRawData_PerfNet_Redirector"),

                    //    SMBServerShares = Query("Win32_PerfRawData_PerfSMB_ServerShares"),
                    //    SMBServerSessions = Query("Win32_PerfRawData_PerfSMB_ServerSessions"),
                    //    SMBClientShares = Query("Win32_PerfRawData_PerfSMB_ClientShares"),
                    //    SMBClientSessions = Query("Win32_PerfRawData_PerfSMB_ClientSessions"),

                    //    HTTPService = Query("Win32_PerfRawData_HTTPService_HTTPService"),
                    //    HTTPServiceRequestQueues = Query("Win32_PerfRawData_HTTPService_HTTPServiceRequestQueues"),

                    //    NETCLRJIT = Query("Win32_PerfRawData_NETFramework_NETCLRJit"),
                    //    NETCLRLoading = Query("Win32_PerfRawData_NETFramework_NETCLRLoading"),
                    //    NETCLRLocksAndThreads = Query("Win32_PerfRawData_NETFramework_NETCLRLocksAndThreads"),
                    //    NETCLRRemoting = Query("Win32_PerfRawData_NETFramework_NETCLRRemoting"),
                    //    NETCLRSecurity = Query("Win32_PerfRawData_NETFramework_NETCLRSecurity"),
                    //    NETCLRInterop = Query("Win32_PerfRawData_NETFramework_NETCLRInterop"),

                    //    ASPNET = Query("Win32_PerfRawData_ASPNET_ASPNET"),
                    //    ASPNETApplications = Query("Win32_PerfRawData_ASPNET_ASPNETApplications"),

                    //    TerminalServices = Query("Win32_PerfRawData_TermService_TerminalServices"),
                    //    TerminalServicesSession = Query("Win32_PerfRawData_TermService_TerminalServicesSession"),

                    //    DNSClient = Query("Win32_PerfRawData_DNSClient_DNSClient"),
                    //    DNS = Query("Win32_PerfRawData_DNS_DNS"),

                    //    IPv4 = Query("Win32_PerfRawData_Tcpip_IPv4"),
                    //    IPv6 = Query("Win32_PerfRawData_Tcpip_IPv6"),

                    //    WFPv4 = Query("Win32_PerfRawData_WFP_WFPv4"),
                    //    WFPv6 = Query("Win32_PerfRawData_WFP_WFPv6"),

                    //    Service = Query("Win32_PerfRawData_PerfOS_Service"),
                    //    Objects = Query("Win32_PerfRawData_PerfOS_Objects"),

                    //    JobObject = Query("Win32_PerfRawData_PerfProc_JobObject"),
                    //    Registry = Query("Win32_PerfRawData_PerfOS_Registry"),

                    //    HyperVHypervisorLogicalProcessor = Query("Win32_PerfRawData_HvStats_HyperVHypervisorLogicalProcessor"),
                    //    HyperVHypervisorVirtualProcessor = Query("Win32_PerfRawData_HvStats_HyperVHypervisorVirtualProcessor"),
                    //    HyperVDynamicMemoryVM = Query("Win32_PerfRawData_HvStats_HyperVDynamicMemoryVM"),
                    //    HyperVVirtualMachineSummary = Query("Win32_PerfRawData_HvStats_HyperVVirtualMachineSummary"),
                    //    HyperVVirtualSwitch = Query("Win32_PerfRawData_HvStats_HyperVVirtualSwitch"),
                    //    HyperVVirtualNetworkAdapter = Query("Win32_PerfRawData_HvStats_HyperVVirtualNetworkAdapter"),

                    //    PrintQueue = Query("Win32_PerfRawData_Spooler_PrintQueue"),
                    //    WindowsSearchIndexer = Query("Win32_PerfRawData_PerfOS_WindowsSearchIndexer"),

                    //    DFSReplication = Query("Win32_PerfRawData_DFSR_DFSR"),
                    //    DFSNamespace = Query("Win32_PerfRawData_DFSNamespace_DFSNamespace"),

                    //    ClusterNode = Query("Win32_PerfRawData_FailoverClustering_ClusterNode"),
                    //    ClusterNetwork = Query("Win32_PerfRawData_FailoverClustering_ClusterNetwork"),
                    //    ClusterNetworkInterface = Query("Win32_PerfRawData_FailoverClustering_ClusterNetworkInterface")
                    //});
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "WMI monitor polling failed.");
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

        private static Dictionary<string, List<object?>> Query(string className)
        {
            using var searcher = new ManagementObjectSearcher(
                Namespace,
                $"SELECT * FROM {className}");

            using var results = searcher.Get();

            var columns = new Dictionary<string, List<object?>>();

            foreach (ManagementObject item in results)
            {
                foreach (PropertyData property in item.Properties)
                {
                    if (!columns.TryGetValue(property.Name, out var values))
                    {
                        values = [];
                        columns[property.Name] = values;
                    }

                    values.Add(property.Value);
                }
            }

            return columns;
        }
    }
}