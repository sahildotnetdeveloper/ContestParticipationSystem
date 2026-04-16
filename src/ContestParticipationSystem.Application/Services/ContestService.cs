using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Contests;
using ContestParticipationSystem.Application.Exceptions;
using ContestParticipationSystem.Application.Interfaces.Repositories;
using ContestParticipationSystem.Application.Interfaces.Services;
using ContestParticipationSystem.Domain.Entities;
using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Application.Services;

public class ContestService : IContestService
{
    private readonly IContestRepository _contestRepository;

    public ContestService(IContestRepository contestRepository)
    {
        _contestRepository = contestRepository;
    }

    public async Task<ContestDetailDto> CreateAsync(Guid adminUserId, ContestCreateRequestDto request, CancellationToken cancellationToken)
    {
        ValidateContestWindow(request.StartTimeUtc, request.EndTimeUtc);

        var contest = new Contest
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            StartTimeUtc = DateTime.SpecifyKind(request.StartTimeUtc, DateTimeKind.Utc),
            EndTimeUtc = DateTime.SpecifyKind(request.EndTimeUtc, DateTimeKind.Utc),
            ContestType = request.ContestType,
            Prize = request.Prize.Trim(),
            CreatedByUserId = adminUserId
        };

        await _contestRepository.AddAsync(contest, cancellationToken);
        await _contestRepository.SaveChangesAsync(cancellationToken);

        return MapContestDetail(contest, new CurrentUserContext { UserId = adminUserId, Role = UserRole.Admin });
    }

    public async Task<ContestDetailDto> UpdateAsync(Guid contestId, ContestUpdateRequestDto request, CancellationToken cancellationToken)
    {
        ValidateContestWindow(request.StartTimeUtc, request.EndTimeUtc);

        var contest = await _contestRepository.GetByIdAsync(contestId, cancellationToken)
            ?? throw new NotFoundException("Contest not found.");

        contest.Name = request.Name.Trim();
        contest.Description = request.Description.Trim();
        contest.StartTimeUtc = DateTime.SpecifyKind(request.StartTimeUtc, DateTimeKind.Utc);
        contest.EndTimeUtc = DateTime.SpecifyKind(request.EndTimeUtc, DateTimeKind.Utc);
        contest.ContestType = request.ContestType;
        contest.Prize = request.Prize.Trim();

        await _contestRepository.SaveChangesAsync(cancellationToken);

        var updated = await _contestRepository.GetWithQuestionsAsync(contestId, cancellationToken) ?? contest;
        return MapContestDetail(updated, new CurrentUserContext { Role = UserRole.Admin });
    }

    public async Task DeleteAsync(Guid contestId, CancellationToken cancellationToken)
    {
        var contest = await _contestRepository.GetByIdAsync(contestId, cancellationToken)
            ?? throw new NotFoundException("Contest not found.");

        _contestRepository.Delete(contest);
        await _contestRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<ContestDetailDto> GetByIdAsync(Guid contestId, CurrentUserContext currentUser, CancellationToken cancellationToken)
    {
        var contest = await _contestRepository.GetWithQuestionsAsync(contestId, cancellationToken)
            ?? throw new NotFoundException("Contest not found.");

        return MapContestDetail(contest, currentUser);
    }

    public async Task<PagedResult<ContestSummaryDto>> GetPagedAsync(CurrentUserContext currentUser, PaginationRequest pagination, CancellationToken cancellationToken)
    {
        var contests = await _contestRepository.GetPagedAsync(pagination.PageNumber, pagination.PageSize, cancellationToken);
        var totalCount = await _contestRepository.CountAsync(cancellationToken);

        return new PagedResult<ContestSummaryDto>
        {
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalCount = totalCount,
            Items = contests.Select(contest => MapContestSummary(contest, currentUser)).ToArray()
        };
    }

    private static void ValidateContestWindow(DateTime startTimeUtc, DateTime endTimeUtc)
    {
        if (DateTime.SpecifyKind(startTimeUtc, DateTimeKind.Utc) >= DateTime.SpecifyKind(endTimeUtc, DateTimeKind.Utc))
        {
            throw new BadRequestException("Contest end time must be later than start time.");
        }
    }

    private static ContestSummaryDto MapContestSummary(Contest contest, CurrentUserContext currentUser)
    {
        var now = DateTime.UtcNow;
        var status = now < contest.StartTimeUtc
            ? "Upcoming"
            : now > contest.EndTimeUtc
                ? "Expired"
                : "Active";

        return new ContestSummaryDto
        {
            Id = contest.Id,
            Name = contest.Name,
            Description = contest.Description,
            StartTimeUtc = contest.StartTimeUtc,
            EndTimeUtc = contest.EndTimeUtc,
            ContestType = contest.ContestType.ToString(),
            Prize = contest.Prize,
            Status = status,
            CanParticipate = CanParticipate(contest.ContestType, currentUser.Role)
        };
    }

    private static ContestDetailDto MapContestDetail(Contest contest, CurrentUserContext currentUser)
    {
        var summary = MapContestSummary(contest, currentUser);
        return new ContestDetailDto
        {
            Id = summary.Id,
            Name = summary.Name,
            Description = summary.Description,
            StartTimeUtc = summary.StartTimeUtc,
            EndTimeUtc = summary.EndTimeUtc,
            ContestType = summary.ContestType,
            Prize = summary.Prize,
            Status = summary.Status,
            CanParticipate = summary.CanParticipate,
            QuestionCount = contest.Questions.Count
        };
    }

    internal static bool CanParticipate(ContestType contestType, UserRole? userRole)
    {
        return contestType switch
        {
            ContestType.Vip => userRole is UserRole.Admin or UserRole.VipUser,
            ContestType.Normal => userRole is UserRole.Admin or UserRole.VipUser or UserRole.NormalUser,
            _ => false
        };
    }
}
