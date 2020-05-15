namespace Testing.Common
{
    public class Foo : IFoo
    {
        public Foo(IBar bar)
        {
            Bar = bar;
        }

        public IBar Bar { get; set; }

        public void SomeMethod()
        {
            Bar.AnotherMethod();
        }

        public int SomeMethod(int value)
        {
            return Bar.AnotherMethod(value);
        }
    }
}