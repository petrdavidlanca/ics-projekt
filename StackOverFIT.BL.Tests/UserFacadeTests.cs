using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Mappers;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using Xunit;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StackOverFIT.BL.Tests;

public class UserFacadeTests : FacadeTestsBase
{
    private readonly UserFacade _userFacade;

    public UserFacadeTests()
        : base(null!)
    {
        _userFacade = new UserFacade(UnitOfWorkFactory, new UserModelMapper());
    }

    [Fact]
    public async Task SaveAsync_Creates_New_User()
    {
        var model = new UserDetailModel
        {
            Id = Guid.Empty,
            Username = "John Doe",
            Email = "john@example.com",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 0
        };

        var saved = await _userFacade.SaveAsync(model);

        Assert.NotEqual(Guid.Empty, saved.Id);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var entity = await verifyCtx.Users.FindAsync(saved.Id);

        Assert.NotNull(entity);
        Assert.Equal("John Doe", entity!.Username);
        Assert.Equal("john@example.com", entity.Email);
    }

    [Fact]
    public async Task SaveAsync_Updates_Existing_User()
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = "Original",
            Email = "original@example.com",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 0
        };

        StackOverFITDbContextSUT.Users.Add(user);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        var model = new UserDetailModel
        {
            Id = user.Id,
            Username = "Updated",
            Email = "updated@example.com",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 0
        };

        var updated = await _userFacade.SaveAsync(model);

        Assert.Equal("Updated", updated.Username);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var entity = await verifyCtx.Users.FindAsync(user.Id);

        Assert.Equal("Updated", entity!.Username);
        Assert.Equal("updated@example.com", entity.Email);
    }

    [Fact]
    public async Task GetAsync_Returns_UserDetail()
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = "Alice",
            Email = "alice@example.com",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 0
        };

        StackOverFITDbContextSUT.Users.Add(user);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        var loaded = await _userFacade.GetAsync(user.Id);

        Assert.NotNull(loaded);
        Assert.Equal("Alice", loaded!.Username);
        Assert.Equal("alice@example.com", loaded.Email);
    }

    [Fact]
    public async Task GetAsync_ReturnsNull_When_NotFound()
    {
        var result = await _userFacade.GetAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_Returns_All_Users()
    {
        StackOverFITDbContextSUT.Users.Add(new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = "User1",
            Email = "u1@example.com",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 0
        });

        StackOverFITDbContextSUT.Users.Add(new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = "User2",
            Email = "u2@example.com",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 0
        });

        await StackOverFITDbContextSUT.SaveChangesAsync();

        var all = await _userFacade.GetAsync();

        Assert.True(all.Count() >= 2);
    }

    [Fact]
    public async Task DeleteAsync_Removes_User()
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = "ToDelete",
            Email = "delete@example.com",
            TotalNumOfGames = 0,
            TotalNumOfLibraries = 0
        };

        StackOverFITDbContextSUT.Users.Add(user);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await _userFacade.DeleteAsync(user.Id);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var entity = await verifyCtx.Users.FindAsync(user.Id);

        Assert.Null(entity);
    }

    [Fact]
    public async Task DeleteAsync_DoesNotThrow_When_NotFound()
    {
        await _userFacade.DeleteAsync(Guid.NewGuid());
    }

}
