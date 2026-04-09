using Microsoft.EntityFrameworkCore;
using StackOverFIT.DAL.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace StackOverFIT.DAL.Tests;

public class DbContextTestsBase : IAsyncLifetime
{
    protected DbContextTestsBase(ITestOutputHelper output)
    {
        var databaseName = $"{GetType().FullName}_{Guid.NewGuid():N}.db";

        DbContextFactory = new DbContextSqLiteFactory(databaseName);
        StackOverFITDbContextSUT = DbContextFactory.CreateDbContext();
    }

    protected IDbContextFactory<StackOverFITDbContext> DbContextFactory { get; }

    protected StackOverFITDbContext StackOverFITDbContextSUT { get; }

    public async Task InitializeAsync()
    {
        await StackOverFITDbContextSUT.Database.EnsureDeletedAsync();
        await StackOverFITDbContextSUT.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await StackOverFITDbContextSUT.Database.EnsureDeletedAsync();
        await StackOverFITDbContextSUT.DisposeAsync();
    }
}