using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace Moq.Contrib.ExpressionBuilders.Logging
{
    public class ExpressionBuilder : IExpressionBuilder, IExpressionBuilderOptions, IExpressionBuilderFluentAnd
    {
        private readonly MatchingOptions _options = new MatchingOptions();

        public Expression<Action<T>> Build<T>() where T : ILogger
        {
            return x => x.Log(ResolveLogLevel(_options.LogLevelPredicate),
                ResolveEventId(_options.EventIdPredicate),
                ResolveState(_options.LoggedValuesPredicates),
                ResolveException(_options.ExceptionPredicate),
                (Func<object, Exception, string>) It.IsAny<object>());
        }

        public IExpressionBuilderFluentAnd LogLevel(Predicate<LogLevel> predicate)
        {
            _options.LogLevelPredicate = predicate;
            return this;
        }

        public IExpressionBuilderFluentAnd LogLevel(LogLevel logLevel)
        {
            _options.LogLevelPredicate = x => x == logLevel;
            return this;
        }

        public IExpressionBuilderFluentAnd EventId(Predicate<EventId> predicate)
        {
            _options.EventIdPredicate = predicate;
            return this;
        }

        public IExpressionBuilderFluentAnd EventId(EventId eventId)
        {
            _options.EventIdPredicate = x => x == eventId;
            return this;
        }

        public IExpressionBuilderFluentAnd LogMessage(string logMessage)
        {
            _options.LoggedValuesPredicates.Add(x => x.Key.Equals("{OriginalFormat}") && x.Value.Equals(logMessage));
            return this;
        }

        public IExpressionBuilderFluentAnd LogMessage(Predicate<string> predicate)
        {
            _options.LoggedValuesPredicates.Add(x => x.Key.Equals("{OriginalFormat}") && predicate((string) x.Value));
            return this;
        }

        public IExpressionBuilderFluentAnd LoggedValue(string key, object value)
        {
            _options.LoggedValuesPredicates.Add(x => x.Key.Equals(key) && x.Value.Equals(value));
            return this;
        }

        public IExpressionBuilderFluentAnd LoggedValue(Predicate<KeyValuePair<string, object>> predicate)
        {
            _options.LoggedValuesPredicates.Add(predicate);
            return this;
        }

        public IExpressionBuilderFluentAnd ExceptionMessage(string exceptionMessage)
        {
            _options.ExceptionPredicate = x => x.Message.Equals(exceptionMessage);
            return this;
        }

        public IExpressionBuilderFluentAnd ExceptionMessage(Predicate<string> predicate)
        {
            _options.ExceptionPredicate = x => predicate(x.Message);
            return this;
        }

        public IExpressionBuilderFluentAnd Exception(Exception exception)
        {
            _options.ExceptionPredicate = x => x.Equals(exception);
            return this;
        }

        public IExpressionBuilderFluentAnd Exception<T>(T exception) where T : Exception
        {
            _options.ExceptionPredicate = x => ((T) x).Equals(exception);
            return this;
        }

        public IExpressionBuilderFluentAnd Exception(Predicate<Exception> predicate)
        {
            _options.ExceptionPredicate = predicate;
            return this;
        }

        public IExpressionBuilderFluentAnd Exception<T>(Predicate<T> predicate) where T : Exception
        {
            _options.ExceptionPredicate = x => predicate((T) x);
            return this;
        }

        public IExpressionBuilderOptions And => this;

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