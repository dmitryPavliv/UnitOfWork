// pluskal

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdoDataMapperAbstract;
using AdoDbCommandProviderAbstract;
using Fakes;
using Moq;
using Repository;
using Xunit;

namespace AdoRepository.Tests
{
    public class BaseRepositoryTests : IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly IRepositoryReader<Foo> _repositoryReader;
        private readonly IRepositoryWriter<Foo> _repositoryWriter;

        private IDbTransaction _transaction;

        public BaseRepositoryTests()
        {
            this._connection = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;
                                                     Initial Catalog=AdoUoWTestDb;
                                                     Integrated Security=true");

            #region Mock

            var dataMapperStup = new Mock<IAdoDataMapper<Foo>>();
            dataMapperStup.Setup(f => f.Map(It.IsAny<IDataReader>()))
                .Returns(
                    (IDataReader reader) =>
                        new Foo
                        {
                            Id = Guid.Parse(reader["Id"].ToString()),
                            Name = reader["Name"].ToString()
                        });

            var commandProviderStud = new Mock<IAdoDbCommandProvider<Foo>>();
            commandProviderStud.Setup(f => f.SelectByIdCommand(It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>(), It.IsAny<Guid>()))
                .Returns(
                    (IDbConnection connection, IDbTransaction transaction, Guid id) =>
                    {
                        IDbCommand command = new SqlCommand();

                        command.Transaction = this._transaction;
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT * FROM [Foos] WHERE Id = @Id";
                        command.Parameters.Add(
                            new SqlParameter {ParameterName = "@Id", DbType = DbType.Guid, Value = id});

                        return command;
                    });
            commandProviderStud.Setup(f => f.DeleteCommand(It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>(), It.IsAny<Foo>()))
                .Returns(
                    (IDbConnection connection, IDbTransaction transaction, Foo item) =>
                    {
                        IDbCommand command = new SqlCommand();

                        command.Connection = connection;
                        command.Transaction = this._transaction;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "DELETE FROM [Foos] WHERE Id = @Id";
                        command.Parameters.Add(
                            new SqlParameter {ParameterName = "@Id", DbType = DbType.Guid, Value = item.Id});

                        return command;
                    });
            commandProviderStud.Setup(f => f.InsertCommand(It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>(), It.IsAny<Foo>()))
                .Returns(
                    (IDbConnection connection, IDbTransaction transaction, Foo item) =>
                    {
                        IDbCommand command = new SqlCommand();

                        command.Connection = connection;
                        command.Transaction = this._transaction;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "INSERT INTO [Foos] VALUES(@Id,@Name)";
                        command.Parameters.Add(
                            new SqlParameter {ParameterName = "@Id", DbType = DbType.Guid, Value = item.Id});
                        command.Parameters.Add(
                            new SqlParameter {ParameterName = "@Name", DbType = DbType.String, Value = item.Name});

                        return command;
                    });
            commandProviderStud.Setup(f => f.UpdateCommand(It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>(), It.IsAny<Foo>()))
                .Returns(
                    (IDbConnection connection, IDbTransaction transaction, Foo item) =>
                    {
                        IDbCommand command = new SqlCommand();

                        command.Connection = connection;
                        command.Transaction = this._transaction;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "UPDATE T SET T.Name = @Name FROM [Foos] T WHERE T.Id = @Id";
                        command.Parameters.Add(
                            new SqlParameter {ParameterName = "@Id", DbType = DbType.Guid, Value = item.Id});
                        command.Parameters.Add(
                            new SqlParameter {ParameterName = "@Name", DbType = DbType.String, Value = item.Name});

                        return command;
                    });
            commandProviderStud.Setup(f => f.SelectAllCommand(It.IsAny<IDbConnection>(), It.IsAny<IDbTransaction>()))
                .Returns(
                    (IDbConnection connection, IDbTransaction transaction) =>
                    {
                        IDbCommand command = new SqlCommand();

                        command.Connection = connection;
                        command.Transaction = this._transaction;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT * FROM [Foos]";

                        return command;
                    });

            #endregion

            var repository = new BaseRepository<Foo>(this._connection, commandProviderStud.Object, dataMapperStup.Object);
            this._repositoryWriter = repository;
            this._repositoryReader = repository;

            this._connection.Open();
        }

        public void Dispose()
        {
            if (this._connection.State == ConnectionState.Open) this._connection.Close();
        }

        #region Test CRUD

        [Fact]
        public void GetsByIdReturnsNullWhenIdNotFound()
        {
            //Arrange
            var wrongId = Guid.NewGuid();

            //Act
            var foo = this._repositoryReader.GetById(wrongId);

            //Assert
            Assert.Null(foo);
        }

