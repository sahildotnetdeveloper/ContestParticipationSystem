using ContestParticipationSystem.Application.DTOs.Questions;

namespace ContestParticipationSystem.Application.DTOs.Participation;

public class StartContestResponseDto
{
    public Guid ParticipationId { get; init; }
    public Guid ContestId { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime EndsAtUtc { get; init; }
    public IReadOnlyCollection<QuestionDto> Questions { get; init; } = Array.Empty<QuestionDto>();
}
