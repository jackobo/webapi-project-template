using Jackobo.AspNetCore.Framework.Types;

namespace Jackobo.WebApiTemplate.Core
{
    /// <summary>
    /// Application specific error catalog.
    /// These errors are returned through the standard <see cref="OperationResult"/> pipeline
    /// and surfaced to the client in a consistent shape. The type is registered in Program.cs
    /// via <c>options.ApplicationSpecificErrors = typeof(Core.ApiErrors)</c>.
    ///
    /// Every error automatically gets its code prefixed with <see cref="ProjectCode"/> (e.g.
    /// <c>Database.RecordNotFound</c> becomes <c>ApiErrorsPrefixPlaceholder.Database.RecordNotFound</c>),
    /// so callers never need to repeat the project prefix when defining a new error below.
    ///
    /// TODO: Define one entry per business error your service can return.
    /// Convention: pick a unique, descriptive <c>Area.Reason</c> string code per error
    /// (keep them grouped/ordered).
    /// </summary>
    public class ApiErrors : ApplicationError
    {
        private const string ProjectCode = "ApiErrorsPrefixPlaceholder";

        private ApiErrors(string errorCode, string errorDescription)
            : base($"{ProjectCode}.{errorCode}", errorDescription)
        {
        }

        // Sample errors — replace these with the errors relevant to your domain.
        public static readonly ApiErrors RecordNotFound = new ApiErrors("Database.RecordNotFound", "Record not found");
        public static readonly ApiErrors SampleValidationError = new ApiErrors("Sample.ValidationError", "Sample validation error");
    }
}
