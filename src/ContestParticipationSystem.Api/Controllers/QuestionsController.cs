using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Questions;
using ContestParticipationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContestParticipationSystem.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = "Admin")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet("contests/{contestId:guid}/questions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<QuestionAdminDto>>>> GetQuestionsByContest(
        Guid contestId,
        CancellationToken cancellationToken)
    {
        var response = await _questionService.GetByContestAsync(contestId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<QuestionAdminDto>>.SuccessResponse(response));
    }

    [HttpPost("contests/{contestId:guid}/questions")]
    public async Task<ActionResult<ApiResponse<QuestionAdminDto>>> CreateQuestion(
        Guid contestId,
        QuestionCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _questionService.CreateAsync(contestId, request, cancellationToken);
        return CreatedAtAction(nameof(GetQuestionsByContest), new { contestId }, ApiResponse<QuestionAdminDto>.SuccessResponse(response, "Question created successfully."));
    }

    [HttpPut("questions/{questionId:guid}")]
    public async Task<ActionResult<ApiResponse<QuestionAdminDto>>> UpdateQuestion(
        Guid questionId,
        QuestionCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _questionService.UpdateAsync(questionId, request, cancellationToken);
        return Ok(ApiResponse<QuestionAdminDto>.SuccessResponse(response, "Question updated successfully."));
    }

    [HttpDelete("questions/{questionId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteQuestion(Guid questionId, CancellationToken cancellationToken)
    {
        await _questionService.DeleteAsync(questionId, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Question deleted successfully."));
    }
}
