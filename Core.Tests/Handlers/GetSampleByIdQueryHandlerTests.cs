using Jackobo.AspNetCore.Framework.Types;
using Jackobo.WebApiTemplate.Core.Abstractions;
using Jackobo.WebApiTemplate.Core.DTO;
using Jackobo.WebApiTemplate.Core.Handlers.Queries.Sample.GetById;

namespace Jackobo.WebApiTemplate.Core.Tests.Handlers
{
    /// <summary>
    /// Sample unit test for a mediator handler.
    /// The handler is isolated from its dependencies with NSubstitute: <see cref="ISampleService"/>
    /// is substituted, so only the handler logic is exercised.
    ///
    /// TODO: Use this as the pattern for the tests of your own handlers.
    /// </summary>
    public class GetSampleByIdQueryHandlerTests
    {
        private readonly ISampleService _sampleService = Substitute.For<ISampleService>();

        [Fact]
        public async Task Handle_WhenServiceReturnsItem_ReturnsSuccessWithTheItem()
        {
            var expected = new SampleDTO { Id = 7, Name = "Sample", CreatedOnUtc = DateTime.UtcNow };
            _sampleService.GetByIdAsync(7).Returns(expected);

            var sut = new GetSampleByIdQueryHandler(_sampleService);

            var result = await sut.Handle(new GetSampleByIdQuery { Id = 7 }, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Same(expected, result.Value);
            await _sampleService.Received(1).GetByIdAsync(7);
        }

        [Fact]
        public async Task Handle_WhenServiceFails_PropagatesTheError()
        {
            _sampleService.GetByIdAsync(42)
                .Returns(ApiErrors.RecordNotFound.WithErrorDetails<SampleDTO>("Record with id 42 was not found"));

            var sut = new GetSampleByIdQueryHandler(_sampleService);

            var result = await sut.Handle(new GetSampleByIdQuery { Id = 42 }, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ApiErrors.RecordNotFound.ErrorCode, result.Error.ErrorCode);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        [InlineData(1, true)]
        public void Validator_EnforcesAPositiveId(int id, bool expectedIsValid)
        {
            var validator = new GetSampleByIdQueryValidator();

            var result = validator.Validate(new GetSampleByIdQuery { Id = id });

            Assert.Equal(expectedIsValid, result.IsValid);
        }
    }
}
