namespace Moq.Contrib.ExpressionBuilders.Logging.LogMethod
{
    public interface IExpressionBuilderSecondaryOperations : IExpressionBuilder
    {
        IExpressionBuilderPrimaryOperations And { get; }
    }
}