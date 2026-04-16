using System.ComponentModel.DataAnnotations;

namespace ContestParticipationSystem.Application.DTOs.Questions;

public class QuestionOptionCreateRequestDto
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    [Range(1, int.MaxValue)]
    public int SortOrder { get; set; }
}
