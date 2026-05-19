using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Presentation.Middleware
{
    public class ApiKeyValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiKeyValidationMiddleware> _logger;
        private const string ApiKeyHeaderName = "X-API-Key";

        public ApiKeyValidationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyValidationMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip API key validation for swagger endpoints
            if (context.Request.Path.StartsWithSegments("/swagger") || 
                context.Request.Path.StartsWithSegments("/api-docs"))
            {
                await _next(context);
                return;
            }

            // Extract API key from header
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyValue))
            {
                _logger.LogWarning($"Request to {context.Request.Path} missing API key header");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "API key is missing" });
                return;
            }

            var apiKey = apiKeyValue.ToString();

            // Validate API key
            var validApiKeys = _configuration.GetSection("ApiSettings:ValidApiKeys").Get<List<string>>();
            
            if (validApiKeys == null || !validApiKeys.Contains(apiKey))
            {
                _logger.LogWarning($"Request to {context.Request.Path} with invalid API key");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid API key" });
                return;
            }

            // Store API key in context for later use
            context.Items["ApiKey"] = apiKey;

            await _next(context);
        }
    }
}
