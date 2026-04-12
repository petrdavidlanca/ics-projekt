using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Mappers;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Repositories;
using StackOverFIT.DAL.UnitOfWork;
using Xunit;

namespace StackOverFIT.BL.Tests;

public class FacadeBaseTests
{
    private class TestEntity : IEntity
    {
        public Guid Id { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    private class TestListModel : ModelBase
    {
        public string Value { get; set; } = string.Empty;
    }

    private class TestDetailModel : ModelBase
    {
        public string Value { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new();
    }

    private class TestEntityMapper : IEntityMapper<TestEntity>
    {
        public void MapToExistingEntity(TestEntity existing, TestEntity updated)
        {
            existing.Value = updated.Value;
        }
    }

    private class TestModelMapper : IModelMapper<TestEntity, TestListModel, TestDetailModel>
    {
        public TestEntity MapToEntity(TestDetailModel model)
            => new() { Id = model.Id, Value = model.Value };

        public TestDetailModel MapToDetailModel(TestEntity entity)
            => new() { Id = entity.Id, Value = entity.Value };

        public IEnumerable<TestListModel> MapToListModel(IEnumerable<TestEntity> entities)
            => entities.Select(e => new TestListModel { Id = e.Id, Value = e.Value });

        public TestListModel MapToListModel(TestEntity? entity)
        {
            throw new NotImplementedException();
        }
    }

    private class FakeRepository : IRepository<TestEntity>
    {

        public readonly List<TestEntity> Storage = new();

        public IQueryable<TestEntity> Get() => Storage.AsQueryable();

        public Task<TestEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Storage.FirstOrDefault(e => e.Id == id));

        public TestEntity Insert(TestEntity entity)
        {
            Storage.Add(entity);
            return entity;
        }

        public Task<TestEntity> UpdateAsync(TestEntity entity, CancellationToken cancellationToken = default)
        {
            var existing = Storage.FirstOrDefault(e => e.Id == entity.Id)
                ?? throw new InvalidOperationException();

            existing.Value = entity.Value;
            return Task.FromResult(existing);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existing = Storage.FirstOrDefault(e => e.Id == id);
            if (existing != null)
                Storage.Remove(existing);

            return Task.CompletedTask;
        }

        public ValueTask<bool> ExistsAsync(TestEntity entity, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(Storage.Any(e => e.Id == entity.Id));
    }

    private class FakeUnitOfWork : IUnitOfWork
    {
        public readonly FakeRepository Repository = new();

        public IRepository<TEntity> GetRepository<TEntity, TEntityMapper>()
            where TEntity : class, IEntity
            where TEntityMapper : class, IEntityMapper<TEntity>
            => Repository as IRepository<TEntity>
               ?? throw new InvalidOperationException();
        public Task CommitAsync() => Task.CompletedTask;
        public Task CommitAsync(CancellationToken aha)
        {
            return Task.CompletedTask;
        }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    private class FakeUnitOfWorkFactory : IUnitOfWorkFactory
    {
        public readonly FakeUnitOfWork Uow = new();

        public IUnitOfWork Create() => Uow;
    }

    private class TestFacade
        : FacadeBase<TestEntity, TestListModel, TestDetailModel, TestEntityMapper>
    {
        public TestFacade(IUnitOfWorkFactory factory)
            : base(factory, new TestModelMapper())
        {
        }
    }

    [Fact]
    public async Task SaveAsync_Inserts_When_Not_Exists()
    {
        var factory = new FakeUnitOfWorkFactory();
        var facade = new TestFacade(factory);

        var model = new TestDetailModel { Value = "A" };

        var result = await facade.SaveAsync(model);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Single(factory.Uow.Repository.Storage);
    }

    [Fact]
    public async Task SaveAsync_Updates_When_Exists()
    {
        var factory = new FakeUnitOfWorkFactory();
        var facade = new TestFacade(factory);

        var entity = new TestEntity { Id = Guid.NewGuid(), Value = "Old" };
        factory.Uow.Repository.Storage.Add(entity);

        var model = new TestDetailModel { Id = entity.Id, Value = "New" };

        var result = await facade.SaveAsync(model);

        Assert.Equal("New", result.Value);
        Assert.Equal("New", entity.Value);
    }



    [Fact]
    public async Task DeleteAsync_Removes_Entity()
    {
        var factory = new FakeUnitOfWorkFactory();
        var facade = new TestFacade(factory);

        var entity = new TestEntity { Id = Guid.NewGuid(), Value = "X" };
        factory.Uow.Repository.Storage.Add(entity);

        await facade.DeleteAsync(entity.Id);

        Assert.Empty(factory.Uow.Repository.Storage);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_When_NotFound()
    {
        var factory = new FakeUnitOfWorkFactory();
        var facade = new TestFacade(factory);

        await facade.DeleteAsync(Guid.NewGuid());

        Assert.Empty(factory.Uow.Repository.Storage);
    }

    [Fact]
    public async Task SaveAsync_Throws_When_Collections_Are_Set()
    {
        var facade = new TestFacade(new FakeUnitOfWorkFactory());

        var model = new TestDetailModel
        {
            Value = "X",
            Tags = { "Invalid" }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => facade.SaveAsync(model));
    }
}
