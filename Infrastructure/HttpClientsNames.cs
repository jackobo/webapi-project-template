namespace Jackobo.WebApiTemplate.Infrastructure
{
    /// <summary>
    /// Logical names for the named HttpClients registered in
    /// <see cref="InfrastructureDependencyInjection"/>. Using constants avoids magic strings
    /// when resolving clients from <c>IHttpClientFactory</c>.
    ///
    /// TODO: Add one constant per external HTTP dependency your service calls.
    /// </summary>
    public static class HttpClientsNames
    {
        public const string SampleExternalApi = nameof(SampleExternalApi);
    }
}
