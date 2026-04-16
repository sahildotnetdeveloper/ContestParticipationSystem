namespace ContestParticipationSystem.Application.DTOs.Participation;

public class SubmitContestResponseDto
{
    public Guid ParticipationId { get; init; }
    public Guid ContestId { get; init; }
    public int Score { get; init; }
    public DateTime SubmittedAtUtc { get; init; }
}
