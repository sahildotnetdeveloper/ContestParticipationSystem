using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Domain.Entities;

public class Participation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ContestId { get; set; }
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAtUtc { get; set; }
    public int Score { get; set; }
    public ParticipationStatus Status { get; set; } = ParticipationStatus.Started;

    public User? User { get; set; }
    public Contest? Contest { get; set; }
    public ICollection<ParticipationAnswer> Answers { get; set; } = new List<ParticipationAnswer>();
}
