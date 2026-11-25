using Jackobo.WebApiTemplate.Core;
using Jackobo.WebApiTemplate.Infrastructure.Services.Sample;
using Microsoft.Extensions.Logging;

namespace Jackobo.WebApiTemplate.Infrastructure.Tests.Services
{
    /// <summary>
    /// Sample unit test for an Infrastructure service.
    /// The framework/platform dependencies (here <see cref="ILogger{TCategoryName}"/>) are
    /// substituted with NSubstitute so the test exercises only the service behaviour.
    ///
    /// TODO: Use this as the pattern for the tests of your own services (substitute the
    /// DbContext wrapper, the HTTP gateway, etc.).
    /// </summary>
    public class SampleServiceTests
    {
        private readonly ILogger<SampleService> _logger = Substitute.For<ILogger<SampleService>>();

        [Fact]
        public async Task CreateAsync_ThenGetByIdAsync_ReturnsTheCreatedItem()
        {
            var sut = new SampleService(_logger);

            var created = await sut.CreateAsync("Created by a unit test");

            Assert.True(created.IsSuccess);

            var fetched = await sut.GetByIdAsync(created.Value.Id);

            Assert.True(fetched.IsSuccess);
            Assert.Equal("Created by a unit test", fetched.Value.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTheItemDoesNotExist_ReturnsRecordNotFound()
        {
            var sut = new SampleService(_logger);

            var result = await sut.GetByIdAsync(int.MaxValue);

            Assert.True(result.IsFailure);
            Assert.Equal(ApiErrors.RecordNotFound.ErrorCode, result.Error.ErrorCode);
        }
    }
}
