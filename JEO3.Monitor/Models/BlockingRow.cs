using System.Runtime.Intrinsics.X86;
using System.Text.Json.Serialization;
using JEO3.Contracts;
using Microsoft.Data.SqlClient;

namespace JEO3.Monitor
{
    public sealed class BlockingRow : IBlockingRow
    {
        [JsonPropertyName("T")]
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("S")]
        public int SessionId { get; set; }
        [JsonPropertyName("B")]
        public int BlockingSessionId { get; set; }
        [JsonPropertyName("IB")]
        public bool IsBlocked { get; set; }
        [JsonPropertyName("ST")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("L")]
        public string LoginName { get; set; } = string.Empty;
        [JsonPropertyName("H")]
        public string HostName { get; set; } = string.Empty;
        [JsonPropertyName("P")]
        public string ProgramName { get; set; } = string.Empty;
        [JsonPropertyName("C")]
        public string Command { get; set; } = string.Empty;
        [JsonPropertyName("W")]
        public string WaitType { get; set; } = string.Empty;
        [JsonPropertyName("WT")]
        public double WaitTimeSec { get; set; }
        [JsonPropertyName("R")]
        public DateTime? RequestStartTime { get; set; }
        [JsonPropertyName("TR")]
        public DateTime? TransactionBeginTime { get; set; }
        [JsonPropertyName("O")]
        public double? OpenTranDurationSec { get; set; }
        [JsonPropertyName("Q")]
        public string QueryText { get; set; } = string.Empty;
        [JsonPropertyName("I")]
        public bool IsVictim { get; set; }
        [JsonPropertyName("WR")]
        public string? WaitResource { get; set; }
        [JsonPropertyName("IL")]
        public string? IsolationLevel { get; set; }
        [JsonPropertyName("MG")]
        public double? MemoryGrantMb { get; set; } // Changed to double? because FLOAT maps to double in C#

        // Handle raw VARBINARY columns safely
        [JsonInclude]
        [JsonPropertyName("SH")]
        public string? SqlHandle { get; set; }
        [JsonInclude]
        [JsonPropertyName("PH")]
        public string? PlanHandle { get; set; }

        // Allow nulls for safe fallback when processing idle sessions
        [JsonPropertyName("SSO")]
        public int? StatementStartOffset { get; set; }
        [JsonPropertyName("SEO")]
        public int? StatementEndOffset { get; set; }  
    }
}
