namespace ContestParticipationSystem.Domain.Entities;

public class ParticipationAnswer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ParticipationId { get; set; }
    public Guid QuestionId { get; set; }
    public string AnswerOptionIdsJson { get; set; } = "[]";
    public bool IsCorrect { get; set; }
    public int AwardedPoints { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;

    public Participation? Participation { get; set; }
    public Question? Question { get; set; }
}
