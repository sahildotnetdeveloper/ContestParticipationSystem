using System.ComponentModel.DataAnnotations;

namespace ContestParticipationSystem.Application.DTOs.Participation;

public class QuestionAnswerSubmissionDto
{
    [Required]
    public Guid QuestionId { get; set; }

    [Required]
    [MinLength(1)]
    public List<Guid> SelectedOptionIds { get; set; } = new();
}
