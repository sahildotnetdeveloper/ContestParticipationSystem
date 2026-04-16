using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.NormalUser;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Contest> CreatedContests { get; set; } = new List<Contest>();
    public ICollection<Participation> Participations { get; set; } = new List<Participation>();
}
