using System;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Poced.Logging;
using Poced.Shared;
using Serilog;
using Serilog.Events;

namespace Diligent.Blueprint.Logging.Tests
{
    [TestFixture]
    public class PocedSerilogLoggerTests
    {
        private Mock<ILogger> errorLogger;
        private Mock<ILogger> usageLogger;
        private Mock<ILogger> diagnosticsLogger;
        private Mock<ILogger> perfLogger;
        private Mock<IOptions<PocedLoggingSettings>> options;

        private PocedSerilogLogger PocedSerilogLogger;

        [SetUp]
        public void BeforeEach()
        {
            errorLogger = new Mock<ILogger>();
            usageLogger = new Mock<ILogger>();
            diagnosticsLogger = new Mock<ILogger>();
            perfLogger = new Mock<ILogger>();
            options = new Mock<IOptions<PocedLoggingSettings>>();
            options.Setup(o => o.Value).Returns(new PocedLoggingSettings());

            PocedSerilogLogger = new PocedSerilogLogger(perfLogger.Object, usageLogger.Object, errorLogger.Object, diagnosticsLogger.Object, options.Object);
        }

        [Test]
        public void WhenWritePerfIsCalled_ThenThePerfLoggerShouldBeUsed()
        {
            perfLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()));
            PocedSerilogLogger.WritePerf(new LogDetail());
            perfLogger.Verify(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()), Times.Once());
        }

        [Test]
        public void WhenWriteUsageIsCalled_ThenTheUsageLoggerShouldBeUsed()
        {
            usageLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()));
            PocedSerilogLogger.WriteUsage(new LogDetail());
            usageLogger.Verify(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()), Times.Once());
        }

        [Test]
        public void WhenWriteDiagnosticIsCalled_And_DiagnosticsIsDisabled_ThenTheDiagnosticLoggerShouldNotBeUsed()
        {
            diagnosticsLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()));
            PocedSerilogLogger.WriteDiagnostic(new LogDetail());
            diagnosticsLogger.Verify(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()), Times.Never);
        }

        [Test]
        public void WhenWriteDiagnosticIsCalled_And_DiagnosticsIsEnabled_ThenTheDiagnosticLoggerShouldBeUsed()
        {

            options = new Mock<IOptions<PocedLoggingSettings>>();
            options.Setup(o => o.Value).Returns(new PocedLoggingSettings{WriteDiagnostics = true});

            PocedSerilogLogger = new PocedSerilogLogger(perfLogger.Object, usageLogger.Object, errorLogger.Object, diagnosticsLogger.Object, options.Object);


            diagnosticsLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()));
            PocedSerilogLogger.WriteDiagnostic(new LogDetail());
            diagnosticsLogger.Verify(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()), Times.Once);
        }

        [Test]
        public void WhenWriteErrorIsCalled_ThenTheErrorLoggerShouldBeUsed()
        {
            errorLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()));
            PocedSerilogLogger.WriteError(new LogDetail());
            errorLogger.Verify(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()), Times.Once());
        }

        [Test]
        public void WhenWriteErrorIsCalled_AndThereIsAnException_ThenTheErrorMessageShouldBeSetFromTheException()
        {
            Exception e = new Exception("A Message");
            LogDetail detail = null;

            errorLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()))
                .Callback<LogEventLevel, string, LogDetail>((l, s, d) =>
                {
                    detail = d;
                });
            PocedSerilogLogger.WriteError(new LogDetail{Exception = e});
            detail.Message.Should().Be("A Message");
        }

        [Test]
        public void WhenWriteErrorIsCalled_AndThereIsANestedException_ThenTheErrorMessageShouldBeSetFromTheNestedException()
        {
            Exception e = new Exception("A Message",
                new Exception("A Nested Message")
                );
            LogDetail detail = null;

            errorLogger.Setup(l => l.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>(), It.IsAny<LogDetail>()))
                .Callback<LogEventLevel, string, LogDetail>((l, s, d) =>
                {
                    detail = d;
                });
            PocedSerilogLogger.WriteError(new LogDetail { Exception = e });
            detail.Message.Should().Be("A Nested Message");
        }
    }
}