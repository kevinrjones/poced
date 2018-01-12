using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Poced.Shared;

namespace Poced.Logging.Web.Tests
{
    [TestFixture]
    public class PocedExceptionFilterTests
    {
        private PocedExceptionFilter PocedExceptionFilter;
        private Mock<IOptions<PocedLoggingSettings>> options;
        private Mock<HttpContext> httpContext;
        private Mock<HttpRequest> request;
        private Mock<HttpResponse> response;
        private Mock<Stream> body;
        private string errorMessage;

        private Mock<IPocedWebLogger> PocedSerilogLogger;

        private ExceptionContext exceptionContext;

        [SetUp]
        public void BeforeEach()
        {
            body = new Mock<Stream>();
            httpContext = new Mock<HttpContext>();
            request = new Mock<HttpRequest>();
            response = new Mock<HttpResponse>();
            httpContext.Setup(h => h.Request).Returns(request.Object);
            httpContext.Setup(h => h.Response).Returns(response.Object);
            body.Setup(b => b.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Callback<byte[], int, int, CancellationToken>((b, o, c, t) => errorMessage = System.Text.Encoding.UTF8.GetString(b));

            response.Setup(r => r.Body).Returns(body.Object);

            options = new Mock<IOptions<PocedLoggingSettings>>();
            options.Setup(o => o.Value).Returns(new PocedLoggingSettings());

            PocedSerilogLogger = new Mock<IPocedWebLogger>();

            exceptionContext = new ExceptionContext(
                new ActionContext(httpContext.Object, new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>());

            exceptionContext.Exception = new Exception();

            PocedExceptionFilter = new PocedExceptionFilter(options.Object, PocedSerilogLogger.Object);
        }
        [Test]
        public void WhenLoggerIsNull_ThenShouldThrowException()
        {
            PocedSerilogLogger = null;
            Action a = CreateSut;
            a.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WhenOptionsIsNull_ThenShouldThrowException()
        {
            options = null;
            Action a = CreateSut;
            a.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WhenExecuted_ShouldWriteToTheErrorLog()
        {
            PocedExceptionFilter.OnException(exceptionContext);

            PocedSerilogLogger.Verify(l => l.WriteError(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<LogLevel>()), Times.Once);
            errorMessage.Should().StartWith("Something went wrong");
        }

        [Test]
        public void WhenExecutedWithAnAuthorizationError_ShouldErrorMessageShouldContainTheCorrectText()
        {
            exceptionContext.Exception = new UnauthorizedAccessException();
            PocedExceptionFilter.OnException(exceptionContext);

            PocedSerilogLogger.Verify(l => l.WriteError(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<LogLevel>()), Times.Once);
            errorMessage.Should().Contain("Unauthorized Access");
        }

        [Test]
        public void WhenExecutedWithANotImplementedException_ShouldErrorMessageShouldContainTheCorrectText()
        {
            exceptionContext.Exception = new NotImplementedException();
            PocedExceptionFilter.OnException(exceptionContext);

            PocedSerilogLogger.Verify(l => l.WriteError(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<LogLevel>()), Times.Once);
            errorMessage.Should().Contain("A server error occurred.");
        }

        [Test]
        public void WhenExecutedWithAPocedException_ShouldErrorMessageShouldContainTheCorrectText()
        {
            exceptionContext.Exception = new PocedException("A Poced Message");
            PocedExceptionFilter.OnException(exceptionContext);

            PocedSerilogLogger.Verify(l => l.WriteError(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<LogLevel>()), Times.Once);
            errorMessage.Should().Contain("A Poced Message");
        }

        [Test]
        public void WhenExecutedWithAnyNonSpecifiedException_ShouldErrorMessageShouldContainTheCorrectText()
        {
            exceptionContext.Exception = new Exception("A Different Message");
            PocedExceptionFilter.OnException(exceptionContext);

            PocedSerilogLogger.Verify(l => l.WriteError(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<LogLevel>()), Times.Once);
            errorMessage.Should().Contain("A Different Message");
        }

        private void CreateSut()
        {
            // ReSharper disable once NotAccessedVariable
            string userId;
            // ReSharper disable once NotAccessedVariable
            string userName;
            // ReSharper disable once NotAccessedVariable
            string location;

            PocedSerilogLogger?.Setup(s => s.GetWebLoggingData(It.IsAny<HttpContext>(), out userId, out userName, out location)).Returns(new Dictionary<string, object>());
            PocedExceptionFilter = new PocedExceptionFilter(options?.Object, PocedSerilogLogger?.Object);
        }

    }
}
