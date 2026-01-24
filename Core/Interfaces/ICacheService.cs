using System;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null);
        void Remove(string cacheKey);
        Task InvalidateAndRepopulateAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null);
    }
}
