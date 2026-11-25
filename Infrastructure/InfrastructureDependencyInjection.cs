using Jackobo.AspNetCore.Framework.Configurations;
using Jackobo.WebApiTemplate.Core.Abstractions;
using Jackobo.WebApiTemplate.Infrastructure.ConfigSections;
using Jackobo.WebApiTemplate.Infrastructure.Services.Sample;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jackobo.WebApiTemplate.Infrastructure
{
    /// <summary>
    /// Dependency injection entry point for the Infrastructure layer.
    /// Register here everything that talks to the outside world: database contexts,
    /// HTTP clients, message brokers, authentication, etc.
    /// </summary>
    public static class InfrastructureDependencyInjection
    {
        public static void AddServiceInfrastructure(this WebApplicationBuilder appBuilder)
        {
            var services = appBuilder.Services;

            // --- Application services ----------------------------------------------------
            // Map each Core abstraction to its Infrastructure implementation.
            services.AddScoped<ISampleService, SampleService>();

            // --- External HTTP dependencies (sample) -------------------------------------
            // TODO: Configure the HttpClients your service needs. Remove if not used.
            services.AddHttpClient(HttpClientsNames.SampleExternalApi, client =>
            {
                var sampleConfig = appBuilder.Configuration.GetConfigSection<SampleConfigSection>();
                if (!string.IsNullOrWhiteSpace(sampleConfig.Url))
                {
                    client.BaseAddress = new Uri(sampleConfig.Url);
                }
                client.Timeout = TimeSpan.FromSeconds(20);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // --- Database (sample, disabled) ---------------------------------------------
            // TODO: Uncomment and configure when you add an EF Core DbContext.
            // Remember to also uncomment the EF Core package in Infrastructure.csproj.
            // services.AddDbContext<DBModels.SampleDbContext>(options =>
            // {
            //     var cnnStr = appBuilder.Configuration.ResolveConnectionStringCredentials("SampleDb");
            //     options.UseSqlServer(cnnStr);
            // });

            // TODO: Register authentication here when your service needs it
            // (e.g. appBuilder.Services.AddAuthentication(...).AddJwtBearer(...)).
        }
    }
}
