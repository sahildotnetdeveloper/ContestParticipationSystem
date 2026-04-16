using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Domain.Entities;

public class Question
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContestId { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public int Order { get; set; }

    public Contest? Contest { get; set; }
    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    public ICollection<ParticipationAnswer> ParticipationAnswers { get; set; } = new List<ParticipationAnswer>();
}
