using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Presentation.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        public RateLimitingMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            IConfiguration configuration,
            ILogger<RateLimitingMiddleware> logger
        )
        {
            _next = next;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip rate limiting for swagger endpoints
            if (
                context.Request.Path.StartsWithSegments("/swagger")
                || context.Request.Path.StartsWithSegments("/api-docs")
            )
            {
                await _next(context);
                return;
            }

            var identifier = GetClientIdentifier(context);
            var rateLimitKey = $"rate_limit_{identifier}";

            // FIX 1: Set the size inside the GetOrCreate factory
            var requestCount = _cache.GetOrCreate(
                rateLimitKey,
                entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                    entry.Size = 1; // <--- Set the size here
                    return 0;
                }
            );

            var maxRequestsPerMinute = _configuration.GetValue<int>(
                "ApiSettings:MaxRequestsPerMinute",
                100
            );

            if (requestCount >= maxRequestsPerMinute)
            {
                _logger.LogWarning($"Rate limit exceeded for client: {identifier}");
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers.Add("Retry-After", "60");
                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        message = "Rate limit exceeded. Maximum requests per minute: "
                            + maxRequestsPerMinute,
                    }
                );
                return;
            }

            // FIX 2: Instead of the shorthand _cache.Set extension method,
            // use a MemoryCacheEntryOptions object to pass the size.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                .SetSize(1);

            // Pass the options object as the 3rd argument
            _cache.Set(rateLimitKey, requestCount + 1, cacheEntryOptions);

            // Add rate limit info to response headers
            context.Response.Headers.Add("X-RateLimit-Limit", maxRequestsPerMinute.ToString());
            context.Response.Headers.Add(
                "X-RateLimit-Remaining",
                (maxRequestsPerMinute - requestCount - 1).ToString()
            );

            await _next(context);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            // Try to get API key from header first
            if (context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
            {
                return $"api_{apiKey}";
            }

            // Fall back to IP address
            var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return $"ip_{remoteIpAddress}";
        }
    }
}
