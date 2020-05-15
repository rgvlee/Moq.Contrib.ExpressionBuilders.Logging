using System;
using Microsoft.Extensions.Logging;
using Moq.Contrib.ExpressionBuilders.Logging.LogMethod;

namespace Moq.Contrib.ExpressionBuilders.Logging
{
    public static class VerifyExtensions
    {
        public static void Verify<T>(this T mocked, IExpressionBuilder builder, Func<Times> times) where T : class, ILogger
        {
            mocked.Verify(builder, times());
        }

        public static void Verify<T>(this T mocked, IExpressionBuilder builder, Times times) where T : class, ILogger
        {
            Mock.Get(mocked).Verify(builder.ToExpression<T>(), times);
        }

        public static void Verify<T>(this Mock<T> mock, IExpressionBuilder builder, Func<Times> times) where T : class, ILogger
        {
            mock.Verify(builder, times());
        }

        public static void Verify<T>(this Mock<T> mock, IExpressionBuilder builder, Times times) where T : class, ILogger
        {
            mock.Verify(builder.ToExpression<T>(), times);
        }
    }
}