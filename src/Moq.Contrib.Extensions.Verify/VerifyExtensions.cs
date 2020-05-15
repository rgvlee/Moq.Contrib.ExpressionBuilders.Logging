using System;
using System.Linq.Expressions;

namespace Moq.Contrib.Extensions.Verify
{
    public static class VerifyExtensions
    {
        public static void Verify<T>(this T mocked, Expression<Action<T>> expression, Func<Times> times) where T : class
        {
            mocked.Verify(expression, times());
        }

        public static void Verify<T>(this T mocked, Expression<Action<T>> expression, Times times) where T : class
        {
            Mock.Get(mocked).Verify(expression, times);
        }

        public static void Verify<T, TResult>(this T mocked, Expression<Func<T, TResult>> expression, Func<Times> times) where T : class
        {
            mocked.Verify(expression, times());
        }

        public static void Verify<T, TResult>(this T mocked, Expression<Func<T, TResult>> expression, Times times) where T : class
        {
            Mock.Get(mocked).Verify(expression, times);
        }
    }
}