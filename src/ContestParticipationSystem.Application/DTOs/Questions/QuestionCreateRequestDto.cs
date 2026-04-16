using System.ComponentModel.DataAnnotations;
using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Application.DTOs.Questions;

public class QuestionCreateRequestDto
{
    [Required]
    [StringLength(1000, MinimumLength = 3)]
    public string Text { get; set; } = string.Empty;

    [Required]
    public QuestionType QuestionType { get; set; }

    [Range(1, int.MaxValue)]
    public int Order { get; set; }

    [MinLength(2)]
    public List<QuestionOptionCreateRequestDto> Options { get; set; } = new();
}