        [Fact]
        public void GetsByIdReturnsItemCorrectlyWhenFound()
        {
            //Arrange
            var foo = new Foo {Id = Guid.NewGuid(), Name = "A"};
            this._repositoryWriter.Insert(foo);

            //Act
            var fooDb = this._repositoryReader.GetById(foo.Id);

            //Assert
            Assert.NotNull(fooDb);
            Assert.Equal(foo.Id, fooDb.Id);
            Assert.Equal("A", fooDb.Name);
        }

        [Fact]
        public void InsertsNewRecordCorrectly()
        {
            //Arrange
            var foo = new Foo {Id = Guid.NewGuid(), Name = "B"};

            //Act
            this.BeginTransaction();
            this._repositoryWriter.Insert(foo);
            this.CommitTransaction();
            var fooFromDb = this._repositoryReader.GetById(foo.Id);

            //Assert
            Assert.NotNull(fooFromDb);
            Assert.Equal(foo.Id, fooFromDb.Id);
            Assert.Equal("B", fooFromDb.Name);
        }

        [Fact]
        public void InsertsRangeCorrectly()
        {
            //Arrange
            this.BeginTransaction();
            var foo1 = new Foo {Id = Guid.NewGuid(), Name = "A"};
            var foo2 = new Foo {Id = Guid.NewGuid(), Name = "B"};

            //Act
            this._repositoryWriter.InsertRange(new[] {foo1, foo2});
            this.CommitTransaction();
            var foo1FromDb = this._repositoryReader.GetById(foo1.Id);
            var foo2FromDb = this._repositoryReader.GetById(foo2.Id);

            //Assert
            Assert.NotNull(foo1FromDb);
            Assert.Equal(foo1.Id, foo1FromDb.Id);
            Assert.Equal("A", foo1FromDb.Name);

            Assert.NotNull(foo2FromDb);
            Assert.Equal(foo2.Id, foo2FromDb.Id);
            Assert.Equal("B", foo2FromDb.Name);
        }

        [Fact]
        public void DeletesRecordByIdCorrectly()
        {
            //Arrange
            this.BeginTransaction();
            var foo = new Foo {Id = Guid.NewGuid(), Name = "B"};
            this._repositoryWriter.Insert(foo);
            this.CommitTransaction();

            //Act
            this.BeginTransaction();
            this._repositoryWriter.Delete(foo.Id);
            this.CommitTransaction();
            var fooFromDb = this._repositoryReader.GetById(foo.Id);

            //Assert
            Assert.Null(fooFromDb);
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
        public void DeletesRecordCorrectly()
        {
            //Arrange
            this.BeginTransaction();
            var foo = new Foo {Id = Guid.NewGuid(), Name = "B"};
            this._repositoryWriter.Insert(foo);
            this.CommitTransaction();

            //Act
            this.BeginTransaction();
            this._repositoryWriter.Delete(foo);
            this.CommitTransaction();
            var fooFromDb = this._repositoryReader.GetById(foo.Id);

            //Assert
            Assert.Null(fooFromDb);
        }

        [Fact]
        public void UpdatesRecordCorrectly()
        {
            //Arrange
            this.BeginTransaction();
            var foo = new Foo {Id = Guid.NewGuid(), Name = "B"};
            this._repositoryWriter.Insert(foo);
            this.CommitTransaction();

            //Act
            foo.Name = "A";
            this.BeginTransaction();
            this._repositoryWriter.Update(foo);
            this.CommitTransaction();
            var fooFromDb = this._repositoryReader.GetById(foo.Id);

            //Assert
            Assert.Equal("A", fooFromDb.Name);
        }

        [Fact]
        public void GetsAllCorrectly()
        {
            //Arrange
            this.BeginTransaction();
            IDbCommand command = new SqlCommand("DELETE FROM [Foos]");
            command.Connection = this._connection;
            command.Transaction = this._transaction;
            command.ExecuteNonQuery();
            var foo1 = new Foo {Id = Guid.NewGuid(), Name = "A"};
            var foo2 = new Foo {Id = Guid.NewGuid(), Name = "B"};
            this._repositoryWriter.InsertRange(new[] {foo1, foo2});
            this.CommitTransaction();

            //Act
            var foos = this._repositoryReader.GetAll().ToList();

            //Assert
            Assert.Equal(2, foos.Count);

            Assert.Contains(foo1, foos);
            Assert.Contains(foo2, foos);
        }

        #endregion Test CRUD

        #region Test PreConditions

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

        #region Private Methods

        private void BeginTransaction()
        {
            this._transaction = this._connection.BeginTransaction();
        }

        private void CommitTransaction()
        {
            this._transaction?.Commit();
            this._transaction = null;
        }

        #endregion Private Methods
    }
}