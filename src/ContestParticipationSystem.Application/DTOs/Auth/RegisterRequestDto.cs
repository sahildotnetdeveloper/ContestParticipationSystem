using System.ComponentModel.DataAnnotations;

namespace ContestParticipationSystem.Application.DTOs.Auth;

public class RegisterRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(64, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
