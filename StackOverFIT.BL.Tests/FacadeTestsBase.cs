using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackOverFIT.BL.Mappers;
using StackOverFIT.DAL;
using StackOverFIT.DAL.Factories;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.UnitOfWork;
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

        var services = new ServiceCollection();

        services.AddSingleton<IDbContextFactory<StackOverFITDbContext>>(DbContextFactory);

        services.AddTransient<LibraryEntityMapper>();
        services.AddTransient<VideoGameEntityMapper>();
        services.AddTransient<UserEntityMapper>();
        services.AddTransient<LibraryVideoGameEntityMapper>();

        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());

        var provider = services.BuildServiceProvider();

        ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
        UnitOfWorkFactory = new UnitOfWorkFactory(DbContextFactory, ServiceScopeFactory);

        LibraryModelMapper = new LibraryModelMapper();
        UserModelMapper = new UserModelMapper();
        VideoGameModelMapper = new VideoGameModelMapper();
    }

    protected IDbContextFactory<StackOverFITDbContext> DbContextFactory { get; }
    protected StackOverFITDbContext StackOverFITDbContextSUT { get; }
    protected IServiceScopeFactory ServiceScopeFactory { get; }

    protected LibraryModelMapper LibraryModelMapper { get; }
    protected UserModelMapper UserModelMapper { get; }
    protected VideoGameModelMapper VideoGameModelMapper { get; }
    protected IUnitOfWorkFactory UnitOfWorkFactory { get; }

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

