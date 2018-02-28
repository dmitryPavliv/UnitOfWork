// pluskal

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using BaseDataModel;

namespace Repository.Contracts
{
    [ContractClassFor(typeof(IRepositoryReaderAsync<>))]
    internal sealed class RepositoryReaderAsyncContract<T> : IRepositoryReaderAsync<T> where T : class, IDataModel, new()
    {
        Task<IEnumerable<T>> IRepositoryReaderAsync<T>.GetAllAsync()
        {
            return default(Task<IEnumerable<T>>);
        }

        Task<T> IRepositoryReaderAsync<T>.GetByIdAsync(Guid id)
        {
            return default(Task<T>);
        }
    }
}