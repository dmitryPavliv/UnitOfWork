// pluskal

using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Effort;
using EF6UnitOfWork;
using Fakes;
using Moq;
using Repository;
using UnitOfWork;
using Xunit;

namespace EF6Repository.Tests
{
    public class BaseRepositoryTests
    {
        private readonly DbContext _context;
        private readonly IRepositoryReader<Foo> _repositoryReader;
        private readonly IRepositoryReaderAsync<Foo> _repositoryReaderAsync;
        private readonly IRepositoryWriter<Foo> _repositoryWriter;
        private readonly IRepositoryWriterAsync<Foo> _repositoryWriterAsync;

        public BaseRepositoryTests()
        {
            var connection = DbConnectionFactory.CreateTransient();
            this._context = new FooContext(connection);

            var repository = new BaseRepository<Foo>(new Ef6UnitOfWork(this._context, IsolationLevel.Unspecified));
            this._repositoryWriter = repository;
            this._repositoryReader = repository;
            this._repositoryWriterAsync = repository;
            this._repositoryReaderAsync = repository;
        }

        #region Test CRUD 

        [Fact]
        public void InsertsEntity()
        {
            //Arrange
            var initialCount = this._context.Set<Foo>().Count();
            var foo = new Foo {Id = Guid.NewGuid()};

            //Act
            this._repositoryWriter.Insert(foo);
            this._context.SaveChanges();

            //Assert
            Assert.Equal(initialCount + 1, this._context.Set<Foo>().Count());
            Assert.Contains(foo, this._context.Set<Foo>().AsEnumerable());
        }

        [Fact]
        public void InsertsRange()
        {
            //Arrange
            var initialCount = this._context.Set<Foo>().Count();
            var foo1 = new Foo {Id = Guid.NewGuid()};
            var foo2 = new Foo {Id = Guid.NewGuid()};

            //Act
            this._repositoryWriter.InsertRange(new[] {foo1, foo2});
            this._context.SaveChanges();

            //Assert
            Assert.Equal(initialCount + 2, this._context.Set<Foo>().Count());
            Assert.Contains(foo1, this._context.Set<Foo>().AsEnumerable());
            Assert.Contains(foo2, this._context.Set<Foo>().AsEnumerable());
        }

        [Fact]
        public void DeletesEntity()
        {
            //Arrange
            var foo = new Foo {Id = Guid.NewGuid()};
            this._repositoryWriter.Insert(foo);
            this._context.SaveChanges();
            var initialCount = this._context.Set<Foo>().Count();

            //Act
            this._repositoryWriter.Delete(foo);
            this._context.SaveChanges();

            //Assert
            Assert.Equal(initialCount - 1, this._context.Set<Foo>().Count());
            Assert.DoesNotContain(foo, this._context.Set<Foo>().AsEnumerable());
        }

        [Fact]
        public void DeletesEntityById()
        {
            //Arrange
            var foo = new Foo {Id = Guid.NewGuid()};
            this._repositoryWriter.Insert(foo);
            this._context.SaveChanges();
            var initialCount = this._context.Set<Foo>().Count();

            //Act
            this._repositoryWriter.Delete(foo.Id);
            this._context.SaveChanges();

            //Assert
            Assert.Equal(initialCount - 1, this._context.Set<Foo>().Count());
            Assert.DoesNotContain(foo, this._context.Set<Foo>().AsEnumerable());
        }

        [Fact]
        public void ThrowsExceptionWhenRecordToDeleteByIdNotFound()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentException>(() => this._repositoryWriter.Delete(Guid.NewGuid()));
        }

        [Fact]
        public void UpdatesEntity()
        {
            //Arrange
            var foo = new Foo {Id = Guid.NewGuid(), Name = "A"};
            this._repositoryWriter.Insert(foo);
            this._context.SaveChanges();

            //Act
            foo.Name = "B";
            this._repositoryWriter.Update(foo);
            this._context.SaveChanges();
            var item = this._repositoryReader.GetById(foo.Id);

            //Assert
            Assert.Equal("B", item.Name);
        }

        [Fact]
        public void GetsItemById()
        {
            //Arrange
            var foo = new Foo {Id = Guid.NewGuid()};
            this._repositoryWriter.Insert(foo);
            this._context.SaveChanges();

            //Act
            var item = this._repositoryReader.GetById(foo.Id);

            //Assert
            Assert.Same(foo, item);
        }

        [Fact]
        public void GetsAll()
        {
            //Arrange
            foreach (var f in this._context.Set<Foo>()) this._repositoryWriter.Delete(f);
            var foo1 = new Foo {Id = Guid.NewGuid()};
            var foo2 = new Foo {Id = Guid.NewGuid()};
            this._repositoryWriter.InsertRange(new[] {foo1, foo2});
            this._context.SaveChanges();

            //Act
            var items = this._repositoryReader.GetAll();

            //Assert
            Assert.Contains(foo1, items);
            Assert.Contains(foo2, items);
        }

        #endregion Test CRUD

        #region Test CRUD Async

