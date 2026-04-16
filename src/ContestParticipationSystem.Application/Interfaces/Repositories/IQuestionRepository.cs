using ContestParticipationSystem.Domain.Entities;

namespace ContestParticipationSystem.Application.Interfaces.Repositories;

public interface IQuestionRepository
{
    Task AddAsync(Question question, CancellationToken cancellationToken);
    Task<Question?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken);
    Task<Question?> GetByIdWithOptionsAsync(Guid questionId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Question>> GetByContestIdAsync(Guid contestId, CancellationToken cancellationToken);
    void Delete(Question question);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
