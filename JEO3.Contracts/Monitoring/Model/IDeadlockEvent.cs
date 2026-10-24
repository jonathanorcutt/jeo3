namespace JEO3.Contracts
{
    public interface IDeadlockEvent
    {
        DateTime Timestamp { get; }
        string VictimSessionId { get; }
        IReadOnlyList<IDeadlockParticipant> Participants { get; }
        string RawXml { get; }
    }
}
