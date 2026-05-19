using System;

namespace Presentation.Attributes
{
    /// <summary>
    /// Indicates that this endpoint requires both API key AND authorization (JWT token).
    /// Use this for POST, PUT, DELETE, and other write operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRequiredAttribute : Attribute
    {
    }
}
