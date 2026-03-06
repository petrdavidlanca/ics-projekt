using StackOverFIT.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace StackOverFIT.DAL;

public class StackOverFITDbContext(DbContextOptions contextOptions) : DbContext(contextOptions)
{
    public DbSet<library_entity> libraries => Set<library_entity>();
    public DbSet<user_entity> users => Set<user_entity>();
    public DbSet<video_game_entity> video_games => Set<video_game_entity>();

    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RecipeEntity>()
            .HasMany(i => i.Ingredients)
            .WithOne(i => i.Recipe)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IngredientEntity>()
            .HasMany<IngredientAmountEntity>()
            .WithOne(i => i.Ingredient)
            .OnDelete(DeleteBehavior.Restrict);
    }*/
}