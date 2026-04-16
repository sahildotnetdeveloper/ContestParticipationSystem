using ContestParticipationSystem.Application.DTOs.Questions;

namespace ContestParticipationSystem.Application.Interfaces.Services;

public interface IQuestionService
{
    Task<QuestionAdminDto> CreateAsync(Guid contestId, QuestionCreateRequestDto request, CancellationToken cancellationToken);
    Task<QuestionAdminDto> UpdateAsync(Guid questionId, QuestionCreateRequestDto request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid questionId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<QuestionAdminDto>> GetByContestAsync(Guid contestId, CancellationToken cancellationToken);
}
