// pluskal

using System;
using BaseDataModel;

namespace AdoTableNameProviderAbstract
{
    public interface IAdoTableNameProvider
    {
        String GetTableName<T>() where T : class, IDataModel, new();
    }
}