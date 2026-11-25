using Jackobo.AspNetCore.Framework.Configurations;
using Jackobo.AspNetCore.Framework.Mediator;
using Jackobo.AspNetCore.Framework.Types;
using Jackobo.WebApiTemplate.Core;
using Jackobo.WebApiTemplate.Core.DTO;
using Jackobo.WebApiTemplate.Core.Handlers.Queries.Sample.GetById;
using Jackobo.WebApiTemplate.WebAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Jackobo.WebApiTemplate.WebAPI.Tests.Controllers
{
    /// <summary>
    /// Sample unit test for a controller.
    /// Controllers are thin, so the test only checks that the request is dispatched to the
    /// mediator and that the result is wrapped in a <see cref="StandardApiResponse{TData}"/>.
    /// <see cref="ISender"/> is substituted with NSubstitute and resolved from the
    /// <c>HttpContext.RequestServices</c> the base controller reads it from.
    ///
    /// TODO: Use this as the pattern for the tests of your own controllers.
    /// </summary>
    public class SampleControllerTests
    {
        private readonly ISender _mediator = Substitute.For<ISender>();

        private SampleController CreateSut()
        {
            var environmentInfo = Substitute.For<IEnvironmentInfoProvider>();

            var services = new ServiceCollection();
            services.AddSingleton(_mediator);
            services.AddSingleton(environmentInfo);
            services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider()
            };

            return new SampleController
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }

        [Fact]
        public async Task GetById_WhenTheItemExists_ReturnsOkWithTheData()
        {
            var expected = new SampleDTO { Id = 1, Name = "First sample", CreatedOnUtc = DateTime.UtcNow };
            _mediator.Send(Arg.Any<GetSampleByIdQuery>(), Arg.Any<CancellationToken>())
                     .Returns(OperationResult.Ok(expected));

            var sut = CreateSut();

            var actionResult = await sut.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<StandardApiResponse<SampleDTO>>(okResult.Value);
            Assert.False(response.HasError);
            Assert.Same(expected, response.Data);

            await _mediator.Received(1)
                           .Send(Arg.Is<GetSampleByIdQuery>(q => q.Id == 1), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetById_WhenTheHandlerFails_ReturnsTheErrorInTheResponse()
        {
            _mediator.Send(Arg.Any<GetSampleByIdQuery>(), Arg.Any<CancellationToken>())
                     .Returns(ApiErrors.RecordNotFound.WithErrorDetails<SampleDTO>("Record with id 99 was not found"));

            var sut = CreateSut();

            var actionResult = await sut.GetById(99);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<StandardApiResponse<SampleDTO>>(okResult.Value);
            Assert.True(response.HasError);
            Assert.Equal(ApiErrors.RecordNotFound.ErrorCode, response.ErrorCode);
        }
    }
}
