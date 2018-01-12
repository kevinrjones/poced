using System;
using System.Collections.Generic;
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
    public class TrackPerformanceAttributeTests
    {
        private TrackPerformanceAttribute trackPerformanceAttribute;

        private Mock<IOptions<PocedLoggingSettings>> options;
        private Mock<HttpContext> httpContext;
        private Mock<HttpRequest> request;

        private Mock<IPocedWebLogger> PocedSerilogLogger;
        private Dictionary<object, object> items = new Dictionary<object, object>();
        private ActionExecutingContext actionExecutingContext;

        [SetUp]
        public void BeforeEach()
        {
            httpContext = new Mock<HttpContext>();
            request = new Mock<HttpRequest>();
            httpContext.Setup(h => h.Request).Returns(request.Object);
            httpContext.Setup(h => h.Items).Returns(items);
            request.Setup(r => r.Method).Returns("GET");

            options = new Mock<IOptions<PocedLoggingSettings>>();
            options.Setup(o => o.Value).Returns(new PocedLoggingSettings());

            PocedSerilogLogger = new Mock<IPocedWebLogger>();

            actionExecutingContext = new ActionExecutingContext(
                new ActionContext(httpContext.Object, new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(), new object());
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
        public void WhenExecutingTheAction_ThenShouldGetWebLoggingData()
        {
            CreateSut();
            // ReSharper disable once NotAccessedVariable
            string userId;
            // ReSharper disable once NotAccessedVariable
            string userName;
            // ReSharper disable once NotAccessedVariable
            string location;

            trackPerformanceAttribute.OnActionExecuting(actionExecutingContext);

            PocedSerilogLogger.Verify(s => s.GetWebLoggingData(It.IsAny<HttpContext>(), out userId, out userName, out location), Times.Once);
        }

        [Test]
        public void WhenExecutingTheAction_ThenShouldStoreThePerfTrackerInTheContext()
        {
            CreateSut();
            trackPerformanceAttribute.OnActionExecuting(actionExecutingContext);
            items.Should().NotBeEmpty();
            items["Stopwatch"].Should().BeOfType<PerfTracker>();
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
            trackPerformanceAttribute = new TrackPerformanceAttribute(options?.Object, PocedSerilogLogger?.Object);
        }
    }
}
