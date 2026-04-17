using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Leaderboard;
using ContestParticipationSystem.Application.DTOs.Users;
using ContestParticipationSystem.Domain.Entities;

namespace ContestParticipationSystem.Application.Interfaces.Repositories;

public interface IParticipationRepository
{
    Task AddAsync(Participation participation, CancellationToken cancellationToken);
    Task AddAnswersAsync(IEnumerable<ParticipationAnswer> answers, CancellationToken cancellationToken);
    Task<Participation?> GetByUserAndContestAsync(Guid userId, Guid contestId, CancellationToken cancellationToken);
    Task<Participation?> GetForSubmissionAsync(Guid userId, Guid contestId, CancellationToken cancellationToken);
    Task RemoveAnswersAsync(IEnumerable<ParticipationAnswer> answers, CancellationToken cancellationToken);
    Task<PagedResult<LeaderboardEntryDto>> GetLeaderboardAsync(Guid contestId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<UserHistoryItemDto>> GetUserHistoryAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
