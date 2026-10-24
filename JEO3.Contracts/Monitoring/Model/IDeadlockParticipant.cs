namespace JEO3.Contracts
{
    public interface IDeadlockParticipant
    {
        DateTime Timestamp { get; set; }
        int SessionId { get; }
        string LoginName { get; }
        string HostName { get; }
        string ProgramName { get; }
        string LastStatement { get; }
        bool IsVictim { get; }
    }
}
