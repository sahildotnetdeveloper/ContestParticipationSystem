using ContestParticipationSystem.Application.Interfaces.Repositories;
using ContestParticipationSystem.Domain.Entities;
using ContestParticipationSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContestParticipationSystem.Infrastructure.Repositories;

public class ContestRepository : IContestRepository
{
    private readonly AppDbContext _dbContext;

    public ContestRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Contest contest, CancellationToken cancellationToken)
    {
        await _dbContext.Contests.AddAsync(contest, cancellationToken);
    }

    public Task<Contest?> GetByIdAsync(Guid contestId, CancellationToken cancellationToken)
    {
        return _dbContext.Contests.SingleOrDefaultAsync(contest => contest.Id == contestId, cancellationToken);
    }

    public Task<Contest?> GetWithQuestionsAsync(Guid contestId, CancellationToken cancellationToken)
    {
        return _dbContext.Contests
            .Include(contest => contest.Questions)
            .SingleOrDefaultAsync(contest => contest.Id == contestId, cancellationToken);
    }

    public Task<Contest?> GetForParticipationAsync(Guid contestId, CancellationToken cancellationToken)
    {
        return _dbContext.Contests
            .Include(contest => contest.Questions.OrderBy(question => question.Order))
            .ThenInclude(question => question.Options.OrderBy(option => option.SortOrder))
            .SingleOrDefaultAsync(contest => contest.Id == contestId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Contest>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _dbContext.Contests
            .AsNoTracking()
            .OrderByDescending(contest => contest.StartTimeUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Contests.CountAsync(cancellationToken);
    }

    public void Delete(Contest contest)
    {
        _dbContext.Contests.Remove(contest);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
