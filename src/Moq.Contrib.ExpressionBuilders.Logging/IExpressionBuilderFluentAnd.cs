namespace Moq.Contrib.ExpressionBuilders.Logging
{
    public interface IExpressionBuilderFluentAnd : IExpressionBuilder
    {
        IExpressionBuilderOptions And { get; }
    }
}