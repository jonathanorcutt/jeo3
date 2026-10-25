namespace JEO3.Core
{
    public record LoginRequest(string Username, string Password);
    public record TokenResponse(string Token);
    public record DataPayload(string Name, decimal Value);
}
