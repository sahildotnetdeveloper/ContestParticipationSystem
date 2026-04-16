using ContestParticipationSystem.Application.DTOs.Questions;
using ContestParticipationSystem.Application.Exceptions;
using ContestParticipationSystem.Application.Interfaces.Repositories;
using ContestParticipationSystem.Application.Interfaces.Services;
using ContestParticipationSystem.Domain.Entities;
using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IContestRepository _contestRepository;
    private readonly IQuestionRepository _questionRepository;

    public QuestionService(IContestRepository contestRepository, IQuestionRepository questionRepository)
    {
        _contestRepository = contestRepository;
        _questionRepository = questionRepository;
    }

    public async Task<QuestionAdminDto> CreateAsync(Guid contestId, QuestionCreateRequestDto request, CancellationToken cancellationToken)
    {
        _ = await _contestRepository.GetByIdAsync(contestId, cancellationToken)
            ?? throw new NotFoundException("Contest not found.");

        ValidateQuestion(request);

        var question = new Question
        {
            ContestId = contestId,
            Text = request.Text.Trim(),
            QuestionType = request.QuestionType,
            Order = request.Order,
            Options = request.Options
                .OrderBy(option => option.SortOrder)
                .Select(option => new QuestionOption
                {
                    Text = option.Text.Trim(),
                    IsCorrect = option.IsCorrect,
                    SortOrder = option.SortOrder
                })
                .ToList()
        };

        await _questionRepository.AddAsync(question, cancellationToken);
        await _questionRepository.SaveChangesAsync(cancellationToken);

        var stored = await _questionRepository.GetByIdWithOptionsAsync(question.Id, cancellationToken) ?? question;
        return MapAdminQuestion(stored);
    }

    public async Task<QuestionAdminDto> UpdateAsync(Guid questionId, QuestionCreateRequestDto request, CancellationToken cancellationToken)
    {
        ValidateQuestion(request);

        var question = await _questionRepository.GetByIdWithOptionsAsync(questionId, cancellationToken)
            ?? throw new NotFoundException("Question not found.");

        question.Text = request.Text.Trim();
        question.QuestionType = request.QuestionType;
        question.Order = request.Order;
        question.Options.Clear();

        foreach (var option in request.Options.OrderBy(option => option.SortOrder))
        {
            question.Options.Add(new QuestionOption
            {
                Text = option.Text.Trim(),
                IsCorrect = option.IsCorrect,
                SortOrder = option.SortOrder
            });
        }

        await _questionRepository.SaveChangesAsync(cancellationToken);

        var stored = await _questionRepository.GetByIdWithOptionsAsync(question.Id, cancellationToken) ?? question;
        return MapAdminQuestion(stored);
    }

    public async Task DeleteAsync(Guid questionId, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetByIdAsync(questionId, cancellationToken)
            ?? throw new NotFoundException("Question not found.");

        _questionRepository.Delete(question);
        await _questionRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<QuestionAdminDto>> GetByContestAsync(Guid contestId, CancellationToken cancellationToken)
    {
        _ = await _contestRepository.GetByIdAsync(contestId, cancellationToken)
            ?? throw new NotFoundException("Contest not found.");

        var questions = await _questionRepository.GetByContestIdAsync(contestId, cancellationToken);
        return questions.OrderBy(question => question.Order).Select(MapAdminQuestion).ToArray();
    }

    private static void ValidateQuestion(QuestionCreateRequestDto request)
    {
        if (request.Options.Select(option => option.SortOrder).Distinct().Count() != request.Options.Count)
        {
            throw new BadRequestException("Option sort order must be unique per question.");
        }

        var correctCount = request.Options.Count(option => option.IsCorrect);
        if (correctCount == 0)
        {
            throw new BadRequestException("At least one correct option is required.");
        }

        if (request.QuestionType is QuestionType.SingleSelect or QuestionType.TrueFalse && correctCount != 1)
        {
            throw new BadRequestException("Single select and true/false questions must have exactly one correct option.");
        }

        if (request.QuestionType == QuestionType.TrueFalse && request.Options.Count != 2)
        {
            throw new BadRequestException("True/false questions must contain exactly two options.");
        }
    }

    internal static QuestionAdminDto MapAdminQuestion(Question question)
    {
        return new QuestionAdminDto
        {
            Id = question.Id,
            ContestId = question.ContestId,
            Text = question.Text,
            QuestionType = question.QuestionType.ToString(),
            Order = question.Order,
            Options = question.Options
                .OrderBy(option => option.SortOrder)
                .Select(option => new QuestionAdminOptionDto
                {
                    Id = option.Id,
                    Text = option.Text,
                    SortOrder = option.SortOrder,
                    IsCorrect = option.IsCorrect
                })
                .ToArray()
        };
    }

    internal static QuestionDto MapPublicQuestion(Question question)
    {
        return new QuestionDto
        {
            Id = question.Id,
            Text = question.Text,
            QuestionType = question.QuestionType.ToString(),
            Order = question.Order,
            Options = question.Options
                .OrderBy(option => option.SortOrder)
                .Select(option => new QuestionOptionDto
                {
                    Id = option.Id,
                    Text = option.Text,
                    SortOrder = option.SortOrder
                })
                .ToArray()
        };
    }
}
