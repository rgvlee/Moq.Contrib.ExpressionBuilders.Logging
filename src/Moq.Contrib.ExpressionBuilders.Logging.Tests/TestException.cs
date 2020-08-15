using System;

namespace Moq.Contrib.ExpressionBuilders.Logging.Tests
{
    public class TestException : Exception
    {
        public TestException(Guid id)
        {
            Id = id;
        }

        public TestException(Guid id, string message = null)
        {
            Id = id;
            Message = message;
        }

        public Guid Id { get; }

        public override string Message { get; }
    }
}