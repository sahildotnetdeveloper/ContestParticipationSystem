using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Leaderboard;
using ContestParticipationSystem.Application.Exceptions;
using ContestParticipationSystem.Application.Interfaces.Repositories;
using ContestParticipationSystem.Application.Interfaces.Services;

namespace ContestParticipationSystem.Application.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly IContestRepository _contestRepository;
    private readonly IParticipationRepository _participationRepository;

    public LeaderboardService(IContestRepository contestRepository, IParticipationRepository participationRepository)
    {
        _contestRepository = contestRepository;
        _participationRepository = participationRepository;
    }

    public async Task<PagedResult<LeaderboardEntryDto>> GetContestLeaderboardAsync(Guid contestId, PaginationRequest pagination, CancellationToken cancellationToken)
    {
        var contest = await _contestRepository.GetByIdAsync(contestId, cancellationToken);
        if (contest is null)
        {
            throw new NotFoundException("Contest not found.");
        }

        return await _participationRepository.GetLeaderboardAsync(contestId, pagination.PageNumber, pagination.PageSize, cancellationToken);
    }
}
