namespace ContestParticipationSystem.Application.DTOs.Questions;

public class QuestionOptionDto
{
    public Guid Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public int SortOrder { get; init; }
}
