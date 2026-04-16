using ContestParticipationSystem.Application.Models;
using ContestParticipationSystem.Domain.Entities;

namespace ContestParticipationSystem.Application.Interfaces.Security;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(User user);
}
