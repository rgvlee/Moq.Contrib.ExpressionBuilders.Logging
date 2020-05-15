using System;

namespace Moq.Contrib.ExpressionBuilders.Logging.Tests
{
    public class TestException : Exception
    {
        public TestException(string message) : base(message) { }

        public Guid Id { get; } = Guid.NewGuid();
    }
}