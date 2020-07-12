using Microsoft.Extensions.Logging;
using Moq.Language.Flow;

namespace Moq.Contrib.ExpressionBuilders.Logging
{
    public static class SetupExtensions
    {
        public static ISetup<T> Setup<T>(this Mock<T> mock, IExpressionBuilder builder) where T : class, ILogger
        {
            return mock.Setup(builder.Build<T>());
        }
    }
}