using System.Security.Claims;
using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static CurrentUserContext GetCurrentUser(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var roleClaim = principal.FindFirstValue(ClaimTypes.Role);

        if (!Guid.TryParse(userIdClaim, out var userId) || !Enum.TryParse<UserRole>(roleClaim, out var role))
        {
            return new CurrentUserContext();
        }

        return new CurrentUserContext
        {
            UserId = userId,
            Role = role
        };
    }
}
