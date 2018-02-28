// pluskal

using System;
using System.Data;
using BaseDataModel;

namespace AdoDbCommandProviderAbstract
{
    public interface IAdoDbCommandProvider<T> where T : class, IDataModel, new()
    {
        IDbCommand DeleteCommand(IDbConnection connection, IDbTransaction transaction, T item);
        IDbCommand InsertCommand(IDbConnection connection, IDbTransaction transaction, T item);

        IDbCommand SelectAllCommand(IDbConnection connection, IDbTransaction transaction);

        IDbCommand SelectByIdCommand(IDbConnection connection, IDbTransaction transaction, Guid id);

        IDbCommand UpdateCommand(IDbConnection connection, IDbTransaction transaction, T item);
    }
}