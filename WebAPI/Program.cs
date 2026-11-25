using Jackobo.AspNetCore.Framework;
using Jackobo.WebApiTemplate.Core;
using Jackobo.WebApiTemplate.Infrastructure;

namespace Jackobo.WebApiTemplate.WebAPI
{
    /// <summary>
    /// Application entry point.
    /// The Jackobo framework wires up the standard pipeline (mediator, validation, error handling,
    /// logging, CORS, Swagger, Redis, etc.). You only provide the options and the
    /// service/middleware hooks.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(128, 150);

            var options = new JackoboWebApiOptions(
                // The application's display title, used as the default Swagger document title.
                // Supplied when the project is scaffolded (the "Application title" parameter of
                // the template).
                appTitle: "Jackobo WebApiTemplate API",
                // The assembly that contains the mediator handlers. Handlers are discovered here,
                // so they do not need to be registered manually.
                mediatorHandlersAssembly: typeof(CoreDependencyInjection).Assembly,
                registerServices: RegisterServices,
                registerMiddlewares: RegisterMiddlewares,
                // The application specific error catalog used by the OperationResult pipeline.
                applicationSpecificErrors: typeof(ApiErrors));

            var app = new JackoboWebapi(options);

            app.Run();
        }

        /// <summary>
        /// Hook to register the HTTP request pipeline middlewares.
        /// </summary>
        private static void RegisterMiddlewares(WebApplication application)
        {
            // TODO: Add middlewares here as your service grows
            // (e.g. application.UseAuthentication(); application.UseAuthorization();).
        }

        /// <summary>
        /// Hook to register the services for each layer. Add cross-cutting registrations here too.
        /// </summary>
        private static void RegisterServices(WebApplicationBuilder appBuilder)
        {
            //#if (UseRedis)
            // Redis needs no registration code here: the framework registers ISharedRedisService
            // and IDedicatedRedisService by itself, driven by configuration.
            //   - shared    -> "Redis:Shared:ConnectionString" points at the instance shared by
            //                  all the Jackobo services; this service declares
            //                  "Redis:KeyPrefix" in appsettings.json.
            //   - dedicated -> this service owns the instance and declares
            //                  "Redis:Dedicated:ConnectionString" per environment.
            // "Redis:KeyPrefix" is mandatory in both cases - it namespaces every key this
            // service writes. Inject ISharedRedisService / IDedicatedRedisService where needed.
            //#endif
            //#if (!UseRedis)
            // NOTE: this service does not use Redis, but "Redis:KeyPrefix" stays in
            // appsettings.json: the framework refuses to register a Redis client without a per
            // service key prefix as soon as a Redis connection string is configured - including
            // a shared one that may come from the framework environment defaults.
            //#endif

            appBuilder.AddServiceCore();
            appBuilder.AddServiceInfrastructure();

            // TODO: Register additional cross-cutting dependencies here if needed
            // (e.g. background services, caches, clients).
        }
    }
}
