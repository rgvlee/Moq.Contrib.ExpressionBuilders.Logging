using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Moq.Contrib.ExpressionBuilders.Logging.Interfaces
{
    public interface IExpressionBuilderOptions
    {
        IExpressionBuilder LogLevel(Predicate<LogLevel> predicate);

        IExpressionBuilder EventId(Predicate<EventId> predicate);

        IExpressionBuilder LogMessage(Predicate<string> predicate);

        IExpressionBuilder LoggedValue(Predicate<KeyValuePair<string, object>> predicate);

        IExpressionBuilder ExceptionMessage(Predicate<string> predicate);

        IExpressionBuilder Exception(Predicate<Exception> predicate);

        IExpressionBuilder Exception<T>(Predicate<T> predicate) where T : Exception;

        IExpressionBuilder LogLevel(LogLevel logLevel);

        IExpressionBuilder EventId(EventId eventId);

        IExpressionBuilder LogMessage(string logMessage);

        IExpressionBuilder LoggedValue(string key, object value);

        IExpressionBuilder ExceptionMessage(string exceptionMessage);

        IExpressionBuilder Exception<T>(T exception) where T : Exception;
    }
}