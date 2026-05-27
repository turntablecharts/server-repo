using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Presentation.Attributes;

namespace Presentation.Middleware
{
    public class AuthorizationEnforcementMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthorizationEnforcementMiddleware> _logger;
        // List of paths that don't require authorization
        private readonly List<string> _publicPaths = new List<string>
        {
            "/api/account/login",
            "/api/account/logout",
            "/api/god/cache/clear",
            "/swagger"
        };

        public AuthorizationEnforcementMiddleware(RequestDelegate next, ILogger<AuthorizationEnforcementMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip check for swagger endpoints
            if (context.Request.Path.StartsWithSegments("/swagger") || 
                context.Request.Path.StartsWithSegments("/api-docs"))
            {
                await _next(context);
                return;
            }

            // Check if path is in public paths list
            string requestPath = context.Request.Path.Value?.ToLower() ?? "";
            bool isPublicPath = false;
            foreach (var path in _publicPaths)
            {
                if (requestPath.Contains(path.ToLower()))
                {
                    isPublicPath = true;
                    break;
                }
            }

            if (isPublicPath)
            {
                await _next(context);
                return;
            }

            // Check if this is a GET request - GET requests only need API key (already validated)
            if (context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // For non-GET requests (POST, PUT, DELETE, etc.), verify Authorization header
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                _logger.LogWarning($"Request to {context.Request.Path} ({context.Request.Method}) missing Authorization header");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Authorization token is required for this operation" });
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"Request to {context.Request.Path} ({context.Request.Method}) with invalid Authorization header format");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid Authorization header format. Expected: Bearer <token>" });
                return;
            }

            await _next(context);
        }
    }
}
