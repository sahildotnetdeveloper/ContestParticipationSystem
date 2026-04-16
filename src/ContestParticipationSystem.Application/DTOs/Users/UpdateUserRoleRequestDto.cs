using System.ComponentModel.DataAnnotations;
using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Application.DTOs.Users;

public class UpdateUserRoleRequestDto
{
    [Required]
    public UserRole Role { get; set; }
}
