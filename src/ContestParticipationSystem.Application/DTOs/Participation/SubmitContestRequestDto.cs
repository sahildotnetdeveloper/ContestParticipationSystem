using System.ComponentModel.DataAnnotations;

namespace ContestParticipationSystem.Application.DTOs.Participation;

public class SubmitContestRequestDto
{
    [Required]
    [MinLength(1)]
    public List<QuestionAnswerSubmissionDto> Answers { get; set; } = new();
}
