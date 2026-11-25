using Jackobo.AspNetCore.Framework.Controllers;
using Jackobo.AspNetCore.Framework.Types;
using Jackobo.WebApiTemplate.Core.DTO;
using Jackobo.WebApiTemplate.Core.Handlers.Commands.Sample.Create;
using Jackobo.WebApiTemplate.Core.Handlers.Queries.Sample.GetById;
using Microsoft.AspNetCore.Mvc;

namespace Jackobo.WebApiTemplate.WebAPI.Controllers
{
    /// <summary>
    /// Sample controller demonstrating the end-to-end flow:
    /// Controller -> mediator request (query/command) -> handler -> service -> DTO response.
    ///
    /// Controllers stay thin: they only build the request and call <c>Send(...)</c>.
    /// The response is automatically wrapped in a <see cref="StandardApiResponse{T}"/>.
    ///
    /// TODO: Add authentication/authorization here (e.g. an [Authorize] attribute) once you
    /// configure an authentication scheme for your service.
    ///
    /// TODO: Replace this controller with the endpoints your service exposes.
    /// </summary>
    [Route("api/v1/sample")]
    public class SampleController : ApiControllerBase
    {
        /// <summary>
        /// Returns a single sample item by id.
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [EndpointDescription("Returns a single sample item by its identifier.")]
        public async Task<ActionResult<StandardApiResponse<SampleDTO>>> GetById([FromRoute] int id)
        {
            return await Send(new GetSampleByIdQuery { Id = id });
        }

        /// <summary>
        /// Creates a new sample item.
        /// </summary>
        [HttpPost]
        [EndpointDescription("Creates a new sample item.")]
        public async Task<ActionResult<StandardApiResponse<SampleDTO>>> Create([FromBody] CreateSampleCommand request)
        {
            return await Send(request);
        }
    }
}
