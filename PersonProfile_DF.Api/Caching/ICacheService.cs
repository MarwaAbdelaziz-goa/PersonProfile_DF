using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace PersonProfile_DF.Api.Caching
{
    public interface ICacheService
    {
        T RetrieveOrCreate<T>(object key, Func<ICacheEntry, T> factory);
        Task<T> RetrieveOrCreateAsync<T>(object key, Func<ICacheEntry, Task<T>> factory);
        void Remove(string cacheKey);
        void RemoveAll();
    }
}

