using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using Poced.Shared;
using Serilog;

namespace Poced.Logging.Web.Tests
{
    [TestFixture]
    class PocedSerilogWebLogger_Other_Tests
    {
        private Mock<ILogger> errorLogger;
        private Mock<ILogger> usageLogger;
        private Mock<ILogger> diagnosticsLogger;
        private Mock<ILogger> perfLogger;
        private Mock<IOptions<PocedLoggingSettings>> options;
        private Mock<HttpContext> httpContext;
        private Mock<HttpRequest> request;

        private PocedSerilogWebLogger PocedSerilogLogger;

        [SetUp]
        public void BeforeEach()
        {
            var featureCollection = new FeatureCollection();
            var routingFeature = new RoutingFeature { RouteData = new RouteData() };
            featureCollection.Set<IRoutingFeature>(routingFeature);

            httpContext = new Mock<HttpContext>();
            request = new Mock<HttpRequest>();

            httpContext.Setup(h => h.Request).Returns(request.Object);
            httpContext.Setup(h => h.Features).Returns(featureCollection);
            request.Setup(r => r.Headers).Returns(new HeaderDictionary(
                new Dictionary<string, StringValues> { { "Referrer", "localhost" } }));
            request.Setup(r => r.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { { "Key", "Value" } }));

            request.Setup(r => r.Method).Returns("GET");
            errorLogger = new Mock<ILogger>();
            usageLogger = new Mock<ILogger>();
            diagnosticsLogger = new Mock<ILogger>();
            perfLogger = new Mock<ILogger>();

            options = new Mock<IOptions<PocedLoggingSettings>>();
            options.Setup(o => o.Value).Returns(new PocedLoggingSettings { WriteDiagnostics = true });

            PocedSerilogLogger = new PocedSerilogWebLogger(perfLogger.Object, usageLogger.Object, errorLogger.Object, diagnosticsLogger.Object, options.Object);
        }

        [Test]
        public void WhenAskingForTheLocationFromAnApi_ShouldReturnTheLocation()
        {
            PocedSerilogLogger.GetLocationAndDataForApiCall(httpContext.Object, "", new Dictionary<string, object>(), out string location);
            location.Should().StartWith("GET");
        }

        [Test]
        public void WhenAskingForTheLocationFromAnApi_ShouldReturnTheQueryData()
        {
            var data = new Dictionary<string, object>();
            PocedSerilogLogger.GetLocationAndDataForApiCall(httpContext.Object, "", data, out string _);

            data.Count.Should().Be(2);
            data["q-0-Key"].Should().Be("Value");
        }
    }
}
