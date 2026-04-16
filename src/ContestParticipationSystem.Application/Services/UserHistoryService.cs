using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Users;
using ContestParticipationSystem.Application.Exceptions;
using ContestParticipationSystem.Application.Interfaces.Repositories;
using ContestParticipationSystem.Application.Interfaces.Services;
using ContestParticipationSystem.Domain.Enums;

namespace ContestParticipationSystem.Application.Services;

public class UserHistoryService : IUserHistoryService
{
    private readonly IUserRepository _userRepository;
    private readonly IParticipationRepository _participationRepository;

    public UserHistoryService(IUserRepository userRepository, IParticipationRepository participationRepository)
    {
        _userRepository = userRepository;
        _participationRepository = participationRepository;
    }

    public async Task<PagedResult<UserHistoryItemDto>> GetHistoryAsync(Guid userId, PaginationRequest pagination, CancellationToken cancellationToken)
    {
        return await _participationRepository.GetUserHistoryAsync(userId, pagination.PageNumber, pagination.PageSize, cancellationToken);
    }

    public async Task UpdateRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken)
    {
        if (role == 0)
        {
            throw new BadRequestException("A valid role is required.");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        user.Role = role;
        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}
