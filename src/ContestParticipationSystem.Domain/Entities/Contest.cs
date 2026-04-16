using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Domain.Entities;

public class Contest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public ContestType ContestType { get; set; }
    public string Prize { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }

    public User? CreatedByUser { get; set; }
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<Participation> Participations { get; set; } = new List<Participation>();
}
