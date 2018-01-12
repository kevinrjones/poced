using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Poced.Shared;

namespace Poced.Logging.Web.Tests
{

    [TestFixture]
    public class ApiLoggerAttributeTests
    {
        private ApiLoggerAttribute apiLoggerAttribute;

        private Mock<IOptions<PocedLoggingSettings>> options;
        private Mock<HttpContext> httpContext;
        private Mock<HttpRequest> request;

        private Mock<IPocedWebLogger> PocedSerilogLogger;
        private readonly Dictionary<object, object> items = new Dictionary<object, object>();
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
                new ActionContext(httpContext.Object, new RouteData(), new ActionDescriptor { AttributeRouteInfo = new AttributeRouteInfo { Template = "a template" } }),
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
        public void WhenExecutingTheAction_ThenShouldStoreThePerfTrackerInTheContext()
        {
            CreateSut();
            apiLoggerAttribute.OnActionExecuting(actionExecutingContext);
            items.Should().NotBeEmpty();
            items["PerfTracker"].Should().BeOfType<PerfTracker>();
        }

        [Test]
        public void WhenExecutingTheAction_ThenShouldCallGetLocationAndDataForApiCall()
        {
            CreateSut();
            // ReSharper disable once NotAccessedVariable
            string location;
            apiLoggerAttribute.OnActionExecuting(actionExecutingContext);
            PocedSerilogLogger.Verify(s => s.GetLocationAndDataForApiCall(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), out location), Times.Once());
        }

        [Test]
        public void WhenExecutingTheAction_ThenShouldCallGetUserData()
        {
            // ReSharper disable once NotAccessedVariable
            string userName;
            // ReSharper disable once NotAccessedVariable
            string userId;
            CreateSut();
            apiLoggerAttribute.OnActionExecuting(actionExecutingContext);
            PocedSerilogLogger.Verify(s => s.GetUserData(It.IsAny<Dictionary<string, object>>(), It.IsAny<ClaimsPrincipal>(), out userId, out userName), Times.Once());
        }

        private void CreateSut()
        {
            apiLoggerAttribute = new ApiLoggerAttribute(options?.Object, PocedSerilogLogger?.Object);
        }
    }
}