// pluskal

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using BaseDataModel;

namespace Repository
{
    // <summary>
    /// Generic writer repository interface
    /// </summary>
    /// <typeparam name="T">Type must be a class and inherited from IDataModel interface</typeparam>
    public interface IRepositoryWriterAsync<T> where T : class, IDataModel, new()
    {
        /// <exception cref="ArgumentException">Item with specified Id not found.</exception>
        Task<T> DeleteAsync(Guid id);

        Task<T> DeleteAsync(T item);
        Task<T> InsertAsync(T item);
        Task<IEnumerable<T>> InsertRangeAsync(IEnumerable<T> items);
        Task UpdateAsync(T item);
    }
}