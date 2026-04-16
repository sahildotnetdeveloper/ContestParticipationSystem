namespace ContestParticipationSystem.Application.DTOs.Questions;

public class QuestionAdminDto
{
    public Guid Id { get; init; }
    public Guid ContestId { get; init; }
    public string Text { get; init; } = string.Empty;
    public string QuestionType { get; init; } = string.Empty;
    public int Order { get; init; }
    public IReadOnlyCollection<QuestionAdminOptionDto> Options { get; init; } = Array.Empty<QuestionAdminOptionDto>();
}
