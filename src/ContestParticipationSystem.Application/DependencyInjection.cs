using ContestParticipationSystem.Application.Interfaces.Services;
using ContestParticipationSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ContestParticipationSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IContestService, ContestService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IParticipationService, ParticipationService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<IUserHistoryService, UserHistoryService>();

        return services;
    }
}
