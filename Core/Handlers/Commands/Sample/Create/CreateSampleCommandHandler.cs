using Jackobo.AspNetCore.Framework.Mediator;
using Jackobo.AspNetCore.Framework.Types;
using Jackobo.WebApiTemplate.Core.Abstractions;
using Jackobo.WebApiTemplate.Core.DTO;

namespace Jackobo.WebApiTemplate.Core.Handlers.Commands.Sample.Create
{
    /// <summary>
    /// Handler for <see cref="CreateSampleCommand"/>.
    /// </summary>
    internal class CreateSampleCommandHandler : IStandardRequestHandler<CreateSampleCommand, SampleDTO>
    {
        private readonly ISampleService _sampleService;

        public CreateSampleCommandHandler(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public async Task<OperationResult<SampleDTO>> Handle(CreateSampleCommand request, CancellationToken cancellationToken)
        {
            // TODO: Add any orchestration logic needed before/after calling the service.
            return await _sampleService.CreateAsync(request.Name);
        }
    }
}
