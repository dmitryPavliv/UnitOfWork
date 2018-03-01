// pluskal

using System.Data;
using BaseDataModel;

namespace AdoDataMapperAbstract
{
    public interface IAdoDataMapper<T> where T : class, IDataModel, new()
    {
        T Map(IDataReader reader);
    }
}