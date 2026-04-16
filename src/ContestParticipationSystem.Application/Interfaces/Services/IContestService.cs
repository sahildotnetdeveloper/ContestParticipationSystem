using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Contests;

namespace ContestParticipationSystem.Application.Interfaces.Services;

public interface IContestService
{
    Task<ContestDetailDto> CreateAsync(Guid adminUserId, ContestCreateRequestDto request, CancellationToken cancellationToken);
    Task<ContestDetailDto> UpdateAsync(Guid contestId, ContestUpdateRequestDto request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid contestId, CancellationToken cancellationToken);
    Task<ContestDetailDto> GetByIdAsync(Guid contestId, CurrentUserContext currentUser, CancellationToken cancellationToken);
    Task<PagedResult<ContestSummaryDto>> GetPagedAsync(CurrentUserContext currentUser, PaginationRequest pagination, CancellationToken cancellationToken);
}
