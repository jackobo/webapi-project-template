using Jackobo.AspNetCore.Framework.Types;
using Jackobo.WebApiTemplate.Core;
using Jackobo.WebApiTemplate.Core.Abstractions;
using Jackobo.WebApiTemplate.Core.DTO;
using Microsoft.Extensions.Logging;

namespace Jackobo.WebApiTemplate.Infrastructure.Services.Sample
{
    /// <summary>
    /// Concrete implementation of <see cref="ISampleService"/>.
    /// This sample keeps data in memory so the endpoint works out of the box without a database.
    ///
    /// TODO: Replace the in-memory storage with your real data source (EF Core DbContext,
    /// stored procedures, an external API via IHttpClientFactory, etc.).
    /// Return <c>ApiErrors.SomeError</c> to signal a failure — the framework converts it into a
    /// consistent error response.
    /// </summary>
    internal class SampleService : ISampleService
    {
        private static readonly List<SampleDTO> _store = new()
        {
            new SampleDTO { Id = 1, Name = "First sample", CreatedOnUtc = DateTime.UtcNow }
        };

        private readonly ILogger<SampleService> _logger;

        public SampleService(ILogger<SampleService> logger)
        {
            _logger = logger;
        }

        public async Task<OperationResult<SampleDTO>> GetByIdAsync(int id)
        {
            var item = _store.FirstOrDefault(x => x.Id == id);

            if (item is null)
            {
                _logger.LogWarning("Sample item with id {Id} was not found", id);
                return ApiErrors.RecordNotFound.WithErrorDetails<SampleDTO>($"Record with id {id} was not found");
            }

            return item;
        }

        public async Task<OperationResult<SampleDTO>> CreateAsync(string name)
        {
            var item = new SampleDTO
            {
                Id = _store.Count == 0 ? 1 : _store.Max(x => x.Id) + 1,
                Name = name,
                CreatedOnUtc = DateTime.UtcNow
            };

            _store.Add(item);

            return item;
        }
    }
}
