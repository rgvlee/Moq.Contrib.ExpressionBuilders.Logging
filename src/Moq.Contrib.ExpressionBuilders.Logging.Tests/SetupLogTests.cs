using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

// ReSharper disable NUnit.MethodWithParametersAndTestAttribute

namespace Moq.Contrib.ExpressionBuilders.Logging.Tests
{
    [TestFixture(typeof(ILogger))]
    [TestFixture(typeof(ILogger<IFoo>))]
    public class SetupLogTests<T> where T : class, ILogger
    {
        private static T CreateMockedLogger()
        {
            return Mock.Of<T>();
        }

        [Test]
        [AutoData]
        public void SetupThenVerify_InvocationMatchesSetUp_VerifyDoesNotThrow(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = "value1";
            var logMessage = $"This is a message with a single property {{{key1}}}";

            var logger = CreateMockedLogger();
            var invocationCount = 0;
            Mock.Get(logger)
                .Setup(Log.With.LogLevel(LogLevel.Error).And.LogMessage(x => x.StartsWith(logMessage.Substring(0, 10))).And.LoggedValue(key1, value1))
                .Callback(() => invocationCount++)
                .Verifiable();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            Mock.Verify(Mock.Get(logger));
            Assert.That(invocationCount, Is.EqualTo(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void SetupThenVerify_InvocationThatDoesNotMatchSetUp_Throws(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = "value1";
            var logMessage = $"This is a message with a single property {{{key1}}}";

            var logger = CreateMockedLogger();
            var invocationCount = 0;
            Mock.Get(logger)
                .Setup(Log.With.LogLevel(x => x == LogLevel.Critical).And.LogMessage(logMessage).And.LoggedValue(key1, value1))
                .Callback(() => invocationCount++)
                .Verifiable();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            Assert.Throws<MockException>(() =>
            {
                Mock.Verify(Mock.Get(logger));
            });

            Assert.That(invocationCount, Is.EqualTo(0));
            Assert.That(invocationCount, Is.Not.EqualTo(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void SetupThenVerifyAll_InvocationMatchesSetUp_VerifyDoesNotThrow(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = "value1";
            var logMessage = $"This is a message with a single property {{{key1}}}";

            var logger = CreateMockedLogger();
            var invocationCount = 0;
            Mock.Get(logger)
                .Setup(Log.With.LogLevel(LogLevel.Error).And.LogMessage(x => x.StartsWith(logMessage.Substring(0, 10))).And.LoggedValue(key1, value1))
                .Callback(() => invocationCount++);

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            Mock.VerifyAll(Mock.Get(logger));
            Assert.That(invocationCount, Is.EqualTo(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void SetupThenVerifyAll_InvocationThatDoesNotMatchSetUp_Throws(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = "value1";
            var logMessage = $"This is a message with a single property {{{key1}}}";

            var logger = CreateMockedLogger();
            var invocationCount = 0;
            Mock.Get(logger).Setup(Log.With.LogLevel(x => x == LogLevel.Critical).And.LogMessage(logMessage).And.LoggedValue(key1, value1)).Callback(() => invocationCount++);

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            Assert.Throws<MockException>(() =>
            {
                Mock.VerifyAll(Mock.Get(logger));
            });

            Assert.That(invocationCount, Is.EqualTo(0));
            Assert.That(invocationCount, Is.Not.EqualTo(numberOfInvocations));
        }
    }
}