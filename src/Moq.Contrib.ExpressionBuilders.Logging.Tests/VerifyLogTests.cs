using System;
using System.Linq;
using AutoFixture;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Testing.Common;

// ReSharper disable NUnit.MethodWithParametersAndTestAttribute

namespace Moq.Contrib.ExpressionBuilders.Logging.Tests
{
    [TestFixture(typeof(ILogger))]
    [TestFixture(typeof(ILogger<IFoo>))]
    public class VerifyLogTests<T> where T : class, ILogger
    {
        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        private Fixture _fixture;

        private static T CreateMockedLogger()
        {
            return Mock.Of<T>();
        }

        [Test]
        [AutoData]
        public void Verify_EventIdExpressionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations, EventId eventId)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(eventId, _fixture.Create<string>());
            }

            logger.Verify(Log.With.EventId(x => x.Id >= eventId.Id && x.Id <= eventId.Id), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_EventIdThatDoesNotMatchInvocations_Throws(int numberOfInvocations)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(_fixture.Create<EventId>(), _fixture.Create<string>());
            }

            Assert.Throws<MockException>(() =>
            {
                logger.Verify(Log.With.EventId(_fixture.Create<EventId>()), Times.Exactly(numberOfInvocations));
            });
        }

        [Test]
        [AutoData]
        public void Verify_EventIdThatMatchesInvocations_DoesNotThrow(int numberOfInvocations, EventId eventId)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(eventId, _fixture.Create<string>());
            }

            logger.Verify(Log.With.EventId(eventId), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ExceptionExpressionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var innerException = new Exception(_fixture.Create<string>());
            var exception = new Exception(_fixture.Create<string>(), innerException);

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(exception, _fixture.Create<string>());
            }

            logger.Verify(Log.With.Exception(x => x.InnerException.Message.Equals(innerException.Message)), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ExceptionMessageExpressionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var exception = new Exception(_fixture.Create<string>());

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(exception, _fixture.Create<string>());
            }

            logger.Verify(Log.With.ExceptionMessage(x => x.Select(y => y).SequenceEqual(exception.Message.Select(y => y))), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ExceptionMessageThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var exception = new Exception(_fixture.Create<string>());

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(exception, _fixture.Create<string>());
            }

            logger.Verify(Log.With.ExceptionMessage(exception.Message), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ExceptionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var exception = new Exception(_fixture.Create<string>());

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(exception, _fixture.Create<string>());
            }

            logger.Verify(Log.With.Exception(exception), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_IntEventIdExpressionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations, int eventId)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(eventId, _fixture.Create<string>());
            }

            logger.Verify(Log.With.EventId(eventId), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_IntEventIdThatDoesNotMatchInvocations_Throws(int numberOfInvocations)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(_fixture.Create<int>(), _fixture.Create<string>());
            }

            Assert.Throws<MockException>(() =>
            {
                logger.Verify(Log.With.EventId(_fixture.Create<int>()), Times.Exactly(numberOfInvocations));
            });
        }

        [Test]
        [AutoData]
        public void Verify_IntEventIdThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var eventId = 5;

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(eventId, _fixture.Create<string>());
            }

            logger.Verify(Log.With.EventId(x => x.Id >= 1 && x.Id <= 10), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_LoggedValueExpressionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = "value1";
            var logMessage = $"This is a message with a single property {{{key1}}}";

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            logger.Verify(Log.With.LoggedValue(x => x.Key.Equals(key1) && x.Value.Equals(value1)), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_LoggedValuesThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var key1 = "key1";
            var key2 = "key2";
            var value1 = "value1";
            var value2 = "value2";
            var logMessage = $"The second property {{{key2}}} is first, followed by the first property {{{key1}}}";

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value2, value1);
            }

            logger.Verify(Log.With.LoggedValue(key1, value1).And.LoggedValue(key2, value2), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_LoggedValueThatDoesNotMatchInvocations_Throws(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = "value1";
            var logMessage = $"This is a message with a single property {{{key1}}}";

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            Assert.Throws<MockException>(() =>
            {
                logger.Verify(Log.With.LoggedValue(_fixture.Create<string>(), _fixture.Create<object>()), Times.Exactly(numberOfInvocations));
            });
        }

        [Test]
        [AutoData]
        public void Verify_LoggedValueThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = "value1";
            var logMessage = $"This is a message with a single property {{{key1}}}";

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            logger.Verify(Log.With.LoggedValue(key1, value1), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_LogLevelAndLogMessageExpressionAndLoggedValueThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = "value1";
            var logMessage = $"This is a message with a single property {{{key1}}}";

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            logger.Verify(Log.With.LogLevel(LogLevel.Error).And.LogMessage(x => x.StartsWith(logMessage.Substring(0, 10))).And.LoggedValue(key1, value1),
                Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_LogLevelExpressionThatDoesNotMatchInvocationAndLogMessageExpressionAndLoggedValueThatDoesMatchInvocations_Throws(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = "value1";
            var logMessage = $"This is a message with a single property {{{key1}}}";

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            Assert.Throws<MockException>(() =>
            {
                logger.Verify(Log.With.LogLevel(x => x == LogLevel.Critical).And.LogMessage(logMessage).And.LoggedValue(key1, value1), Times.Exactly(numberOfInvocations));
            });
        }

        [Test]
        [AutoData]
        public void Verify_LogLevelExpressionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(_fixture.Create<string>());
            }

            logger.Verify(Log.With.LogLevel(x => x == LogLevel.Trace || x == LogLevel.Error), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_LogLevelThatDoesNotMatchInvocations_Throws(int numberOfInvocations)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(_fixture.Create<string>());
            }

            Assert.Throws<MockException>(() =>
            {
                logger.Verify(Log.With.LogLevel(LogLevel.Trace), Times.Exactly(numberOfInvocations));
            });
        }

        [Test]
        public void Verify_LogLevelThatMatchesInvocation_DoesNotThrow()
        {
            var logger = CreateMockedLogger();

            logger.LogError(_fixture.Create<string>());

            logger.Verify(Log.With.LogLevel(LogLevel.Error), Times.Once);
        }

        [Test]
        [AutoData]
        public void Verify_LogLevelThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(_fixture.Create<string>());
            }

            logger.Verify(Log.With.LogLevel(LogLevel.Error), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_LogMessageContainingOriginalFormatProperty_DoesNotThrow(int numberOfInvocations)
        {
            var originalFormatValue = _fixture.Create<string>();
            var logMessage = "This is a log message that contains the property {OriginalFormat}";
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, originalFormatValue);
            }

            logger.Verify(Log.With.LogMessage(logMessage).And.LoggedValue("OriginalFormat", originalFormatValue), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_LogMessageExpressionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations, string logMessage)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage);
            }

            logger.Verify(Log.With.LogMessage(x => x.Select(y => y).SequenceEqual(logMessage.Select(y => y))), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_LogMessageThatDoesNotMatchInvocations_Throws(int numberOfInvocations)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(_fixture.Create<string>());
            }

            Assert.Throws<MockException>(() =>
            {
                logger.Verify(Log.With.LogMessage(_fixture.Create<string>()), Times.Exactly(numberOfInvocations));
            });
        }

        [Test]
        [AutoData]
        public void Verify_LogMessageThatMatchesInvocations_DoesNotThrow(int numberOfInvocations, string logMessage)
        {
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage);
            }

            logger.Verify(Log.With.LogMessage(logMessage), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_NullException_DoesNotThrow(int numberOfInvocations, string logMessage)
        {
            var exception = (Exception) null;
            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(exception, logMessage);
            }

            logger.Verify(Log.With.LogMessage(logMessage).And.Exception(exception), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_NullExceptionMessage_DoesNotThrow(int numberOfInvocations, string logMessage)
        {
            var expectedExceptionMessage = (string) null;
            var exception = new TestException(_fixture.Create<Guid>(), expectedExceptionMessage);

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(exception, logMessage);
            }

            logger.Verify(Log.With.LogMessage(logMessage).And.ExceptionMessage(expectedExceptionMessage), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_NullLoggedValue_DoesNotThrow(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = (string) null;
            var logMessage = $"This is a message with a single null property {{{key1}}}";

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            logger.Verify(Log.With.LoggedValue(key1, value1), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_NullLogMessage_DoesNotThrow(int numberOfInvocations)
        {
            var logMessage = (string) null;

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage);
            }

            logger.Verify(Log.With.LogMessage(logMessage), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ObjectLoggedValueThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var key1 = "key1";
            var value1 = _fixture.Create<Foo>();
            var logMessage = $"This is a message with a single property {{{key1}}}";

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            logger.Verify(Log.With.LoggedValue(key1, value1), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_TestExceptionExpressionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var exception = _fixture.Create<TestException>();

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(exception, _fixture.Create<string>());
            }

            logger.Verify(Log.With.Exception(x => ((TestException) x).Id.Equals(exception.Id)), Times.Exactly(numberOfInvocations));
            logger.Verify(Log.With.Exception<TestException>(x => x.Id.Equals(exception.Id)), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_TestExceptionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var exception = _fixture.Create<TestException>();

            var logger = CreateMockedLogger();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(exception, _fixture.Create<string>());
            }

            logger.Verify(Log.With.Exception(exception), Times.Exactly(numberOfInvocations));
        }
    }
}