using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jackobo.WebApiTemplate.Core
{
    /// <summary>
    /// Dependency injection entry point for the Core layer.
    /// Register here the services that live in the Core project (pure application logic,
    /// in-memory caches, domain helpers, etc.).
    ///
    /// NOTE: The mediator handlers (IStandardRequestHandler implementations) are discovered
    /// automatically by the Jackobo framework based on the assembly passed to
    /// JackoboWebApiOptions in Program.cs, so you do NOT need to register them here.
    /// </summary>
    public static class CoreDependencyInjection
    {
        public static void AddServiceCore(this WebApplicationBuilder appBuilder)
        {
            // TODO: Register Core-layer services here, for example:
            // appBuilder.Services.AddSingleton<ISomeDomainService, SomeDomainService>();
        }
    }
}
