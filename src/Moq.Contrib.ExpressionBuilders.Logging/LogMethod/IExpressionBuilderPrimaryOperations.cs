using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Moq.Contrib.ExpressionBuilders.Logging.LogMethod
{
    public interface IExpressionBuilderPrimaryOperations : IExpressionBuilder
    {
        IExpressionBuilderSecondaryOperations LogLevel(Predicate<LogLevel> predicate);

        IExpressionBuilderSecondaryOperations LogLevel(LogLevel logLevel);

        IExpressionBuilderSecondaryOperations EventId(Predicate<EventId> predicate);

        IExpressionBuilderSecondaryOperations EventId(EventId eventId);

        IExpressionBuilderSecondaryOperations LogMessage(string logMessage);

        IExpressionBuilderSecondaryOperations LogMessage(Predicate<string> predicate);

        IExpressionBuilderSecondaryOperations LoggedValue(string key, object value);

        IExpressionBuilderSecondaryOperations LoggedValue(Predicate<KeyValuePair<string, object>> predicate);

        IExpressionBuilderSecondaryOperations ExceptionMessage(string exceptionMessage);

        IExpressionBuilderSecondaryOperations ExceptionMessage(Predicate<string> predicate);

        IExpressionBuilderSecondaryOperations Exception(Exception exception);

        IExpressionBuilderSecondaryOperations Exception<T>(T exception) where T : Exception;

        IExpressionBuilderSecondaryOperations Exception(Predicate<Exception> predicate);

        IExpressionBuilderSecondaryOperations Exception<T>(Predicate<T> predicate) where T : Exception;
    }
}