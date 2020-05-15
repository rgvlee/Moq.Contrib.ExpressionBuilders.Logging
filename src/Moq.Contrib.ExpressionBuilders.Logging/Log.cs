using Moq.Contrib.ExpressionBuilders.Logging.LogMethod;

namespace Moq.Contrib.ExpressionBuilders.Logging
{
    public static class Log
    {
        public static IExpressionBuilderPrimaryOperations With => new ExpressionBuilder();
    }
}