namespace ContestParticipationSystem.Application.DTOs.Leaderboard;

public class LeaderboardEntryDto
{
    public int Rank { get; init; }
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public int Score { get; init; }
    public DateTime SubmittedAtUtc { get; init; }
}
