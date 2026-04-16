using ContestParticipationSystem.Api.Extensions;
using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Participation;
using ContestParticipationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContestParticipationSystem.Api.Controllers;

[ApiController]
[Route("api/contests/{contestId:guid}/participation")]
[Authorize(Roles = "Admin,VipUser,NormalUser")]
public class ParticipationController : ControllerBase
{
    private readonly IParticipationService _participationService;

    public ParticipationController(IParticipationService participationService)
    {
        _participationService = participationService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<ApiResponse<StartContestResponseDto>>> StartContest(
        Guid contestId,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _participationService.StartContestAsync(
            currentUser.UserId!.Value,
            currentUser,
            contestId,
            cancellationToken);

        return Ok(ApiResponse<StartContestResponseDto>.SuccessResponse(response, "Contest started successfully."));
    }

    [HttpPost("submit")]
    public async Task<ActionResult<ApiResponse<SubmitContestResponseDto>>> SubmitContest(
        Guid contestId,
        SubmitContestRequestDto request,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _participationService.SubmitAsync(
            currentUser.UserId!.Value,
            currentUser,
            contestId,
            request,
            cancellationToken);

        return Ok(ApiResponse<SubmitContestResponseDto>.SuccessResponse(response, "Contest submitted successfully."));
    }
}
