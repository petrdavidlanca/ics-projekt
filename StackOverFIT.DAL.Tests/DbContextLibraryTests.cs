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
        // Arrange
        //vytvorime majitele a knihovnu
        var user = new user_entity
        {
            Id = Guid.NewGuid(),
            username = "MajitelKnihovny",
            email = "majitel@test.cz",
            total_num_of_games = 0,
            total_num_of_libraries = 1
        };

        var library = new library_entity
        {
            Id = Guid.NewGuid(),
            name = "RPG Classics",
            description = "Sbirka RPG klasik",
            num_of_games = 0
        };

        // Act
        //ulozime uzivatele i knihovnu
        await using var db_setup = await DbContextFactory.CreateDbContextAsync();
        db_setup.users.Add(user);
        db_setup.libraries.Add(library);
        await db_setup.SaveChangesAsync();

        // Assert
        //overime ulozeni
        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        var actual_entity = await db_assert.libraries.SingleAsync(i => i.Id == library.Id);

        Assert.Equal(
            library with 
            { 
                video_games = Array.Empty<video_game_entity>() 
            },
            actual_entity with 
            { 
                video_games = Array.Empty<video_game_entity>() 
            }
        );
    }

    [Fact]
    public async Task Update_Library_Persisted()
    {
        // Arrange

        var library_id = Guid.NewGuid();
        var base_library = new library_entity
        {
            Id = library_id,
            name = "Puvodni nazev",
            description = "Puvodni popis",
            num_of_games = 5 //pro testovani aktualizace nastavime neco jineho nez 0, aby bylo videt, ze se opravdu aktualizuje
        };

        await using var db_setup = await DbContextFactory.CreateDbContextAsync();
        db_setup.libraries.Add(base_library);
        await db_setup.SaveChangesAsync();

        var entity_to_update = base_library with
        {
            name = "Aktualizovany nazev",
            description = "Aktualizovany popis"
        };

        StackOverFITDbContextSUT.libraries.Update(entity_to_update);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Assert
        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        var actual_entity = await db_assert.libraries.SingleAsync(i => i.Id == library_id);

        Assert.Equal(
            entity_to_update with 
            { 
                video_games = Array.Empty<video_game_entity>() 
            },
            actual_entity with 
            { 
                video_games = Array.Empty<video_game_entity>() 
            }
        );
    }

    [Fact]
    public async Task Delete_Library_Deleted()
    {
        // Arrange
        var library = new library_entity
        {
            Id = Guid.NewGuid(),
            name = "Knihovna ke smazani",
            num_of_games = 5
        };

        await using var db_setup = await DbContextFactory.CreateDbContextAsync();
        db_setup.libraries.Add(library);
        await db_setup.SaveChangesAsync();

        // Act
        StackOverFITDbContextSUT.libraries.Remove(library);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Assert
        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        Assert.False(await db_assert.libraries.AnyAsync(i => i.Id == library.Id));
    }
}
