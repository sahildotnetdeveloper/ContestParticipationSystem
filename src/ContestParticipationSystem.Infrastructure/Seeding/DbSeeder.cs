using ContestParticipationSystem.Domain.Entities;
using ContestParticipationSystem.Domain.Enums;
using ContestParticipationSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContestParticipationSystem.Infrastructure.Seeding;

public class DbSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public DbSeeder(AppDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var admin = BuildUser("System Admin", "admin@contest.local", "Admin@123", UserRole.Admin);
        var vip = BuildUser("VIP Demo", "vip@contest.local", "VipUser@123", UserRole.VipUser);
        var normal = BuildUser("Normal Demo", "user@contest.local", "NormalUser@123", UserRole.NormalUser);

        await _dbContext.Users.AddRangeAsync(new[] { admin, vip, normal }, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private User BuildUser(string fullName, string email, string password, UserRole role)
    {
        var user = new User
        {
            FullName = fullName,
            Email = email,
            Role = role
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        return user;
    }
}
