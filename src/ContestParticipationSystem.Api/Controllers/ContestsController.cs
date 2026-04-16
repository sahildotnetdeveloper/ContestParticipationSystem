using ContestParticipationSystem.Api.Extensions;
using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Contests;
using ContestParticipationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContestParticipationSystem.Api.Controllers;

[ApiController]
[Route("api/contests")]
public class ContestsController : ControllerBase
{
    private readonly IContestService _contestService;

    public ContestsController(IContestService contestService)
    {
        _contestService = contestService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResult<ContestSummaryDto>>>> GetContests(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var response = await _contestService.GetPagedAsync(User.GetCurrentUser(), pagination, cancellationToken);
        return Ok(ApiResponse<PagedResult<ContestSummaryDto>>.SuccessResponse(response));
    }

    [HttpGet("{contestId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ContestDetailDto>>> GetContest(Guid contestId, CancellationToken cancellationToken)
    {
        var response = await _contestService.GetByIdAsync(contestId, User.GetCurrentUser(), cancellationToken);
        return Ok(ApiResponse<ContestDetailDto>.SuccessResponse(response));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ContestDetailDto>>> CreateContest(
        ContestCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _contestService.CreateAsync(currentUser.UserId!.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetContest), new { contestId = response.Id }, ApiResponse<ContestDetailDto>.SuccessResponse(response, "Contest created successfully."));
    }

    [HttpPut("{contestId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ContestDetailDto>>> UpdateContest(
        Guid contestId,
        ContestUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _contestService.UpdateAsync(contestId, request, cancellationToken);
        return Ok(ApiResponse<ContestDetailDto>.SuccessResponse(response, "Contest updated successfully."));
    }

    [HttpDelete("{contestId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteContest(Guid contestId, CancellationToken cancellationToken)
    {
        await _contestService.DeleteAsync(contestId, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Contest deleted successfully."));
    }
}