        [Fact]
        public async Task InsertsEntityAsync()
        {
            //Arrange
            var initialCount = this._context.Set<Foo>().Count();
            var foo = new Foo {Id = Guid.NewGuid()};

            //Act
            await this._repositoryWriterAsync.InsertAsync(foo);
            await this._context.SaveChangesAsync();

            //Assert
            Assert.Equal(initialCount + 1, this._context.Set<Foo>().Count());
            Assert.Contains(foo, this._context.Set<Foo>().AsEnumerable());
        }

        [Fact]
        public async Task InsertsRangeAsync()
        {
            //Arrange
            var initialCount = this._context.Set<Foo>().Count();
            var foo1 = new Foo {Id = Guid.NewGuid()};
            var foo2 = new Foo {Id = Guid.NewGuid()};

            //Act
            await this._repositoryWriterAsync.InsertRangeAsync(new[] {foo1, foo2});
            await this._context.SaveChangesAsync();

            //Assert
            Assert.Equal(initialCount + 2, this._context.Set<Foo>().Count());
            Assert.Contains(foo1, this._context.Set<Foo>().AsEnumerable());
            Assert.Contains(foo2, this._context.Set<Foo>().AsEnumerable());
        }

        [Fact]
        public async Task DeletesEntityAsync()
        {
            //Arrange
            var foo = new Foo {Id = Guid.NewGuid()};
            await this._repositoryWriterAsync.InsertAsync(foo);
            this._context.SaveChanges();
            var initialCount = this._context.Set<Foo>().Count();

            //Act
            await this._repositoryWriterAsync.DeleteAsync(foo);
            await this._context.SaveChangesAsync();

            //Assert
            Assert.Equal(initialCount - 1, this._context.Set<Foo>().Count());
            Assert.DoesNotContain(foo, this._context.Set<Foo>().AsEnumerable());
        }

        [Fact]
        public async Task DeletesEntityByIdAsync()
        {
            //Arrange
            var foo = new Foo {Id = Guid.NewGuid()};
            await this._repositoryWriterAsync.InsertAsync(foo);
            await this._context.SaveChangesAsync();
            var initialCount = this._context.Set<Foo>().Count();

            //Act
            await this._repositoryWriterAsync.DeleteAsync(foo.Id);
            await this._context.SaveChangesAsync();

            //Assert
            Assert.Equal(initialCount - 1, this._context.Set<Foo>().Count());
            Assert.DoesNotContain(foo, this._context.Set<Foo>().AsEnumerable());
        }

        [Fact]
        public async Task ThrowsExceptionWhenRecordToDeleteByIdAsyncNotFound()
        {
            //Arrange

            //Act

            //Assert
            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () => await this._repositoryWriterAsync.DeleteAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdatesEntityAsync()
        {
            //Arrange
            var foo = new Foo {Id = Guid.NewGuid(), Name = "A"};
            await this._repositoryWriterAsync.InsertAsync(foo);
            await this._context.SaveChangesAsync();

            //Act
            foo.Name = "B";
            await this._repositoryWriterAsync.UpdateAsync(foo);
            await this._context.SaveChangesAsync();
            var item = await this._repositoryReaderAsync.GetByIdAsync(foo.Id);

            //Assert
            Assert.Equal("B", item.Name);
        }

        [Fact]
        public async Task GetsItemByIdAsync()
        {
            //Arrange
            var foo = new Foo {Id = Guid.NewGuid()};
            await this._repositoryWriterAsync.InsertAsync(foo);
            await this._context.SaveChangesAsync();

            //Act
            var item = await this._repositoryReaderAsync.GetByIdAsync(foo.Id);

            //Assert
            Assert.Same(foo, item);
        }

        [Fact]
        public async Task GetsAllAsync()
        {
            //Arrange
            foreach (var f in this._context.Set<Foo>()) await this._repositoryWriterAsync.DeleteAsync(f);
            var foo1 = new Foo {Id = Guid.NewGuid()};
            var foo2 = new Foo {Id = Guid.NewGuid()};
            await this._repositoryWriterAsync.InsertRangeAsync(new[] {foo1, foo2});
            await this._context.SaveChangesAsync();

            //Act
            var items = await this._repositoryReaderAsync.GetAllAsync();

            //Assert
            Assert.Contains(foo1, items);
            Assert.Contains(foo2, items);
        }

        #endregion CRUD Async

        #region Test PreConditions

        [Fact]
        public void PreConditionFailsWhenNotEf6UnitOfWorkPassedToConstructor()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentException>(
                () => { new BaseRepository<Foo>(new Mock<IUnitOfWork>().Object); });
        }

        [Fact]
        public void PreConditionFailedWhenTryingToInsertNull()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(
                () => { this._repositoryWriter.Insert(null); });
        }

        [Fact]
        public void PreConditionFailedWhenTryingToInsertNullRange()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(
                () => { this._repositoryWriter.InsertRange(null); });
        }

        [Fact]
        public void PreConditionFailedWhenTryingToUpdateNullItem()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(
                () => { this._repositoryWriter.Update(null); });
        }

        [Fact]
        public void PreConditionFailedWhenTryingToDeleteNullItem()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(
                () => { this._repositoryWriter.Delete(null); });
        }

        #endregion Test PreConditions
    }
}