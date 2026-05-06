using StackOverFIT.DAL.Factories;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Migrator;
using StackOverFIT.DAL.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace StackOverFIT.DAL;

public static class DALInstaller
{
    public static IServiceCollection AddDALServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbContextFactory<StackOverFITDbContext>>(serviceProvider =>
        {
            var dalOptions = serviceProvider.GetRequiredService<IOptions<DALOptions>>();
            return new DbContextSqLiteFactory(dalOptions.Value.DatabaseFilePath);
        });
        services.AddSingleton<IDbMigrator, DbMigrator>();

        services.AddSingleton<LibraryEntityMapper>();
        services.AddSingleton<LibraryVideoGameEntityMapper>();
        services.AddSingleton<UserEntityMapper>();
        services.AddSingleton<VideoGameEntityMapper>();

        return services;
    }
}
