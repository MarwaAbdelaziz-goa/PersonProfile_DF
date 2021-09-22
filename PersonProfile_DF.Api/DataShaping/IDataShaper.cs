using System.Collections.Generic;
using PersonProfile_DF.Api.Models;

namespace PersonProfile_DF.Api.DataShaping
{
    public interface IDataShaper<T>
    {
        IEnumerable<Entity> ShapeData(IEnumerable<T> entities, string fieldsString);
        Entity ShapeData(T entity, string fieldsString);
    }
}

