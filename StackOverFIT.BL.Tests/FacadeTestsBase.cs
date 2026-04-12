using StackOverFIT.BL.Mappers;
using StackOverFIT.DAL;
using StackOverFIT.DAL.Factories;
using StackOverFIT.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace StackOverFIT.BL.Tests;

public class FacadeTestsBase : IAsyncLifetime
{

    protected FacadeTestsBase(ITestOutputHelper output)
    {
        var databaseName = $"{GetType().FullName}_{Guid.NewGuid():N}.db";

        DbContextFactory = new DbContextSqLiteFactory(databaseName);
        StackOverFITDbContextSUT = DbContextFactory.CreateDbContext();

        LibraryModelMapper = new LibraryModelMapper();
        UserModelMapper = new UserModelMapper();
        VideoGameModelMapper = new VideoGameModelMapper();

        UnitOfWorkFactory = new UnitOfWorkFactory(DbContextFactory, ServiceScopeFactory);
    }

    protected IDbContextFactory<StackOverFITDbContext> DbContextFactory { get; }
    protected StackOverFITDbContext StackOverFITDbContextSUT { get; }
    protected IServiceScopeFactory ServiceScopeFactory { get; }

    protected LibraryModelMapper LibraryModelMapper { get; }
    protected UserModelMapper UserModelMapper { get; }
    protected VideoGameModelMapper VideoGameModelMapper { get; }
    protected UnitOfWorkFactory UnitOfWorkFactory { get; }

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
