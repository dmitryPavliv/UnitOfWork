// pluskal

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using BaseDataModel;

namespace Repository.Contracts
{
    using static Contract;

    [ContractClassFor(typeof(IRepositoryWriterAsync<>))]
    internal sealed class RepositoryWriterAsyncContract<T> : IRepositoryWriterAsync<T> where T : class, IDataModel, new()
    {
        Task<T> IRepositoryWriterAsync<T>.DeleteAsync(Guid id)
        {
            return default(Task<T>);
        }

        Task<T> IRepositoryWriterAsync<T>.DeleteAsync(T item)
        {
            Requires<ArgumentNullException>(item != null, "item can't be null");

            return default(Task<T>);
        }

        Task<T> IRepositoryWriterAsync<T>.InsertAsync(T item)
        {
            Requires<ArgumentNullException>(item != null, "item can't be null");

            return default(Task<T>);
        }

        Task<IEnumerable<T>> IRepositoryWriterAsync<T>.InsertRangeAsync(IEnumerable<T> items)
        {
            Requires<ArgumentNullException>(items != null, "items can't be null");
            Requires<ArgumentException>(items.Any(), "items count should be greater than zero");

            return default(Task<IEnumerable<T>>);
        }

        Task IRepositoryWriterAsync<T>.UpdateAsync(T item)
        {
            Requires<ArgumentNullException>(item != null, "item can't be null");

            return default(Task);
        }
    }
}