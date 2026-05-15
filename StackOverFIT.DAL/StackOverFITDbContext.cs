using StackOverFIT.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace StackOverFIT.DAL;

public class StackOverFITDbContext(DbContextOptions contextOptions) : DbContext(contextOptions)
{
    public DbSet<LibraryEntity> Libraries => Set<LibraryEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<VideoGameEntity> VideoGames => Set<VideoGameEntity>();

    public DbSet<LibraryVideoGameEntity> LibraryVideoGames => Set<LibraryVideoGameEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LibraryEntity>()
            .HasMany(l => l.VideoGames)
            .WithMany(g => g.Libraries)
            .UsingEntity<LibraryVideoGameEntity>(
                j => j.HasOne(x => x.VideoGame).WithMany().HasForeignKey(x => x.VideoGameId),
                j => j.HasOne(x => x.Library).WithMany().HasForeignKey(x => x.LibraryId)
            );
        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.GameLibrary)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserEntityId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade
            );
    }
}