using StackOverFIT.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace StackOverFIT.DAL;

public class StackOverFITDbContext(DbContextOptions contextOptions) : DbContext(contextOptions)
{
    public DbSet<LibraryEntity> Libraries => Set<LibraryEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<VideoGameEntity> VideoGames => Set<VideoGameEntity>();
}