using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq.Contrib.ExpressionBuilders.Logging.Helpers;
using NUnit.Framework;

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
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Trace);
            //LoggerHelper.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
            _fixture = new Fixture();

            logger = CreateMockedLogger();
        }

        private Fixture _fixture;

        private T logger;

        private static T CreateMockedLogger()
        {
            return Mock.Of<T>();
        }

        [Test]
        [AutoData]
        public void Verify_EventIdExpressionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations, EventId eventId)
        {
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
            var key1 = _fixture.Create<string>();
            var value1 = _fixture.Create<string>();
            var logMessage = $"This is a message with a single property {{{key1}}}";

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
            var key1 = _fixture.Create<string>();
            var key2 = _fixture.Create<string>();
            var value1 = _fixture.Create<string>();
            var value2 = _fixture.Create<string>();
            var logMessage = $"This is a message where the second property {{{key2}}} is first, followed by the first property {{{key1}}}";

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
            var key1 = _fixture.Create<string>();
            var value1 = _fixture.Create<string>();
            var logMessage = $"This is a message with a single property {{{key1}}}";

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
            var key1 = _fixture.Create<string>();
            var value1 = _fixture.Create<string>();
            var logMessage = $"This is a message with a single property {{{key1}}}";

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
            var key1 = _fixture.Create<string>();
            var value1 = _fixture.Create<string>();
            var logMessage = $"This is a message with a single property {{{key1}}}";

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
            var key1 = _fixture.Create<string>();
            var value1 = _fixture.Create<string>();
            var logMessage = $"This is a message with a single property {{{key1}}}";

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
            logger.LogError(_fixture.Create<string>());

            logger.Verify(Log.With.LogLevel(LogLevel.Error), Times.Once);
        }

        [Test]
        [AutoData]
        public void Verify_LogLevelThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
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
            var logMessage = "This is a message that contains the property {OriginalFormat}";

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
            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(null, logMessage);
            }

            logger.Verify(Log.With.LogMessage(logMessage).And.Exception((Exception) null), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_NullExceptionMessage_DoesNotThrow(int numberOfInvocations, string logMessage)
        {
            var exception = new TestException(_fixture.Create<Guid>(), null);

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(exception, logMessage);
            }

            logger.Verify(Log.With.LogMessage(logMessage).And.ExceptionMessage((string) null), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ParamsArrayWithNullItem_DoesNotThrow(int numberOfInvocations)
        {
            var key1 = _fixture.Create<string>();
            var logMessage = $"This is a message with a single property {{{key1}}}";

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, new object[] { null });
            }

            logger.Verify(Log.With.LoggedValue(key1, null), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_NullLogMessage_DoesNotThrow(int numberOfInvocations)
        {
            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(null);
            }

            logger.Verify(Log.With.LogMessage((string) null), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ObjectLoggedValueThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var key1 = _fixture.Create<string>();
            var value1 = _fixture.Create<Bar>();
            var logMessage = $"This is a message with a single property {{{key1}}}";

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, value1);
            }

            logger.Verify(Log.With.LoggedValue(key1, value1), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ParamsArrayWithTwoNullItems_DoesNotThrow(int numberOfInvocations)
        {
            var key1 = _fixture.Create<string>();
            var key2 = _fixture.Create<string>();
            var logMessage = $"This is a message with multiple properties {{{key1}}} {{{key2}}}";

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, null, null);
            }

            logger.Verify(Log.With.LoggedValue(key1, null).And.LoggedValue(key2, null), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_TestExceptionExpressionThatMatchesInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var exception = _fixture.Create<TestException>();

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

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(exception, _fixture.Create<string>());
            }

            logger.Verify(Log.With.Exception(exception), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ParamsArrayWithNullItemAndStringItem_DoesNotThrow(int numberOfInvocations)
        {
            var key1 = _fixture.Create<string>();
            var key2 = _fixture.Create<string>();
            var logMessage = $"This is a message with multiple properties {{{key1}}} {{{key2}}}";
            var stringLoggedValue = _fixture.Create<string>();

            for (var i = 0; i < numberOfInvocations; i++)
            {
                logger.LogError(logMessage, null, stringLoggedValue);
            }

            logger.Verify(Log.With.LoggedValue(key1, null).And.LoggedValue(key2, stringLoggedValue), Times.Exactly(numberOfInvocations));
        }
    }
}