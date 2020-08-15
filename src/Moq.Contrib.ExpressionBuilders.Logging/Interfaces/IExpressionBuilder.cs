using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace Moq.Contrib.ExpressionBuilders.Logging.Interfaces
{
    public interface IExpressionBuilder
    {
        IExpressionBuilderOptions And { get; }

        Expression<Action<T>> Build<T>() where T : ILogger;
    }
}