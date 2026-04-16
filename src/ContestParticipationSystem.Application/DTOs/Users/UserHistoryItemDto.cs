namespace ContestParticipationSystem.Application.DTOs.Users;

public class UserHistoryItemDto
{
    public Guid ContestId { get; init; }
    public string ContestName { get; init; } = string.Empty;
    public int Score { get; init; }
    public DateTime SubmittedAtUtc { get; init; }
    public string? PrizeWon { get; init; }
}
