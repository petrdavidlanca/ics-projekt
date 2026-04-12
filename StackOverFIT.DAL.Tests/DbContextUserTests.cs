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
        UserEntity entity = new()
        {
            Id = Guid.Parse("D4E5F6A1-B2C3-4D5E-1F2A-3B4C5D6E7F8A"),
            Username = "NovyHrac",
            Email = "novy@test.cz",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 0
        };

        StackOverFITDbContextSUT.Users.Add(entity);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await using var db_assert = await DbContextFactory.CreateDbContextAsync();
        var actual_entities = await db_assert.Users.SingleAsync(i => i.Id == entity.Id);

        Assert.Equal(
            entity with 
            { 
                GameLibrary = Array.Empty<LibraryEntity>() 
            },
            actual_entities with 
            { 
                GameLibrary = Array.Empty<LibraryEntity>() 
            }
        );
    }

    [Fact]
    public async Task GetById_User_UserRetrieved()
    {
        UserEntity entity = new()
        {
            Id = Guid.Parse("A1B2C3D4-E5F6-4A1B-8C9D-0E1F2A3B4C5D"),
            Username = "HracProCteni",
            Email = "cteni@test.cz",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 0
        };
        StackOverFITDbContextSUT.Users.Add(entity);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        var retrieved_entity = await StackOverFITDbContextSUT.Users.SingleAsync(i => i.Id == entity.Id);

        Assert.Equal(entity, retrieved_entity);
    }

    [Fact]
    public async Task Update_User_Persisted()
    {
        UserEntity base_entity = new()
        {
            Id = Guid.Parse("B2C3D4E5-F6A1-4B2C-9D0E-1F2A3B4C5D6E"),
            Username = "PuvodniJmeno",
            Email = "puvodni@test.cz",
            TotalNumOfGames = 5,
            TotalNumOfLibraries = 5
        };

        await using var db_setup = await DbContextFactory.CreateDbContextAsync(); 
        db_setup.Users.Add(base_entity);
        await db_setup.SaveChangesAsync();

        var entity_to_update = base_entity with
        {
            Username = "AktualizovaneJmeno",
            Email = "aktualizovany@test.cz"
        };

        StackOverFITDbContextSUT.Users.Update(entity_to_update);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actual_entity = await dbx.Users.SingleAsync(i => i.Id == entity_to_update.Id);

        Assert.Equal(
            entity_to_update with
            {
                GameLibrary = Array.Empty<LibraryEntity>()
            },
            actual_entity with
            {
                GameLibrary = Array.Empty<LibraryEntity>()
            }
        );
    }

    [Fact]
    public async Task Delete_User_UserDeleted()
    {
        UserEntity entity_to_delete = new()
        {
            Id = Guid.Parse("C3D4E5F6-A1B2-4C3D-0E1F-2A3B4C5D6E7F"),
            Username = "SmazMe",
            Email = "smazat@test.cz",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 0
        };
        StackOverFITDbContextSUT.Users.Add(entity_to_delete);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        StackOverFITDbContextSUT.Users.Remove(entity_to_delete);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        Assert.False(await StackOverFITDbContextSUT.Users.AnyAsync(i => i.Id == entity_to_delete.Id));
    }
}
