using Jackobo.AspNetCore.Framework.Types;
using Jackobo.WebApiTemplate.Core.DTO;

namespace Jackobo.WebApiTemplate.Core.Abstractions
{
    /// <summary>
    /// Abstraction for the sample service.
    /// Abstractions live in the Core layer so that handlers depend only on interfaces,
    /// while the concrete implementation lives in the Infrastructure layer.
    ///
    /// TODO: Replace this with the abstractions your service needs (repositories,
    /// gateways to external systems, etc.).
    /// </summary>
    public interface ISampleService
    {
        /// <summary>Returns a single sample item by its identifier.</summary>
        Task<OperationResult<SampleDTO>> GetByIdAsync(int id);

        /// <summary>Creates a new sample item and returns it.</summary>
        Task<OperationResult<SampleDTO>> CreateAsync(string name);
    }
}
