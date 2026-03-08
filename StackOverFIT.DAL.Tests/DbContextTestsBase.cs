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
        /*pro vytvoreni vypisu do konzole*/
        //XUnitTestOutputConverter converter = new(output);
        //Console.SetOut(converter);

        // Vytvoří unikátní název souboru pro databázi pro každý spuštěný test
        var databaseName = $"{GetType().FullName}_{Guid.NewGuid():N}.db";

        // Použije vaši továrnu z DALu (kterou už tam máte připravenou)
        DbContextFactory = new DbContextSqLiteFactory(databaseName);
        StackOverFITDbContextSUT = DbContextFactory.CreateDbContext();
    }

    protected IDbContextFactory<StackOverFITDbContext> DbContextFactory { get; }

    // SUT = System Under Test (tohle budeme používat přímo v testech)
    protected StackOverFITDbContext StackOverFITDbContextSUT { get; }

    public async Task InitializeAsync()
    {
        // Před testem se ujistíme, že databáze neexistuje a vytvoříme čistou
        await StackOverFITDbContextSUT.Database.EnsureDeletedAsync();
        await StackOverFITDbContextSUT.Database.EnsureCreatedAsync();

        // Učitelé tady mají naplnění databáze testovacími daty (tzv. Seedy)
        // např: await dbx.SeedIngredients()... 
        // My to prozatím necháme prázdné a data si do DB vložíme přímo v konkrétních testech.
    }

    public async Task DisposeAsync()
    {
        // Po testu databázi smažeme, ať nám na disku nezůstávají desítky .db souborů
        await StackOverFITDbContextSUT.Database.EnsureDeletedAsync();
        await StackOverFITDbContextSUT.DisposeAsync();
    }
}