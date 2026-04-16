using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Leaderboard;

namespace ContestParticipationSystem.Application.Interfaces.Services;

public interface ILeaderboardService
{
    Task<PagedResult<LeaderboardEntryDto>> GetContestLeaderboardAsync(Guid contestId, PaginationRequest pagination, CancellationToken cancellationToken);
}
