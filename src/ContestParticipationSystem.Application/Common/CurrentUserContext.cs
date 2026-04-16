using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Application.Common;

public class CurrentUserContext
{
    public Guid? UserId { get; init; }
    public UserRole? Role { get; init; }
    public bool IsAuthenticated => UserId.HasValue && Role.HasValue;
}
