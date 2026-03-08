using Microsoft.EntityFrameworkCore;
using StackOverFIT.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace StackOverFIT.DAL.Tests;

public class DbContextUserTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_User_Persisted()
    {
        // Arrange
        user_entity entity = new()
        {
            Id = Guid.Parse("D4E5F6A1-B2C3-4D5E-1F2A-3B4C5D6E7F8A"),
            username = "NovyHrac",
            email = "novy@test.cz",
            total_num_of_games = 0,
            total_num_of_libraries = 0
        };

        // Act
        StackOverFITDbContextSUT.users.Add(entity); //zkontrolovani nazvu DbSetu "users"
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Assert
        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        var actual_entities = await db_assert.users.SingleAsync(i => i.Id == entity.Id);

        //bypassnuti porovnavani kolekci (coz zpusobovalo chybu testu, i kdyz hodnoty se rovnaly) tim, ze obe budou mit prazdnou referenci, protoze v testu se nezajimame o obsah kolekce
        Assert.Equal(
            entity with 
            { 
                game_library = Array.Empty<library_entity>() 
            },
            actual_entities with 
            { 
                game_library = Array.Empty<library_entity>() 
            }
        );
    }

    [Fact]
    public async Task GetById_User_UserRetrieved()
    {
        // Arrange
        //musime uzivatele do testovaci DB nejprve dostat
        user_entity entity = new()
        {
            Id = Guid.Parse("A1B2C3D4-E5F6-4A1B-8C9D-0E1F2A3B4C5D"),
            username = "HracProCteni",
            email = "cteni@test.cz",
            total_num_of_games = 0,
            total_num_of_libraries = 0
        };
        StackOverFITDbContextSUT.users.Add(entity);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Act
        var retrieved_entity = await StackOverFITDbContextSUT.users.SingleAsync(i => i.Id == entity.Id);

        // Assert
        Assert.Equal(entity, retrieved_entity);
    }

    [Fact]
    public async Task Update_User_Persisted()
    {
        // Arrange
        user_entity base_entity = new()
        {
            Id = Guid.Parse("B2C3D4E5-F6A1-4B2C-9D0E-1F2A3B4C5D6E"),
            username = "PuvodniJmeno",
            email = "puvodni@test.cz",
            total_num_of_games = 5,
            total_num_of_libraries = 5
        };
        //vytvorime novy DbContext pro pripravu dat, abychom nemeli v testu dva stejny DbContexty, coz by mohlo zpusobit problemy s trackingem entit
        await using var db_setup = await DbContextFactory.CreateDbContextAsync(); 
        db_setup.users.Add(base_entity);
        await db_setup.SaveChangesAsync();

        // Act
        //zkopirujeme record s novymi daty a updatneme
        var entity_to_update = base_entity with
        {
            username = "AktualizovaneJmeno",
            email = "aktualizovany@test.cz"
        };

        StackOverFITDbContextSUT.users.Update(entity_to_update);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actual_entity = await dbx.users.SingleAsync(i => i.Id == entity_to_update.Id);
        //znovu to co u pridavani usera
        Assert.Equal(
            entity_to_update with
            {
                game_library = Array.Empty<library_entity>()
            },
            actual_entity with
            {
                game_library = Array.Empty<library_entity>()
            }
        );
    }

    [Fact]
    public async Task Delete_User_UserDeleted()
    {
        // Arrange
        user_entity entity_to_delete = new()
        {
            Id = Guid.Parse("C3D4E5F6-A1B2-4C3D-0E1F-2A3B4C5D6E7F"),
            username = "SmazMe",
            email = "smazat@test.cz",
            total_num_of_games = 0,
            total_num_of_libraries = 0
        };
        StackOverFITDbContextSUT.users.Add(entity_to_delete);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Act
        StackOverFITDbContextSUT.users.Remove(entity_to_delete);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Assert
        Assert.False(await StackOverFITDbContextSUT.users.AnyAsync(i => i.Id == entity_to_delete.Id));
    }
}
