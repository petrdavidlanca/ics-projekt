using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Repositories;
using Xunit;

namespace StackOverFIT.DAL.Tests;

public class RepositoryTests
{

    private class TestEntity : IEntity
    {
        public Guid Id { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    private class TestDbContext : DbContext
    {
        public DbSet<TestEntity> TestEntities => Set<TestEntity>();

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);
            base.OnModelCreating(modelBuilder);
        }
    }


    private class TestEntityMapper : IEntityMapper<TestEntity>
    {
        public void MapToExistingEntity(TestEntity existing, TestEntity updated)
        {
            existing.Value = updated.Value;
        }
    }

    private TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("Filename=:memory:")
            .Options;

        var ctx = new TestDbContext(options);
        ctx.Database.OpenConnection();
        ctx.Database.EnsureCreated();
        return ctx;
    }

    [Fact]
    public async Task Insert_Adds_Entity_To_DbSet()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        var entity = new TestEntity { Id = Guid.NewGuid(), Value = "A" };

        repo.Insert(entity);
        await ctx.SaveChangesAsync();

        Assert.Equal(1, ctx.Set<TestEntity>().Count());
    }

    [Fact]
    public async Task Get_Returns_All_Entities()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        ctx.Set<TestEntity>().Add(new TestEntity { Id = Guid.NewGuid(), Value = "A" });
        ctx.Set<TestEntity>().Add(new TestEntity { Id = Guid.NewGuid(), Value = "B" });
        await ctx.SaveChangesAsync();

        var all = repo.Get().ToList();

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Entity_When_Exists()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        var id = Guid.NewGuid();
        ctx.Set<TestEntity>().Add(new TestEntity { Id = id, Value = "A" });
        await ctx.SaveChangesAsync();

        var result = await repo.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("A", result!.Value);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Null_When_Not_Found()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_Updates_Tracked_Entity_Using_Mapper()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        var id = Guid.NewGuid();
        ctx.Set<TestEntity>().Add(new TestEntity { Id = id, Value = "Old" });
        await ctx.SaveChangesAsync();

        var updated = new TestEntity { Id = id, Value = "New" };

        await repo.UpdateAsync(updated);
        await ctx.SaveChangesAsync();

        var result = await ctx.Set<TestEntity>().FindAsync(id);

        Assert.Equal("New", result!.Value);
    }

    [Fact]
    public async Task UpdateAsync_Throws_When_Entity_Not_Found()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        var updated = new TestEntity { Id = Guid.NewGuid(), Value = "X" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => repo.UpdateAsync(updated));
    }

    [Fact]
    public async Task DeleteAsync_Removes_Entity()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        var id = Guid.NewGuid();
        ctx.Set<TestEntity>().Add(new TestEntity { Id = id, Value = "A" });
        await ctx.SaveChangesAsync();

        await repo.DeleteAsync(id);
        await ctx.SaveChangesAsync();

        Assert.Null(await ctx.Set<TestEntity>().FindAsync(id));
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_When_NotFound()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        await repo.DeleteAsync(Guid.NewGuid());
        await ctx.SaveChangesAsync();

        Assert.Empty(ctx.Set<TestEntity>());
    }

    [Fact]
    public async Task ExistsAsync_Returns_True_When_Entity_Exists()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        var id = Guid.NewGuid();
        var entity = new TestEntity { Id = id, Value = "A" };

        ctx.Set<TestEntity>().Add(entity);
        await ctx.SaveChangesAsync();

        var exists = await repo.ExistsAsync(entity);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_Returns_False_When_Entity_Not_Exists()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        var entity = new TestEntity { Id = Guid.NewGuid(), Value = "A" };

        var exists = await repo.ExistsAsync(entity);

        Assert.False(exists);
    }

    [Fact]
    public async Task ExistsAsync_Returns_False_When_Id_Empty()
    {
        using var ctx = CreateContext();
        var repo = new Repository<TestEntity>(ctx, new TestEntityMapper());

        var entity = new TestEntity { Id = Guid.Empty, Value = "A" };

        var exists = await repo.ExistsAsync(entity);

        Assert.False(exists);
    }
}
