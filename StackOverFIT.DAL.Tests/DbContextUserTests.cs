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
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntities = await dbx.users.SingleAsync(i => i.Id == entity.Id);

        //bypassnuti porovnavani kolekci (coz zpusobovalo chybu testu, i kdyz hodnoty se rovnaly) tim, ze obema podstrcime stejnou prazdnou referenci
        Assert.Equal(
            entity with 
            { 
                game_library = Array.Empty<library_entity>() 
            },
            actualEntities with 
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
        var retrievedEntity = await StackOverFITDbContextSUT.users.SingleAsync(i => i.Id == entity.Id);

        // Assert
        Assert.Equal(entity, retrievedEntity);
    }

    [Fact]
    public async Task Update_User_Persisted()
    {
        // Arrange
        user_entity baseEntity = new()
        {
            Id = Guid.Parse("B2C3D4E5-F6A1-4B2C-9D0E-1F2A3B4C5D6E"),
            username = "PuvodniJmeno",
            email = "puvodni@test.cz",
            total_num_of_games = 0,
            total_num_of_libraries = 0
        };
        //izolovanej dbxSetup pro vlozeni puvodniho recordu, ktery chceme updatovat, aby se to nepletlo s hlavnim dbxSUT a nezpusobovalo chyby testu kvuli sledovani stejnych entit
        await using var dbxSetup = await DbContextFactory.CreateDbContextAsync();
        dbxSetup.users.Add(baseEntity);
        await dbxSetup.SaveChangesAsync();

        // Act
        //zkopirujeme record s novymi daty a updatneme
        var entityToUpdate = baseEntity with
        {
            username = "AktualizovaneJmeno",
            email = "aktualizovany@test.cz"
        };

        StackOverFITDbContextSUT.users.Update(entityToUpdate);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.users.SingleAsync(i => i.Id == entityToUpdate.Id);
        //znovu to co u pridavani usera
        Assert.Equal(
            entityToUpdate with
            {
                game_library = Array.Empty<library_entity>()
            },
            actualEntity with
            {
                game_library = Array.Empty<library_entity>()
            }
        );
    }

    [Fact]
    public async Task Delete_User_UserDeleted()
    {
        // Arrange
        user_entity entityToDelete = new()
        {
            Id = Guid.Parse("C3D4E5F6-A1B2-4C3D-0E1F-2A3B4C5D6E7F"),
            username = "SmazMne",
            email = "smazat@test.cz",
            total_num_of_games = 0,
            total_num_of_libraries = 0
        };
        StackOverFITDbContextSUT.users.Add(entityToDelete);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Act
        StackOverFITDbContextSUT.users.Remove(entityToDelete);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        // Assert
        Assert.False(await StackOverFITDbContextSUT.users.AnyAsync(i => i.Id == entityToDelete.Id));
    }
}
