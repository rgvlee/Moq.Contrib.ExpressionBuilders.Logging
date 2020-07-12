using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Moq.Contrib.ExpressionBuilders.Logging
{
    public interface IExpressionBuilderOptions : IExpressionBuilder
    {
        IExpressionBuilderFluentAnd LogLevel(Predicate<LogLevel> predicate);

        IExpressionBuilderFluentAnd LogLevel(LogLevel logLevel);

        IExpressionBuilderFluentAnd EventId(Predicate<EventId> predicate);

        IExpressionBuilderFluentAnd EventId(EventId eventId);

        IExpressionBuilderFluentAnd LogMessage(string logMessage);

        IExpressionBuilderFluentAnd LogMessage(Predicate<string> predicate);

        IExpressionBuilderFluentAnd LoggedValue(string key, object value);

        IExpressionBuilderFluentAnd LoggedValue(Predicate<KeyValuePair<string, object>> predicate);

        IExpressionBuilderFluentAnd ExceptionMessage(string exceptionMessage);

        IExpressionBuilderFluentAnd ExceptionMessage(Predicate<string> predicate);

        IExpressionBuilderFluentAnd Exception(Exception exception);

        IExpressionBuilderFluentAnd Exception<T>(T exception) where T : Exception;

        IExpressionBuilderFluentAnd Exception(Predicate<Exception> predicate);

        IExpressionBuilderFluentAnd Exception<T>(Predicate<T> predicate) where T : Exception;
    }
}