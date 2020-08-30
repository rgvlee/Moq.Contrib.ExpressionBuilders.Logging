using System;
using System.Linq;
using AutoFixture;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

// ReSharper disable NUnit.MethodWithParametersAndTestAttribute

namespace Moq.Contrib.ExpressionBuilders.Logging.Tests
{
    [TestFixture(typeof(ILogger))]
    [TestFixture(typeof(ILogger<IFoo>))]
    public class ReadmeExamples<T> where T : class, ILogger
    {
        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        private Fixture _fixture;

        [Test]
        public void Example01()
        {
            var logger = Mock.Of<T>();

            logger.LogError(666, "I am a meat popsicle");

            logger.Verify(Log.With.LogLevel(LogLevel.Error).And.EventId(666).And.LogMessage("I am a meat popsicle"), Times.Once);
        }

        [Test]
        [AutoData]
        public void Example02(int numberOfInvocations)
        {
            var logger = Mock.Of<T>();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(_fixture.Create<EventId>(), _fixture.Create<string>());
            }

            logger.Verify(Log.With.LogLevel(LogLevel.Error), Times.Exactly(numberOfInvocations));
        }

        [Test]
        public void Example03()
        {
            var logger = Mock.Of<T>();

            logger.LogError(666, "I am a meat popsicle");

            logger.Verify(Log.With.LogLevel(x => x == LogLevel.Error || x == LogLevel.Critical)
                    .And.EventId(x => x.Id >= 1 && x.Id <= 1000)
                    .And.LogMessage(x => x.Contains("popsicle")),
                Times.Once);
        }

        [Test]
        public void Example04()
        {
            var loggerMock = new Mock<T>();
            var logger = loggerMock.Object;

            logger.LogError(666, "I am a meat popsicle");

            loggerMock.Verify(Log.With.LogLevel(LogLevel.Error).And.EventId(666).And.LogMessage("I am a meat popsicle"), Times.Once);

            logger.Verify(Log.With.LogLevel(LogLevel.Error).And.EventId(666).And.LogMessage("I am a meat popsicle"), Times.Once);
        }

        [Test]
        public void Example05()
        {
            var exceptionMessage = _fixture.Create<string>();
            var exception = new Exception(exceptionMessage);
            var logger = Mock.Of<T>();

            logger.LogError(666, exception, "I am a meat popsicle");

            logger.Verify(Log.With.ExceptionMessage(exceptionMessage), Times.Once);
        }

        [Test]
        public void Example06()
        {
            var exceptionMessage = "Wow that escalated quickly";
            var firstWord = exceptionMessage.Split().First();
            var thirdWord = exceptionMessage.Split().ElementAt(2);
            var exception = new Exception(exceptionMessage);
            var logger = Mock.Of<T>();

            logger.LogError(666, exception, "I am a meat popsicle");

            logger.Verify(Log.With.ExceptionMessage(x => x.StartsWith(firstWord) && x.Contains(thirdWord)), Times.Once);
        }

        [Test]
        public void Example07()
        {
            var innerExceptionMessage = _fixture.Create<string>();
            var innerException = new Exception(innerExceptionMessage);
            var exceptionMessage = _fixture.Create<string>();
            var exception = new Exception(exceptionMessage, innerException);
            var logger = Mock.Of<T>();

            logger.LogError(666, exception, "I am a meat popsicle");

            logger.Verify(Log.With.Exception(x => x.Message.Equals(exceptionMessage) && x.InnerException.Message.Equals(innerExceptionMessage)), Times.Once);
        }

        [Test]
        [AutoData]
        public void Example08(int numberOfInvocations)
        {
            var logMessage = "I am a meat popsicle";
            var invocationCount = 0;

            var loggerMock = new Mock<ILogger<IFoo>>();
            loggerMock.Setup(Log.With.LogLevel(x => x == LogLevel.Error).And.LogMessage(logMessage)).Callback(() => invocationCount++).Verifiable();
            var logger = loggerMock.Object;

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage);
            }

            Mock.Verify(loggerMock);
            Assert.That(invocationCount, Is.EqualTo(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Example09(int numberOfInvocations)
        {
            var loggerMock = new Mock<T>();
            loggerMock.Setup(Log.With.LogLevel(LogLevel.Error).And.EventId(666).And.LogMessage("I am a meat popsicle"));
            var logger = loggerMock.Object;

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(666, "I am a meat popsicle");
            }

            Mock.VerifyAll(loggerMock);
        }

        [Test]
        [AutoData]
        public void Example10(int numberOfInvocations)
        {
            var logger = Mock.Of<T>();

            logger.LogError("I am a meat popsicle");
            logger.LogCritical("Aziz, light!");

            Assert.Throws<MockException>(() =>
            {
                logger.Verify(Log.With.LogLevel(LogLevel.Error).And.LogLevel(LogLevel.Critical), Times.Exactly(2));
            });

            logger.Verify(Log.With.LogLevel(LogLevel.Error), Times.Exactly(1));
            logger.Verify(Log.With.LogLevel(LogLevel.Critical), Times.Exactly(1));
        }

        [Test]
        [AutoData]
        public void Example11(int numberOfInvocations)
        {
            var logger = Mock.Of<T>();

            logger.LogError("I am a meat popsicle");
            logger.LogCritical("Aziz, light!");

            logger.Verify(Log.With.LogLevel(x => x == LogLevel.Error || x == LogLevel.Critical), Times.Exactly(2));
        }

        [Test]
        [AutoData]
        public void Example12(int numberOfInvocations)
        {
            var key1 = "key1";
            var key2 = "key2";
            var value1 = "value1";
            var value2 = "value2";
            var logMessage = $"The second property {{{key2}}} is first, followed by the first property {{{key1}}}";

            var logger = Mock.Of<T>();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value2, value1);
            }

            logger.Verify(Log.With.LoggedValue(key1, value1).And.LoggedValue(key2, value2), Times.Exactly(numberOfInvocations));
        }
    }
}