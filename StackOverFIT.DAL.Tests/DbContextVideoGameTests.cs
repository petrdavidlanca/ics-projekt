using Microsoft.EntityFrameworkCore;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace StackOverFIT.DAL.Tests;

public class DbContextVideoGameTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_VideoGame_Persisted()
    {
        // Arrange
        var entity = new video_game_entity
        {
            Id = Guid.NewGuid(),
            name = "GTA 6",
            description = "Pribehovka s otevrenym svetem",
            pegi_rating = 18,
            ImageUrl = "https://example.com/gta6.jpg"
        };

        // Act
        StackOverFITDbContextSUT.video_games.Add(entity);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Assert
        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        var actual_entity = await db_assert.video_games.SingleAsync(i => i.Id == entity.Id);

        Assert.Equal(
            entity with 
            { 
                game_genres = Array.Empty<game_genres>() 
            },
            actual_entity with 
            { 
                game_genres = Array.Empty<game_genres>() 
            }
        );
    }

    [Fact]
    public async Task Update_VideoGame_Persisted()
    {
        // Arrange
        var game_id = Guid.NewGuid();
        var baseGame = new video_game_entity
        {
            Id = game_id,
            name = "Minecraft",
            description = "Puvodni popis",
            pegi_rating = 18
        };

        await using var db_setup = await DbContextFactory.CreateDbContextAsync();
        db_setup.video_games.Add(baseGame);
        await db_setup.SaveChangesAsync();

        // Act
        var entity_to_update = baseGame with
        {
            name = "Minecraft 2",
            description = "Minecraft na steroidech"
        };

        StackOverFITDbContextSUT.video_games.Update(entity_to_update);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Assert
        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        var actual_entity = await db_assert.video_games.SingleAsync(i => i.Id == game_id);

        Assert.Equal(
            entity_to_update with { game_genres = Array.Empty<game_genres>() },
            actual_entity with { game_genres = Array.Empty<game_genres>() }
        );
    }

    [Fact]
    public async Task Delete_VideoGame_Deleted()
    {
        // Arrange
        var game = new video_game_entity
        {
            Id = Guid.NewGuid(),
            name = "Hra ke smazání",
            description = "Brzy zmizím",
            pegi_rating = 3
        };

        await using var db_setup = await DbContextFactory.CreateDbContextAsync();
        db_setup.video_games.Add(game);
        await db_setup.SaveChangesAsync();

        // Act
        StackOverFITDbContextSUT.video_games.Remove(game);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Assert
        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        Assert.False(await db_assert.video_games.AnyAsync(i => i.Id == game.Id));
    }
}