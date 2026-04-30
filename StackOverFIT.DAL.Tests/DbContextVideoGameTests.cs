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
        var entity = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "GTA 6",
            Description = "Pribehovka s otevrenym svetem",
            PegiRating = 18,
            ImageUrl = "https://example.com/gta6.jpg"
        };

        StackOverFITDbContextSUT.VideoGames.Add(entity);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        var actual_entity = await db_assert.VideoGames.SingleAsync(i => i.Id == entity.Id);

        Assert.Equal(
            entity with 
            { 
                GameGenres = Array.Empty<GameGenres>(),
                Libraries = Array.Empty<LibraryEntity>()
            },
            actual_entity with 
            { 
                GameGenres = Array.Empty<GameGenres>(),
                Libraries = Array.Empty<LibraryEntity>()
            }
        );
    }

    [Fact]
    public async Task Update_VideoGame_Persisted()
    {
        var game_id = Guid.NewGuid();
        var baseGame = new VideoGameEntity
        {
            Id = game_id,
            Name = "Minecraft",
            Description = "Puvodni popis",
            PegiRating = 18
        };

        await using var db_setup = await DbContextFactory.CreateDbContextAsync();
        db_setup.VideoGames.Add(baseGame);
        await db_setup.SaveChangesAsync();

        var entity_to_update = baseGame with
        {
            Name = "Minecraft 2",
            Description = "Minecraft na steroidech"
        };

        StackOverFITDbContextSUT.VideoGames.Update(entity_to_update);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        var actual_entity = await db_assert.VideoGames.SingleAsync(i => i.Id == game_id);

        Assert.Equal(
            entity_to_update with { GameGenres = Array.Empty<GameGenres>(), Libraries = Array.Empty<LibraryEntity>() },
            actual_entity with { GameGenres = Array.Empty<GameGenres>(), Libraries = Array.Empty<LibraryEntity>() }
        );
    }

    [Fact]
    public async Task Delete_VideoGame_Deleted()
    {
        var game = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Hra ke smazání",
            Description = "Brzy zmizím",
            PegiRating = 3
        };

        await using var db_setup = await DbContextFactory.CreateDbContextAsync();
        db_setup.VideoGames.Add(game);
        await db_setup.SaveChangesAsync();

        StackOverFITDbContextSUT.VideoGames.Remove(game);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        Assert.False(await db_assert.VideoGames.AnyAsync(i => i.Id == game.Id));
    }
}