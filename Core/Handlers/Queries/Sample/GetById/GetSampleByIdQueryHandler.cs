using Jackobo.AspNetCore.Framework.Mediator;
using Jackobo.AspNetCore.Framework.Types;
using Jackobo.WebApiTemplate.Core.Abstractions;
using Jackobo.WebApiTemplate.Core.DTO;

namespace Jackobo.WebApiTemplate.Core.Handlers.Queries.Sample.GetById
{
    /// <summary>
    /// Handler for <see cref="GetSampleByIdQuery"/>.
    /// Handlers are discovered automatically by the framework (no manual DI registration needed).
    /// They are typically thin: they orchestrate one or more services and return an
    /// <see cref="OperationResult{T}"/>. An OperationResult can be created implicitly from the
    /// payload (success) or from an <c>ApplicationError</c> (failure).
    /// </summary>
    internal class GetSampleByIdQueryHandler : IStandardRequestHandler<GetSampleByIdQuery, SampleDTO>
    {
        private readonly ISampleService _sampleService;

        public GetSampleByIdQueryHandler(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public async Task<OperationResult<SampleDTO>> Handle(GetSampleByIdQuery request, CancellationToken cancellationToken)
        {
            // TODO: Add any orchestration logic needed before/after calling the service.
            return await _sampleService.GetByIdAsync(request.Id);
        }
    }
}
