using System.ComponentModel.DataAnnotations;
using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Application.DTOs.Contests;

public class ContestCreateRequestDto
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime StartTimeUtc { get; set; }

    [Required]
    public DateTime EndTimeUtc { get; set; }

    [Required]
    public ContestType ContestType { get; set; }

    [Required]
    [StringLength(250)]
    public string Prize { get; set; } = string.Empty;
}
