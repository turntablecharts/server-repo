using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private const int DEFAULT_EXPIRATION_MINUTES = 30;

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_memoryCache.TryGetValue(cacheKey, out T cachedItem))
            {
                _logger.LogInformation($"Cache hit for key: {cacheKey}");
                return cachedItem;
            }

            _logger.LogInformation($"Cache miss for key: {cacheKey}. Fetching from source...");
            T item = await factory();

            if (item != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(expiration ?? TimeSpan.FromMinutes(DEFAULT_EXPIRATION_MINUTES))
                    .SetSize(1) // Each entry has a size of 1
                    .SetPriority(CacheItemPriority.Normal);

                _memoryCache.Set(cacheKey, item, cacheOptions);
            }

            return item;
        }

        public void Remove(string cacheKey)
        {
            _logger.LogInformation($"Removing cache key: {cacheKey}");
            _memoryCache.Remove(cacheKey);
        }

        public async Task InvalidateAndRepopulateAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            _logger.LogInformation($"Invalidating and repopulating cache key: {cacheKey}");
            _memoryCache.Remove(cacheKey);
            await GetOrCreateAsync(cacheKey, factory, expiration);
        }
    }
}
