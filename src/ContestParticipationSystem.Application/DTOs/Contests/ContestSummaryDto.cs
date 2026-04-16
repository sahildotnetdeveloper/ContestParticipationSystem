namespace ContestParticipationSystem.Application.DTOs.Contests;

public class ContestSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime StartTimeUtc { get; init; }
    public DateTime EndTimeUtc { get; init; }
    public string ContestType { get; init; } = string.Empty;
    public string Prize { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool CanParticipate { get; init; }
}
