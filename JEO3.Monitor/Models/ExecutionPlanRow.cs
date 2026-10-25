using System.Text.Json.Serialization;
using JEO3.Contracts;

namespace JEO3.Monitor.Models
{

    public sealed class ExecutionPlanRow : IExecutionPlanRow
    {
        [JsonPropertyName("FP")]
        public string FullBatchPlanXml { get; set; } = string.Empty;

        [JsonPropertyName("SP")]
        public string SpecificStatementPlanXml { get; set; } = string.Empty;
    }
}
