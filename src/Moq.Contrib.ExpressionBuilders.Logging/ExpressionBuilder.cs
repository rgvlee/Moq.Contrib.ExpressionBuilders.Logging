using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Moq.Contrib.ExpressionBuilders.Logging.Helpers;
using Moq.Contrib.ExpressionBuilders.Logging.Interfaces;

namespace Moq.Contrib.ExpressionBuilders.Logging
{
    public class ExpressionBuilder : IExpressionBuilder, IExpressionBuilderOptions
    {
        private const string LogMessageLoggedValueKey = "{OriginalFormat}";
        private const string NullLogMessageValue = "[null]";

        private static readonly ILogger<ExpressionBuilder> Logger = LoggerHelper.CreateLogger<ExpressionBuilder>();

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
                Logger.LogTrace("Logged value: {key}: {value}", x.Key, x.Value);
                if (value != null)
                {
                    Logger.LogTrace("Expected logged value: {key}: {value} ({type})", key, value, value.GetType().Name);
                }
                else
                {
                    Logger.LogTrace("Expected logged value: {key}: {value}", key, value);
                }

                if (x.Key != key)
                {
                    Logger.LogTrace("Key does not match; {left}, {right}", x.Key, key);
                    return false;
                }

                if (x.Value == null)
                {
                    Logger.LogTrace("value == null: {result}", value == null);
                    return value == null;
                }

                if (value is string stringValue)
                {
                    Logger.LogTrace("x.Value == stringValue: {result}", x.Value == stringValue);
                    return (string) x.Value == stringValue;
                }

                Logger.LogTrace("x.Value.Equals(value): {result}", x.Value.Equals(value));
                return x.Value.Equals(value);
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
                    return exception == null;
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
                ? It.Is<IReadOnlyList<KeyValuePair<string, object>>>(state => predicates.All(predicate => ResolveLoggedValues(state).Any(keyValuePair => predicate(keyValuePair))))
                : It.IsAny<IReadOnlyList<KeyValuePair<string, object>>>();
        }

        private static IReadOnlyList<KeyValuePair<string, object>> ResolveLoggedValues(IReadOnlyList<KeyValuePair<string, object>> state)
        {
            Logger.LogTrace("{stateCount}", state.Count.ToString());

            var logMessage = (string) state[state.Count - 1].Value;
            Logger.LogDebug("Log message: {logMessage}", logMessage);

            var keys = Regex.Matches(logMessage, @"(?<!{){((?:\{{2})*([^{}]*)(?:}{2})*)}(?!})")
                .Cast<System.Text.RegularExpressions.Match>()
                .Select(x => x.Groups[2].Value)
                .ToList();
            Logger.LogDebug("Keys extracted from the log message: {keys}", string.Join(", ", keys));

            var loggedValues = new List<KeyValuePair<string, object>>();
            for (var i = 0; i < state.Count; i++)
            {
                try
                {
                    var kvp = state[i];
                    Logger.LogTrace("{key}: {value}", kvp.Key, kvp.Value);
                    loggedValues.Add(kvp);
                }
                catch (Exception ex)
                {
                    Logger.LogTrace(ex, "Unable to get the item at index {index}", i.ToString());
                    if (state.Count <= 2)
                    {
                        continue;
                    }

                    Logger.LogTrace(ex, "Manually adding null logged value", i.ToString());
                    loggedValues.Add(new KeyValuePair<string, object>(keys[i], null));
                }
            }

            Logger.LogDebug("Logged values: {loggedValues}", string.Join(", ", loggedValues));

            return loggedValues;
        }

        private static Exception ResolveException(Predicate<Exception> predicate)
        {
            return predicate != null ? It.Is<Exception>(x => predicate(x)) : It.IsAny<Exception>();
        }
    }
}