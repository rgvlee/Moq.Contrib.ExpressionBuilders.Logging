using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace Moq.Contrib.ExpressionBuilders.Logging
{
    public interface IExpressionBuilder
    {
        Expression<Action<T>> Build<T>() where T : ILogger;
    }
}