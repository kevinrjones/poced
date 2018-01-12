using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Poced.Shared;

namespace Poced.Logging.Web.Tests
{
    class TrackUsageAttributeTests
    {
        [TestFixture]
        public class TrackApiUsageAttributeTests
        {
            private TrackUsageAttribute trackUsageAttribute;

            private Mock<IOptions<PocedLoggingSettings>> options;
            private Mock<HttpContext> httpContext;
            private Mock<HttpRequest> request;

            private Mock<IPocedWebLogger> PocedSerilogLogger;
            private readonly Dictionary<object, object> items = new Dictionary<object, object>();
            private ResultExecutedContext resultExecutedContext;

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

                resultExecutedContext = new ResultExecutedContext(
                    new ActionContext(httpContext.Object, new RouteData(), 
                    new ControllerActionDescriptor
                    {
                        AttributeRouteInfo = new AttributeRouteInfo { Template = "a template" },
                        ControllerName = "controller",
                        ActionName = "action"
                    }),
                    new List<IFilterMetadata>(),
                    new ContentResult(),
                    new object());
            }


            [Test]
            public void WhenLoggerIsNull_ThenShouldThrowException()
            {
                PocedSerilogLogger = null;
                Action a = () => CreateSut();
                a.ShouldThrow<ArgumentNullException>();
            }

            [Test]
            public void WhenOptionsIsNull_ThenShouldThrowException()
            {
                options = null;
                Action a = () => CreateSut();
                a.ShouldThrow<ArgumentNullException>();
            }

            [Test]
            public void AfterExecutingTheResult_ThenShouldCallWriteUsage()
            {
                CreateSut();
                trackUsageAttribute.OnResultExecuted(resultExecutedContext);
                PocedSerilogLogger.Verify(s => s.WriteUsage(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(), "Name", It.IsAny<Dictionary<string, object>>(), It.IsAny<LogLevel>()), Times.Once());
            }

            [Test]
            public void AfterExecutingTheResult_ThenShouldSetTheName()
            {
                CreateSut("");
                trackUsageAttribute.OnResultExecuted(resultExecutedContext);
                PocedSerilogLogger.Verify(s => s.WriteUsage(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(), "controller_action_GET", It.IsAny<Dictionary<string, object>>(), It.IsAny<LogLevel>()), Times.Once());
            }

            private void CreateSut(string name = "Name")
            {
                trackUsageAttribute = new TrackUsageAttribute(options?.Object, PocedSerilogLogger?.Object, name);
            }

        }
    }
}
