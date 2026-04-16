using System.Text.Json;
using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Participation;
using ContestParticipationSystem.Application.Exceptions;
using ContestParticipationSystem.Application.Interfaces.Repositories;
using ContestParticipationSystem.Application.Interfaces.Services;
using ContestParticipationSystem.Domain.Entities;
using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Application.Services;

public class ParticipationService : IParticipationService
{
    private const int CorrectAnswerPoints = 10;

    private readonly IContestRepository _contestRepository;
    private readonly IParticipationRepository _participationRepository;

    public ParticipationService(IContestRepository contestRepository, IParticipationRepository participationRepository)
    {
        _contestRepository = contestRepository;
        _participationRepository = participationRepository;
    }

    public async Task<StartContestResponseDto> StartContestAsync(Guid userId, CurrentUserContext currentUser, Guid contestId, CancellationToken cancellationToken)
    {
        var contest = await _contestRepository.GetForParticipationAsync(contestId, cancellationToken)
            ?? throw new NotFoundException("Contest not found.");

        ValidateContestAccess(contest, currentUser);

        var now = DateTime.UtcNow;
        if (now < contest.StartTimeUtc)
        {
            throw new BadRequestException("Contest has not started yet.");
        }

        if (now > contest.EndTimeUtc)
        {
            throw new BadRequestException("Contest has already expired.");
        }

        if (!contest.Questions.Any())
        {
            throw new BadRequestException("Contest cannot be started because it has no questions.");
        }

        Participation? participation = null;

        await _participationRepository.ExecuteInTransactionAsync(async innerToken =>
        {
            participation = await _participationRepository.GetByUserAndContestAsync(userId, contestId, innerToken);
            if (participation is not null && participation.Status == ParticipationStatus.Submitted)
            {
                throw new ConflictException("Contest has already been submitted.");
            }

            if (participation is null)
            {
                participation = new Participation
                {
                    UserId = userId,
                    ContestId = contestId,
                    StartedAtUtc = now,
                    Status = ParticipationStatus.Started
                };

                await _participationRepository.AddAsync(participation, innerToken);
                await _participationRepository.SaveChangesAsync(innerToken);
            }
        }, cancellationToken);

        return new StartContestResponseDto
        {
            ParticipationId = participation!.Id,
            ContestId = contest.Id,
            StartedAtUtc = participation.StartedAtUtc,
            EndsAtUtc = contest.EndTimeUtc,
            Questions = contest.Questions
                .OrderBy(question => question.Order)
                .Select(QuestionService.MapPublicQuestion)
                .ToArray()
        };
    }

    public async Task<SubmitContestResponseDto> SubmitAsync(Guid userId, CurrentUserContext currentUser, Guid contestId, SubmitContestRequestDto request, CancellationToken cancellationToken)
    {
        if (request.Answers.Count == 0)
        {
            throw new BadRequestException("Submission cannot be empty.");
        }

        var contest = await _contestRepository.GetForParticipationAsync(contestId, cancellationToken)
            ?? throw new NotFoundException("Contest not found.");

        ValidateContestAccess(contest, currentUser);

        var now = DateTime.UtcNow;
        if (now < contest.StartTimeUtc)
        {
            throw new BadRequestException("Contest has not started yet.");
        }

        if (now > contest.EndTimeUtc)
        {
            throw new BadRequestException("Contest has already expired.");
        }

        var duplicatedQuestionIds = request.Answers
            .GroupBy(answer => answer.QuestionId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        if (duplicatedQuestionIds.Length > 0)
        {
            throw new BadRequestException("Each question can be answered only once per submission.");
        }

        var questionMap = contest.Questions.ToDictionary(question => question.Id);
        var answerEntities = new List<ParticipationAnswer>();
        var totalScore = 0;

        foreach (var answer in request.Answers)
        {
            if (!questionMap.TryGetValue(answer.QuestionId, out var question))
            {
                throw new BadRequestException($"Question {answer.QuestionId} is invalid for this contest.");
            }

            var normalizedSelectedOptionIds = answer.SelectedOptionIds
                .Distinct()
                .OrderBy(optionId => optionId)
                .ToArray();

            if (normalizedSelectedOptionIds.Length == 0)
            {
                throw new BadRequestException($"Question {answer.QuestionId} must contain at least one selected option.");
            }

            var validOptionIds = question.Options.Select(option => option.Id).ToHashSet();
            if (normalizedSelectedOptionIds.Any(optionId => !validOptionIds.Contains(optionId)))
            {
                throw new BadRequestException($"Question {answer.QuestionId} contains invalid option identifiers.");
            }

            if (question.QuestionType is QuestionType.SingleSelect or QuestionType.TrueFalse && normalizedSelectedOptionIds.Length != 1)
            {
                throw new BadRequestException($"Question {answer.QuestionId} must contain exactly one selected option.");
            }

            var correctOptionIds = question.Options
                .Where(option => option.IsCorrect)
                .Select(option => option.Id)
                .OrderBy(optionId => optionId)
                .ToArray();

            var isCorrect = normalizedSelectedOptionIds.SequenceEqual(correctOptionIds);
            var awardedPoints = isCorrect ? CorrectAnswerPoints : 0;
            totalScore += awardedPoints;

            answerEntities.Add(new ParticipationAnswer
            {
                QuestionId = question.Id,
                AnswerOptionIdsJson = JsonSerializer.Serialize(normalizedSelectedOptionIds),
                IsCorrect = isCorrect,
                AwardedPoints = awardedPoints,
                SubmittedAtUtc = now
            });
        }

        Participation? participation = null;

        await _participationRepository.ExecuteInTransactionAsync(async innerToken =>
        {
            participation = await _participationRepository.GetForSubmissionAsync(userId, contestId, innerToken)
                ?? throw new BadRequestException("Contest must be started before submitting.");

            if (participation.SubmittedAtUtc.HasValue || participation.Status == ParticipationStatus.Submitted)
            {
                throw new ConflictException("Contest has already been submitted.");
            }

            participation.Answers.Clear();
            foreach (var answerEntity in answerEntities)
            {
                participation.Answers.Add(answerEntity);
            }

            participation.Score = totalScore;
            participation.SubmittedAtUtc = now;
            participation.Status = ParticipationStatus.Submitted;

            await _participationRepository.SaveChangesAsync(innerToken);
        }, cancellationToken);

        return new SubmitContestResponseDto
        {
            ParticipationId = participation!.Id,
            ContestId = contestId,
            Score = participation.Score,
            SubmittedAtUtc = participation.SubmittedAtUtc!.Value
        };
    }

    private static void ValidateContestAccess(Contest contest, CurrentUserContext currentUser)
    {
        if (!currentUser.IsAuthenticated || !ContestService.CanParticipate(contest.ContestType, currentUser.Role))
        {
            throw new ForbiddenException("You are not allowed to participate in this contest.");
        }
    }
}
