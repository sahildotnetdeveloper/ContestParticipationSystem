using ContestParticipationSystem.Application.Interfaces.Repositories;
using ContestParticipationSystem.Application.Interfaces.Security;
using ContestParticipationSystem.Domain.Entities;
using ContestParticipationSystem.Infrastructure.Persistence;
using ContestParticipationSystem.Infrastructure.Repositories;
using ContestParticipationSystem.Infrastructure.Security;
using ContestParticipationSystem.Infrastructure.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContestParticipationSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IContestRepository, ContestRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IParticipationRepository, ParticipationRepository>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<DbSeeder>();

        return services;
    }
}
