using System;

namespace Moq.Contrib.ExpressionBuilders.Logging.Tests
{
    public class Foo
    {
        public Foo(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}