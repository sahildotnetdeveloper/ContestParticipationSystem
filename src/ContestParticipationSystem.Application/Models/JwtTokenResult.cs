namespace ContestParticipationSystem.Application.Models;

public class JwtTokenResult
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}
