// pluskal

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using BaseDataModel;

namespace Repository
{
    /// <summary>
    ///     Generic repository reader interface
    /// </summary>
    /// <typeparam name="T">Type must be a class and inherited from IDataModel interface</typeparam>
    public interface IRepositoryReader<T> where T : class, IDataModel, new()
    {
        /// <summary>
        ///     Gets all items in persistence storage
        /// </summary>
        /// <returns>All items of type T in persistence storage</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        ///     Gets item by id
        /// </summary>
        /// <param name="id">Id of item to get</param>
        /// <returns>Item if found or null otherwise</returns>
        T GetById(Guid id);
    }
}