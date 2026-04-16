using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Participation;

namespace ContestParticipationSystem.Application.Interfaces.Services;

public interface IParticipationService
{
    Task<StartContestResponseDto> StartContestAsync(Guid userId, CurrentUserContext currentUser, Guid contestId, CancellationToken cancellationToken);
    Task<SubmitContestResponseDto> SubmitAsync(Guid userId, CurrentUserContext currentUser, Guid contestId, SubmitContestRequestDto request, CancellationToken cancellationToken);
}
