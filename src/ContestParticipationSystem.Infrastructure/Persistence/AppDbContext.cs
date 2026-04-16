using ContestParticipationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContestParticipationSystem.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Contest> Contests => Set<Contest>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<Participation> Participations => Set<Participation>();
    public DbSet<ParticipationAnswer> ParticipationAnswers => Set<ParticipationAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);
            entity.Property(user => user.FullName).HasMaxLength(100).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(256).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(1000).IsRequired();
            entity.HasIndex(user => user.Email).IsUnique();
        });

        modelBuilder.Entity<Contest>(entity =>
        {
            entity.HasKey(contest => contest.Id);
            entity.Property(contest => contest.Name).HasMaxLength(150).IsRequired();
            entity.Property(contest => contest.Description).HasMaxLength(2000).IsRequired();
            entity.Property(contest => contest.Prize).HasMaxLength(250).IsRequired();
            entity.HasIndex(contest => contest.StartTimeUtc);
            entity.HasIndex(contest => contest.EndTimeUtc);
            entity.HasOne(contest => contest.CreatedByUser)
                .WithMany(user => user.CreatedContests)
                .HasForeignKey(contest => contest.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(question => question.Id);
            entity.Property(question => question.Text).HasMaxLength(1000).IsRequired();
            entity.HasIndex(question => new { question.ContestId, question.Order }).IsUnique();
            entity.HasOne(question => question.Contest)
                .WithMany(contest => contest.Questions)
                .HasForeignKey(question => question.ContestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(option => option.Id);
            entity.Property(option => option.Text).HasMaxLength(500).IsRequired();
            entity.HasIndex(option => new { option.QuestionId, option.SortOrder }).IsUnique();
            entity.HasOne(option => option.Question)
                .WithMany(question => question.Options)
                .HasForeignKey(option => option.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Participation>(entity =>
        {
            entity.HasKey(participation => participation.Id);
            entity.HasIndex(participation => new { participation.UserId, participation.ContestId }).IsUnique();
            entity.HasIndex(participation => participation.UserId);
            entity.HasIndex(participation => participation.ContestId);
            entity.HasOne(participation => participation.User)
                .WithMany(user => user.Participations)
                .HasForeignKey(participation => participation.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(participation => participation.Contest)
                .WithMany(contest => contest.Participations)
                .HasForeignKey(participation => participation.ContestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ParticipationAnswer>(entity =>
        {
            entity.HasKey(answer => answer.Id);
            entity.Property(answer => answer.AnswerOptionIdsJson).HasColumnType("jsonb");
            entity.HasIndex(answer => new { answer.ParticipationId, answer.QuestionId }).IsUnique();
            entity.HasOne(answer => answer.Participation)
                .WithMany(participation => participation.Answers)
                .HasForeignKey(answer => answer.ParticipationId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(answer => answer.Question)
                .WithMany(question => question.ParticipationAnswers)
                .HasForeignKey(answer => answer.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
