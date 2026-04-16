using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Users;
using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Application.Interfaces.Services;

public interface IUserHistoryService
{
    Task<PagedResult<UserHistoryItemDto>> GetHistoryAsync(Guid userId, PaginationRequest pagination, CancellationToken cancellationToken);
    Task UpdateRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken);
}
