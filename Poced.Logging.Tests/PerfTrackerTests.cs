using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Poced.Logging.Tests
{
    [TestFixture]
    public class PerfTrackerTests
    {
        private PerfTracker perfTracker;
        private Mock<IPocedLogger> PocedLogger;

        [SetUp]
        public void BeforeEach()
        {
            PocedLogger = new Mock<IPocedLogger>();
            perfTracker = new PerfTracker(PocedLogger.Object, "name", "userId", "userName", "product", "location", "layer");
        }

        [Test]
        public void WhenStoppingThePerfTracker_TheCorrectDetailsShouldBeLogged()
        {
            LogDetail detail = null;
            PocedLogger.Setup(h => h.WritePerf(It.IsAny<LogDetail>(), It.IsAny<LogLevel>()))
                .Callback<LogDetail, LogLevel>((d, level) => detail = d);
            perfTracker.Stop();

            detail.Should().NotBeNull();
            detail.Message.Should().Be("name");
            detail.UserId.Should().Be("userId");
            detail.UserName.Should().Be("userName");
            detail.Product.Should().Be("product");
            detail.Location.Should().Be("location");
            detail.Layer.Should().Be("layer");
        }

        [Test]
        public void WhenStoppingThePerfTracker_TheTimeShouldBeLogged()
        {
            PocedLogger.Setup(h => h.WritePerf(It.Is<LogDetail>(detail => detail.EllapsedMilliseconds > 0), It.IsAny<LogLevel>()));
            perfTracker.Stop();
            PocedLogger.Verify();
        }
    }
}
