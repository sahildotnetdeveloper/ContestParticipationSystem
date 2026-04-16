using ContestParticipationSystem.Api.Extensions;
using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Users;
using ContestParticipationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContestParticipationSystem.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserHistoryService _userHistoryService;

    public UsersController(IUserHistoryService userHistoryService)
    {
        _userHistoryService = userHistoryService;
    }

    [HttpGet("me/history")]
    [Authorize(Roles = "Admin,VipUser,NormalUser")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserHistoryItemDto>>>> GetMyHistory(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var currentUser = User.GetCurrentUser();
        var response = await _userHistoryService.GetHistoryAsync(currentUser.UserId!.Value, pagination, cancellationToken);
        return Ok(ApiResponse<PagedResult<UserHistoryItemDto>>.SuccessResponse(response));
    }

    [HttpPatch("{userId:guid}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateRole(
        Guid userId,
        UpdateUserRoleRequestDto request,
        CancellationToken cancellationToken)
    {
        await _userHistoryService.UpdateRoleAsync(userId, request.Role, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "User role updated successfully."));
    }
}
