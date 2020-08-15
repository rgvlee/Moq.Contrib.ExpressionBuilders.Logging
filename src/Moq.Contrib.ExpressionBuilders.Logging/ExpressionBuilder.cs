using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq.Contrib.ExpressionBuilders.Logging.Interfaces;
using rgvlee.Common;

namespace Moq.Contrib.ExpressionBuilders.Logging
{
    public class ExpressionBuilder : IExpressionBuilder, IExpressionBuilderOptions
    {
        public const string LogMessageLoggedValueKey = "{OriginalFormat}";
        public const string NullLogMessageValue = "[null]";
        private readonly MatchingOptions _options = new MatchingOptions();

        public Expression<Action<T>> Build<T>() where T : ILogger
        {
            return x => x.Log(ResolveLogLevel(_options.LogLevelPredicate),
                ResolveEventId(_options.EventIdPredicate),
                ResolveState(_options.LoggedValuesPredicates),
                ResolveException(_options.ExceptionPredicate),
                (Func<object, Exception, string>) It.IsAny<object>());
        }

        public IExpressionBuilderOptions And => this;

        public IExpressionBuilder LogLevel(Predicate<LogLevel> predicate)
        {
            EnsureArgument.IsNotNull(predicate, nameof(predicate));

            _options.LogLevelPredicate = x => predicate(x);

            return this;
        }

        public IExpressionBuilder EventId(Predicate<EventId> predicate)
        {
            EnsureArgument.IsNotNull(predicate, nameof(predicate));

            _options.EventIdPredicate = x => predicate(x);

            return this;
        }

        public IExpressionBuilder LogMessage(Predicate<string> predicate)
        {
            EnsureArgument.IsNotNull(predicate, nameof(predicate));

            _options.LoggedValuesPredicates.Add(x => x.Key == LogMessageLoggedValueKey && predicate((string) x.Value));

            return this;
        }

        public IExpressionBuilder LoggedValue(Predicate<KeyValuePair<string, object>> predicate)
        {
            EnsureArgument.IsNotNull(predicate, nameof(predicate));

            _options.LoggedValuesPredicates.Add(predicate);

            return this;
        }

        public IExpressionBuilder ExceptionMessage(Predicate<string> predicate)
        {
            EnsureArgument.IsNotNull(predicate, nameof(predicate));

            _options.ExceptionPredicate = x => predicate(x.Message);

            return this;
        }

        public IExpressionBuilder Exception(Predicate<Exception> predicate)
        {
            EnsureArgument.IsNotNull(predicate, nameof(predicate));

            _options.ExceptionPredicate = x => predicate(x);

            return this;
        }

        public IExpressionBuilder Exception<T>(Predicate<T> predicate) where T : Exception
        {
            EnsureArgument.IsNotNull(predicate, nameof(predicate));

            _options.ExceptionPredicate = x => predicate((T) x);

            return this;
        }

        public IExpressionBuilder LogLevel(LogLevel logLevel)
        {
            _options.LogLevelPredicate = x => x == logLevel;

            return this;
        }

        public IExpressionBuilder EventId(EventId eventId)
        {
            _options.EventIdPredicate = x => x == eventId;

            return this;
        }

        public IExpressionBuilder LogMessage(string logMessage)
        {
            _options.LoggedValuesPredicates.Add(x => x.Key == LogMessageLoggedValueKey && (string) x.Value == (logMessage ?? NullLogMessageValue));

            return this;
        }

        public IExpressionBuilder LoggedValue(string key, object value)
        {
            _options.LoggedValuesPredicates.Add(x =>
            {
                if (x.Value == null || x.Value is string)
                {
                    return x.Key == key && x.Value == value;
                }

                return x.Key == key && x.Value.Equals(value);
            });

            return this;
        }

        public IExpressionBuilder ExceptionMessage(string exceptionMessage)
        {
            _options.ExceptionPredicate = x => x.Message == exceptionMessage;

            return this;
        }

        public IExpressionBuilder Exception<T>(T exception) where T : Exception
        {
            _options.ExceptionPredicate = x =>
            {
                if (x == null)
                {
                    return x == exception;
                }

                return x.Equals(exception);
            };

            return this;
        }

        private static LogLevel ResolveLogLevel(Predicate<LogLevel> predicate)
        {
            return predicate != null ? It.Is<LogLevel>(x => predicate(x)) : It.IsAny<LogLevel>();
        }

        private static EventId ResolveEventId(Predicate<EventId> predicate)
        {
            return predicate != null ? It.Is<EventId>(x => predicate(x)) : It.IsAny<EventId>();
        }

        private static IReadOnlyList<KeyValuePair<string, object>> ResolveState(IEnumerable<Predicate<KeyValuePair<string, object>>> predicates)
        {
            return predicates.Any()
                ? It.Is<IReadOnlyList<KeyValuePair<string, object>>>(state => predicates.All(predicate => state.Any(keyValuePair => predicate(keyValuePair))))
                : It.IsAny<IReadOnlyList<KeyValuePair<string, object>>>();
        }

        private static Exception ResolveException(Predicate<Exception> predicate)
        {
            return predicate != null ? It.Is<Exception>(x => predicate(x)) : It.IsAny<Exception>();
        }
    }
}