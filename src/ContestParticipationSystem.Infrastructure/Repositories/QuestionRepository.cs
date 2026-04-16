using ContestParticipationSystem.Application.Interfaces.Repositories;
using ContestParticipationSystem.Domain.Entities;
using ContestParticipationSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContestParticipationSystem.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _dbContext;

    public QuestionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Question question, CancellationToken cancellationToken)
    {
        await _dbContext.Questions.AddAsync(question, cancellationToken);
    }

    public Task<Question?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken)
    {
        return _dbContext.Questions.SingleOrDefaultAsync(question => question.Id == questionId, cancellationToken);
    }

    public Task<Question?> GetByIdWithOptionsAsync(Guid questionId, CancellationToken cancellationToken)
    {
        return _dbContext.Questions
            .Include(question => question.Options.OrderBy(option => option.SortOrder))
            .SingleOrDefaultAsync(question => question.Id == questionId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Question>> GetByContestIdAsync(Guid contestId, CancellationToken cancellationToken)
    {
        return await _dbContext.Questions
            .AsNoTracking()
            .Include(question => question.Options.OrderBy(option => option.SortOrder))
            .Where(question => question.ContestId == contestId)
            .OrderBy(question => question.Order)
            .ToArrayAsync(cancellationToken);
    }

    public void Delete(Question question)
    {
        _dbContext.Questions.Remove(question);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
