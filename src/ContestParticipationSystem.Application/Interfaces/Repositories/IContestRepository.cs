using ContestParticipationSystem.Domain.Entities;

namespace ContestParticipationSystem.Application.Interfaces.Repositories;

public interface IContestRepository
{
    Task AddAsync(Contest contest, CancellationToken cancellationToken);
    Task<Contest?> GetByIdAsync(Guid contestId, CancellationToken cancellationToken);
    Task<Contest?> GetWithQuestionsAsync(Guid contestId, CancellationToken cancellationToken);
    Task<Contest?> GetForParticipationAsync(Guid contestId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Contest>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    void Delete(Contest contest);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
