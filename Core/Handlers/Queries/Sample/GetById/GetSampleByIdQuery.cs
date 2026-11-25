using Jackobo.AspNetCore.Framework.Mediator;
using Jackobo.WebApiTemplate.Core.DTO;
using FluentValidation;

namespace Jackobo.WebApiTemplate.Core.Handlers.Queries.Sample.GetById
{
    /// <summary>
    /// Query that asks for a single sample item by id.
    /// A query implements <see cref="IStandardRequest{TResponse}"/> where TResponse is the
    /// payload returned to the caller. It is dispatched from the controller via <c>Send(...)</c>.
    ///
    /// TODO: Model your own read operations as queries like this one.
    /// </summary>
    public class GetSampleByIdQuery : IStandardRequest<SampleDTO>
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// Optional FluentValidation validator. When present, the framework runs it automatically
    /// before the handler executes and short-circuits with a validation error if it fails.
    /// </summary>
    internal class GetSampleByIdQueryValidator : AbstractValidator<GetSampleByIdQuery>
    {
        public GetSampleByIdQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
