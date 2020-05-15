using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace Moq.Contrib.ExpressionBuilders.Logging.LogMethod
{
    public interface IExpressionBuilder
    {
        Expression<Action<T>> ToExpression<T>() where T : ILogger;
    }
}