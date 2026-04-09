using Microsoft.EntityFrameworkCore;
using StackOverFIT.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace StackOverFIT.DAL.Tests;

public class DbContextLibraryTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_Library_Persisted()
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = "MajitelKnihovny",
            Email = "majitel@test.cz",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 1
        };

        var library = new LibraryEntity
        {
            Id = Guid.NewGuid(),
            Name = "RPG Classics",
            Description = "Sbirka RPG klasik",
            NumOfGames = 0
        };

        await using var db_setup = await DbContextFactory.CreateDbContextAsync();
        db_setup.Users.Add(user);
        db_setup.Libraries.Add(library);
        await db_setup.SaveChangesAsync();

        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        var actual_entity = await db_assert.Libraries.SingleAsync(i => i.Id == library.Id);

        Assert.Equal(
            library with 
            { 
                VideoGames = Array.Empty<VideoGameEntity>() 
            },
            actual_entity with 
            { 
                VideoGames = Array.Empty<VideoGameEntity>() 
            }
        );
    }

    [Fact]
    public async Task Update_Library_Persisted()
    {
        var library_id = Guid.NewGuid();
        var base_library = new LibraryEntity
        {
            Id = library_id,
            Name = "Puvodni nazev",
            Description = "Puvodni popis",
            NumOfGames = 5
        };

        await using var db_setup = await DbContextFactory.CreateDbContextAsync();
        db_setup.Libraries.Add(base_library);
        await db_setup.SaveChangesAsync();

        var entity_to_update = base_library with
        {
            Name = "Aktualizovany nazev",
            Description = "Aktualizovany popis"
        };

        StackOverFITDbContextSUT.Libraries.Update(entity_to_update);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        var actual_entity = await db_assert.Libraries.SingleAsync(i => i.Id == library_id);

        Assert.Equal(
            entity_to_update with 
            { 
                VideoGames = Array.Empty<VideoGameEntity>() 
            },
            actual_entity with 
            { 
                VideoGames = Array.Empty<VideoGameEntity>() 
            }
        );
    }

    [Fact]
    public async Task Delete_Library_Deleted()
    {
        var library = new LibraryEntity
        {
            Id = Guid.NewGuid(),
            Name = "Knihovna ke smazani",
            NumOfGames = 5
        };

        await using var db_setup = await DbContextFactory.CreateDbContextAsync();
        db_setup.Libraries.Add(library);
        await db_setup.SaveChangesAsync();

        StackOverFITDbContextSUT.Libraries.Remove(library);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        Assert.False(await db_assert.Libraries.AnyAsync(i => i.Id == library.Id));
    }
}
