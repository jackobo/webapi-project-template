using Jackobo.AspNetCore.Framework.Mediator;
using Jackobo.WebApiTemplate.Core.DTO;
using FluentValidation;

namespace Jackobo.WebApiTemplate.Core.Handlers.Commands.Sample.Create
{
    /// <summary>
    /// Command that creates a new sample item.
    /// Commands represent write operations and also implement
    /// <see cref="IStandardRequest{TResponse}"/>. By convention, the request properties carry
    /// the input from the client (bound from the request body/route in the controller).
    ///
    /// TODO: Model your own write operations as commands like this one.
    /// </summary>
    public class CreateSampleCommand : IStandardRequest<SampleDTO>
    {
        public string Name { get; set; } = string.Empty;
    }

    internal class CreateSampleCommandValidator : AbstractValidator<CreateSampleCommand>
    {
        public CreateSampleCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }
}
