using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using Poced.Shared;
using Serilog;
using Serilog.Events;

namespace Poced.Logging.Web.Tests
{
    [TestFixture]
    public class PocedSerilogWebLogger_WriteDiagnostic_Tests
    {
        private Mock<ILogger> errorLogger;
        private Mock<ILogger> usageLogger;
        private Mock<ILogger> diagnosticsLogger;
        private Mock<ILogger> perfLogger;
        private Mock<IOptions<PocedLoggingSettings>> options;
        private Mock<HttpContext> httpContext;

        private PocedSerilogWebLogger PocedSerilogLogger;

        [SetUp]
        public void BeforeEach()
        {
            httpContext = new Mock<HttpContext>();
            errorLogger = new Mock<ILogger>();
            usageLogger = new Mock<ILogger>();
            diagnosticsLogger = new Mock<ILogger>();
            perfLogger = new Mock<ILogger>();
            options = new Mock<IOptions<PocedLoggingSettings>>();
            options.Setup(o => o.Value).Returns(new PocedLoggingSettings{WriteDiagnostics = true});

            PocedSerilogLogger = new PocedSerilogWebLogger(perfLogger.Object, usageLogger.Object, errorLogger.Object, diagnosticsLogger.Object, options.Object);
        }

        [Test]
        public void WhenWriteDiagnosticsIsCalled_ThenTheLocationShouldBeSetFromTheRequestData()
        {
            LogDetail logdetail = null;

            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.Setup(r => r.Headers).Returns(new HeaderDictionary());
            request.Setup(r => r.Path).Returns("/location");

            httpContext.Setup(c => c.Request).Returns(request.Object);

            diagnosticsLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()))
                .Callback<LogEventLevel, string, LogDetail>((l, s, d) => logdetail = d);
            PocedSerilogLogger.WriteDiagnostic(httpContext.Object, "product", "layer", "activityName");

            logdetail.Location.Should().Be("/location");
        }

        [Test]
        public void WhenWriteDiagnosticsIsCalled_ThenTheLocationShouldNotBeSetIfThereIsNoRequestData()
        {
            LogDetail logdetail = null;

            diagnosticsLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()))
                .Callback<LogEventLevel, string, LogDetail>((l, s, d) => logdetail = d);
            PocedSerilogLogger.WriteDiagnostic(httpContext.Object, "product", "layer", "activityName");

            logdetail.Location.Should().Be("unknown");
        }

        [Test]
        public void WhenWriteDiagnosticsIsCalled_ThenTheUserClaimDataShouldBeSetFromTheRequestData()
        {
            LogDetail logdetail = null;
            ClaimsPrincipal.ClaimsPrincipalSelector = () => new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Name, "UserName"),
                    new Claim(ClaimTypes.NameIdentifier, "UserId")
                }));

            Mock<HttpRequest> request = new Mock<HttpRequest>();

            request.Setup(r => r.Headers).Returns(new HeaderDictionary());


            httpContext.Setup(c => c.Request).Returns(request.Object);

            diagnosticsLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()))
                .Callback<LogEventLevel, string, LogDetail>((l, s, d) => logdetail = d);
            PocedSerilogLogger.WriteDiagnostic(httpContext.Object, "product", "layer", "activityName");

            logdetail.UserName.Should().Be("UserName");
            logdetail.UserId.Should().Be("UserId");
        }

        [Test]
        public void WhenWriteDiagnosticsIsCalled_ThenTheOtherClaimDataShouldBeSetFromTheRequestData()
        {
            LogDetail logdetail = null;
            ClaimsPrincipal.ClaimsPrincipalSelector = () => new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Authentication, "authentication"),
                }));

            Mock<HttpRequest> request = new Mock<HttpRequest>();

            request.Setup(r => r.Headers).Returns(new HeaderDictionary());


            httpContext.Setup(c => c.Request).Returns(request.Object);

            diagnosticsLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()))
                .Callback<LogEventLevel, string, LogDetail>((l, s, d) => logdetail = d);
            PocedSerilogLogger.WriteDiagnostic(httpContext.Object, "product", "layer", "activityName");

            logdetail.AdditionalInfo.Should().NotBeEmpty();
            logdetail.AdditionalInfo[$"UserClaim-1-{ClaimTypes.Authentication}"].Should().Be("authentication");
        }

        [Test]
        public void WhenWriteDiagnosticsIsCalled_ThenTheBrowserShouldBeSetFromTheRequestData()
        {
            LogDetail logdetail = null;

            Mock<HttpRequest> request = new Mock<HttpRequest>();

            request.Setup(r => r.Headers).Returns(new HeaderDictionary(
                new Dictionary<string, StringValues> { { "User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36" } }));
            request.Setup(r => r.Query).Returns(new QueryCollection());
            httpContext.Setup(c => c.Request).Returns(request.Object);

            diagnosticsLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()))
                .Callback<LogEventLevel, string, LogDetail>((l, s, d) => logdetail = d);
            PocedSerilogLogger.WriteDiagnostic(httpContext.Object, "product", "layer", "activityName");

            logdetail.AdditionalInfo.Should().NotBeEmpty();
            logdetail.AdditionalInfo["Browser"].ToString().Should().StartWith("Chrome");
        }

        [Test]
        public void WhenWriteUsageIsCalled_ThenTheQueryStringShouldBeSetFromTheRequestData()
        {
            LogDetail logdetail = null;

            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.Setup(r => r.Headers).Returns(new HeaderDictionary(
                new Dictionary<string, StringValues> { { "User-Agent", "Some User Agent" } }));

            request.Setup(r => r.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { { "Key", "Value" } }));
            httpContext.Setup(c => c.Request).Returns(request.Object);

            diagnosticsLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()))
                .Callback<LogEventLevel, string, LogDetail>((l, s, d) => logdetail = d);
            PocedSerilogLogger.WriteDiagnostic(httpContext.Object, "product", "layer", "activityName");

            logdetail.AdditionalInfo.Should().NotBeEmpty();
            logdetail.AdditionalInfo["QueryString-Key"].Should().Be("Value");
        }
    }
}