using System.Data;
using ContestParticipationSystem.Application.Common;
using ContestParticipationSystem.Application.DTOs.Leaderboard;
using ContestParticipationSystem.Application.DTOs.Users;
using ContestParticipationSystem.Application.Exceptions;
using ContestParticipationSystem.Application.Interfaces.Repositories;
using ContestParticipationSystem.Domain.Entities;
using ContestParticipationSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContestParticipationSystem.Infrastructure.Repositories;

public class ParticipationRepository : IParticipationRepository
{
    private readonly AppDbContext _dbContext;

    public ParticipationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Participation participation, CancellationToken cancellationToken)
    {
        await _dbContext.Participations.AddAsync(participation, cancellationToken);
    }

    public Task<Participation?> GetByUserAndContestAsync(Guid userId, Guid contestId, CancellationToken cancellationToken)
    {
        return _dbContext.Participations
            .Include(participation => participation.Answers)
            .SingleOrDefaultAsync(
                participation => participation.UserId == userId && participation.ContestId == contestId,
                cancellationToken);
    }

    public Task<Participation?> GetForSubmissionAsync(Guid userId, Guid contestId, CancellationToken cancellationToken)
    {
        return _dbContext.Participations
            .Include(participation => participation.Answers)
            .SingleOrDefaultAsync(
                participation => participation.UserId == userId && participation.ContestId == contestId,
                cancellationToken);
    }

    public async Task<PagedResult<LeaderboardEntryDto>> GetLeaderboardAsync(Guid contestId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Participations
            .AsNoTracking()
            .Where(participation => participation.ContestId == contestId && participation.SubmittedAtUtc.HasValue)
            .OrderByDescending(participation => participation.Score)
            .ThenBy(participation => participation.SubmittedAtUtc)
            .ThenBy(participation => participation.User!.FullName);

        var totalCount = await query.CountAsync(cancellationToken);
        var pageEntries = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(participation => new
            {
                participation.UserId,
                UserFullName = participation.User!.FullName,
                participation.Score,
                SubmittedAtUtc = participation.SubmittedAtUtc!.Value
            })
            .ToArrayAsync(cancellationToken);

        var items = pageEntries
            .Select((entry, index) => new LeaderboardEntryDto
            {
                Rank = ((pageNumber - 1) * pageSize) + index + 1,
                UserId = entry.UserId,
                FullName = entry.UserFullName,
                Score = entry.Score,
                SubmittedAtUtc = entry.SubmittedAtUtc
            })
            .ToArray();

        return new PagedResult<LeaderboardEntryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<UserHistoryItemDto>> GetUserHistoryAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Participations
            .AsNoTracking()
            .Where(participation => participation.UserId == userId && participation.SubmittedAtUtc.HasValue)
            .OrderByDescending(participation => participation.SubmittedAtUtc)
            .Select(participation => new
            {
                participation.ContestId,
                ContestName = participation.Contest!.Name,
                participation.Score,
                SubmittedAtUtc = participation.SubmittedAtUtc!.Value,
                Prize = participation.Contest!.Prize,
                ContestEnded = participation.Contest!.EndTimeUtc <= DateTime.UtcNow
            });

        var totalCount = await query.CountAsync(cancellationToken);
        var entries = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        var contestIds = entries.Select(entry => entry.ContestId).Distinct().ToArray();
        var winnerLookup = await _dbContext.Participations
            .AsNoTracking()
            .Where(participation => contestIds.Contains(participation.ContestId) && participation.SubmittedAtUtc.HasValue)
            .GroupBy(participation => participation.ContestId)
            .Select(group => group
                .OrderByDescending(participation => participation.Score)
                .ThenBy(participation => participation.SubmittedAtUtc)
                .Select(participation => new { participation.ContestId, participation.UserId })
                .First())
            .ToDictionaryAsync(item => item.ContestId, item => item.UserId, cancellationToken);

        var items = entries.Select(entry => new UserHistoryItemDto
        {
            ContestId = entry.ContestId,
            ContestName = entry.ContestName,
            Score = entry.Score,
            SubmittedAtUtc = entry.SubmittedAtUtc,
            PrizeWon = entry.ContestEnded && winnerLookup.TryGetValue(entry.ContestId, out var winnerUserId) && winnerUserId == userId
                ? entry.Prize
                : null
        }).ToArray();

        return new PagedResult<UserHistoryItemDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            await action(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("A concurrent update was detected. Please retry the request.");
        }
        catch (DbUpdateException exception) when (exception.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new ConflictException("This participation already exists.");
        }
    }
}
