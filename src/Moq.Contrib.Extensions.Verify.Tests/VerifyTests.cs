using AutoFixture.NUnit3;
using NUnit.Framework;
using Testing.Common;

// ReSharper disable NUnit.MethodWithParametersAndTestAttribute

namespace Moq.Contrib.Extensions.Verify.Tests
{
    [TestFixture]
    public class VerifyTests
    {
        [Test]
        [AutoData]
        public void Verify_ForMethodWithReturnValueWithExpressionMatchingActualInvocation_DoesNotThrow(int value)
        {
            var bar = Mock.Of<IBar>();
            var foo = new Foo(bar);

            foo.SomeMethod(value);

            bar.Verify(x => x.AnotherMethod(value), Times.Once);
            bar.Verify(x => x.AnotherMethod(It.IsAny<int>()), Times.Once);
            bar.Verify(x => x.AnotherMethod(It.Is<int>(y => y.Equals(value))), Times.Once);
        }

        [Test]
        [AutoData]
        public void Verify_ForMethodWithReturnValueWithExpressionMatchingActualInvocations_DoesNotThrow(int numberOfInvocations, int value)
        {
            var bar = Mock.Of<IBar>();
            var foo = new Foo(bar);

            for (var i = 0; i < numberOfInvocations; i++)
            {
                foo.SomeMethod(value);
            }

            bar.Verify(x => x.AnotherMethod(value), Times.Exactly(numberOfInvocations));
            bar.Verify(x => x.AnotherMethod(It.IsAny<int>()), Times.Exactly(numberOfInvocations));
            bar.Verify(x => x.AnotherMethod(It.Is<int>(y => y.Equals(value))), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ForMethodWithReturnValueWithExpressionThatDoesNotMatchActualInvocations_Throws(int numberOfInvocations)
        {
            var bar = Mock.Of<IBar>();
            var foo = new Foo(bar);

            for (var i = 0; i < numberOfInvocations; i++)
            {
                foo.SomeMethod(1);
            }

            Assert.Throws<MockException>(() =>
            {
                bar.Verify(x => x.AnotherMethod(), Times.Exactly(numberOfInvocations));
            });
        }

        [Test]
        public void Verify_ForVoidMethodWithExpressionMatchingActualInvocation_DoesNotThrow()
        {
            var bar = Mock.Of<IBar>();
            var foo = new Foo(bar);

            foo.SomeMethod();

            bar.Verify(x => x.AnotherMethod(), Times.Once);
        }

        [Test]
        [AutoData]
        public void Verify_ForVoidMethodWithExpressionMatchingActualInvocations_DoesNotThrow(int numberOfInvocations)
        {
            var bar = Mock.Of<IBar>();
            var foo = new Foo(bar);

            for (var i = 0; i < numberOfInvocations; i++)
            {
                foo.SomeMethod();
            }

            bar.Verify(x => x.AnotherMethod(), Times.Exactly(numberOfInvocations));
        }

        [Test]
        [AutoData]
        public void Verify_ForVoidMethodWithExpressionThatDoesNotMatchActualInvocations_Throws(int numberOfInvocations)
        {
            var bar = Mock.Of<IBar>();
            var foo = new Foo(bar);

            for (var i = 0; i < numberOfInvocations; i++)
            {
                foo.SomeMethod();
            }

            Assert.Throws<MockException>(() =>
            {
                bar.Verify(x => x.AnotherMethod(1), Times.Exactly(numberOfInvocations));
            });
        }
    }
}