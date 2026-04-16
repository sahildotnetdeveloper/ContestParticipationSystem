namespace ContestParticipationSystem.Domain.Entities;

public class QuestionOption
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }

    public Question? Question { get; set; }
}
