using System;

namespace Presentation.Attributes
{
    /// <summary>
    /// Indicates that this endpoint requires a valid API key in the X-API-Key header.
    /// Enforced by ApiKeyValidationMiddleware.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyRequiredAttribute : Attribute
    {
    }
}
