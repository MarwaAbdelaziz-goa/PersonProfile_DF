using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PersonProfile_DF.Api.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private static readonly SemaphoreSlim Locker = new SemaphoreSlim(1, 1);
        private readonly IMemoryCache _memoryCache;
        private readonly ApiProjectConfig _apiProjectConfig;
        public static List<object> CacheKeys = new List<object>();

        public MemoryCacheService(IMemoryCache memoryCache, IOptions<ApiProjectConfig> apiProjectConfig)
        {
            _apiProjectConfig = apiProjectConfig.Value;
            _memoryCache = memoryCache;
        }

        public T RetrieveOrCreate<T>(object key, Func<ICacheEntry, T> factory)
        {
            Locker.Wait();

            try
            {
                object obj;

                if (_memoryCache.TryGetValue(key, out obj))
                {
                    return (T)obj;
                }
                else
                {
                    CacheKeys.Add(key);

                    return _memoryCache.GetOrCreate(key, entry =>
                    {
                        entry.AbsoluteExpiration = DateTime.Now.AddSeconds(_apiProjectConfig.ServerSideCacheExpirationInSeconds);
                        return factory(entry);
                    });
                }
            }
            catch (Exception)
            {
                CacheKeys.Remove(key);
                throw;
            }
            finally
            {
                Locker.Release();
            }
        }

        public async Task<T> RetrieveOrCreateAsync<T>(object key, Func<ICacheEntry, Task<T>> factory)
        {
            await Locker.WaitAsync();

            try
            {
                object obj;

                if (_memoryCache.TryGetValue(key, out obj))
                {
                    return (T)obj;
                }
                else
                {
                    CacheKeys.Add(key);

                    return await _memoryCache.GetOrCreateAsync(key, async entry =>
                    {
                        entry.AbsoluteExpiration = DateTime.Now.AddSeconds(_apiProjectConfig.ServerSideCacheExpirationInSeconds);
                        return await factory(entry);
                    });
                }
            }
            catch (Exception exp)
            {
                CacheKeys.Remove(key);
                throw;
            }
            finally
            {
                Locker.Release();
            }
        }

        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public void RemoveAll()
        {
            foreach (var cacheKey in CacheKeys)
            {
                _memoryCache.Remove(cacheKey);
            }
        }
    }
}


