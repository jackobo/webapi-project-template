using Jackobo.AspNetCore.Framework.Configurations;

namespace Jackobo.WebApiTemplate.Infrastructure.ConfigSections
{
    /// <summary>
    /// Strongly typed configuration section bound from appsettings.json.
    /// The <see cref="JackoboConfigSectionAttribute"/> argument is the JSON section path
    /// (use ':' to address nested sections, e.g. "ExternalApi:Sample").
    /// Resolve it with <c>configuration.GetConfigSection&lt;SampleConfigSection&gt;()</c>.
    ///
    /// TODO: Add the configuration sections your service needs and bind them the same way.
    /// </summary>
    [JackoboConfigSection("ExternalApi:Sample")]
    internal class SampleConfigSection
    {
        // TODO: Replace with the settings your service actually requires.
        public string Url { get; set; } = string.Empty;
    }
}
