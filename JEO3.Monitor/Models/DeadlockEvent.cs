using System.Text.Json.Serialization;
using JEO3.Contracts;

namespace JEO3.Monitor
{
    public sealed class DeadlockEvent : IDeadlockEvent
    {
        [JsonPropertyName("T")]
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("V")]
        public string VictimSessionId { get; init; }
        public IReadOnlyList<IDeadlockParticipant> Participants { get; init; } = []; // Added explicit name 'Participants'
        [JsonPropertyName("R")]
        public string RawXml { get; init; } = "";
    }
}
