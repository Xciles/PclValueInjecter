using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    [TestFixture]
    public class KnownTargetValueInjectionTest
    {
        public class Foo
        {
            public string S { get; set; }
            public string S2 { get; set; }
        }

        public class FirstToFoo : KnownTargetValueInjection<Foo>
        {
            protected override void Inject(object source, ref Foo target)
            {
                var fvalue = source.GetProps()[0].GetValue(source);
                target.S = (string)fvalue;
                target.S2 = (string)fvalue;
            }
        }

        [Test]
        public void Test()
        {
            var o = new { Hey = "hello" };
            var foo = new Foo();
            foo.InjectFrom<FirstToFoo>(o);

            foo.S.IsEqualTo("hello");
            foo.S2.IsEqualTo("hello");
        }
    }
}