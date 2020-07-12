namespace Moq.Contrib.ExpressionBuilders.Logging
{
    public static class Log
    {
        public static IExpressionBuilderOptions With => new ExpressionBuilder();
    }
}