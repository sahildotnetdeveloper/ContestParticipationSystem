namespace ContestParticipationSystem.Application.DTOs.Questions;

public class QuestionDto
{
    public Guid Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public string QuestionType { get; init; } = string.Empty;
    public int Order { get; init; }
    public IReadOnlyCollection<QuestionOptionDto> Options { get; init; } = Array.Empty<QuestionOptionDto>();
}
