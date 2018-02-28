// pluskal

using System;

namespace BaseDataModel
{
    /// <summary>
    ///     Base interface for all data models in project
    /// </summary>
    public interface IDataModel
    {
        Guid Id { get; set; }
    }
}