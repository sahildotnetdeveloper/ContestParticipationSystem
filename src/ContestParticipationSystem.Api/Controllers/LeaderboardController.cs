using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Leaderboard;
using ContestParticipationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContestParticipationSystem.Api.Controllers;

[ApiController]
[Route("api/contests/{contestId:guid}/leaderboard")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResult<LeaderboardEntryDto>>>> GetLeaderboard(
        Guid contestId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var response = await _leaderboardService.GetContestLeaderboardAsync(contestId, pagination, cancellationToken);
        return Ok(ApiResponse<PagedResult<LeaderboardEntryDto>>.SuccessResponse(response));
    }
}
