using System;
using System.Collections.Generic;
using System.Text;

namespace JEO3.Monitor.Wmi
{
    public sealed class WmiMonitorSnapshot
    {
        public DateTime SampledAtUtc { get; init; }

        public Dictionary<string, List<object?>> Exceptions { get; init; } = [];
        public Dictionary<string, List<object?>> Memory { get; init; } = [];
        public Dictionary<string, List<object?>> Locks { get; init; } = [];
        public Dictionary<string, List<object?>> Databases { get; init; } = [];
        public Dictionary<string, List<object?>> QueryStore { get; init; } = [];
        public Dictionary<string, List<object?>> WebService { get; init; } = [];

        public Dictionary<string, List<object?>> Processor { get; init; } = [];
        public Dictionary<string, List<object?>> ProcessorInformation { get; init; } = [];
        public Dictionary<string, List<object?>> Process { get; init; } = [];
        public Dictionary<string, List<object?>> ProcessV2 { get; init; } = [];
        public Dictionary<string, List<object?>> Thread { get; init; } = [];
        public Dictionary<string, List<object?>> System { get; init; } = [];

        public Dictionary<string, List<object?>> LogicalDisk { get; init; } = [];
        public Dictionary<string, List<object?>> PhysicalDisk { get; init; } = [];
        public Dictionary<string, List<object?>> PagingFile { get; init; } = [];
        public Dictionary<string, List<object?>> Cache { get; init; } = [];

        public Dictionary<string, List<object?>> NetworkInterface { get; init; } = [];
        public Dictionary<string, List<object?>> TCPv4 { get; init; } = [];
        public Dictionary<string, List<object?>> TCPv6 { get; init; } = [];
        public Dictionary<string, List<object?>> UDPv4 { get; init; } = [];
        public Dictionary<string, List<object?>> UDPv6 { get; init; } = [];

        public Dictionary<string, List<object?>> Server { get; init; } = [];
        public Dictionary<string, List<object?>> ServerWorkQueues { get; init; } = [];
        public Dictionary<string, List<object?>> Redirector { get; init; } = [];

        public Dictionary<string, List<object?>> SMBServerShares { get; init; } = [];
        public Dictionary<string, List<object?>> SMBServerSessions { get; init; } = [];
        public Dictionary<string, List<object?>> SMBClientShares { get; init; } = [];
        public Dictionary<string, List<object?>> SMBClientSessions { get; init; } = [];

        public Dictionary<string, List<object?>> HTTPService { get; init; } = [];
        public Dictionary<string, List<object?>> HTTPServiceRequestQueues { get; init; } = [];

        public Dictionary<string, List<object?>> NETCLRJIT { get; init; } = [];
        public Dictionary<string, List<object?>> NETCLRLoading { get; init; } = [];
        public Dictionary<string, List<object?>> NETCLRLocksAndThreads { get; init; } = [];
        public Dictionary<string, List<object?>> NETCLRRemoting { get; init; } = [];
        public Dictionary<string, List<object?>> NETCLRSecurity { get; init; } = [];
        public Dictionary<string, List<object?>> NETCLRInterop { get; init; } = [];

        public Dictionary<string, List<object?>> ASPNET { get; init; } = [];
        public Dictionary<string, List<object?>> ASPNETApplications { get; init; } = [];

        public Dictionary<string, List<object?>> TerminalServices { get; init; } = [];
        public Dictionary<string, List<object?>> TerminalServicesSession { get; init; } = [];

        public Dictionary<string, List<object?>> DNSClient { get; init; } = [];
        public Dictionary<string, List<object?>> DNS { get; init; } = [];

        public Dictionary<string, List<object?>> IPv4 { get; init; } = [];
        public Dictionary<string, List<object?>> IPv6 { get; init; } = [];

        public Dictionary<string, List<object?>> WFPv4 { get; init; } = [];
        public Dictionary<string, List<object?>> WFPv6 { get; init; } = [];

        public Dictionary<string, List<object?>> Service { get; init; } = [];
        public Dictionary<string, List<object?>> Objects { get; init; } = [];

        public Dictionary<string, List<object?>> JobObject { get; init; } = [];
        public Dictionary<string, List<object?>> Registry { get; init; } = [];

        public Dictionary<string, List<object?>> HyperVHypervisorLogicalProcessor { get; init; } = [];
        public Dictionary<string, List<object?>> HyperVHypervisorVirtualProcessor { get; init; } = [];
        public Dictionary<string, List<object?>> HyperVDynamicMemoryVM { get; init; } = [];
        public Dictionary<string, List<object?>> HyperVVirtualMachineSummary { get; init; } = [];
        public Dictionary<string, List<object?>> HyperVVirtualSwitch { get; init; } = [];
        public Dictionary<string, List<object?>> HyperVVirtualNetworkAdapter { get; init; } = [];

        public Dictionary<string, List<object?>> PrintQueue { get; init; } = [];
        public Dictionary<string, List<object?>> WindowsSearchIndexer { get; init; } = [];

        public Dictionary<string, List<object?>> DFSReplication { get; init; } = [];
        public Dictionary<string, List<object?>> DFSNamespace { get; init; } = [];

        public Dictionary<string, List<object?>> ClusterNode { get; init; } = [];
        public Dictionary<string, List<object?>> ClusterNetwork { get; init; } = [];
        public Dictionary<string, List<object?>> ClusterNetworkInterface { get; init; } = [];
    }
}
