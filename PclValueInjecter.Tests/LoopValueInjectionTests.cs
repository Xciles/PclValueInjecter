using System;
using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    [TestFixture]
    public class LoopValueInjectionTests
    {
        public class Foo
        {
            public string Name { get; set; }
            public int Number { get; set; }
            public DateTime Date { get; set; }
            public long Prop { get; set; }
        }

        public class Bar
        {
            public string Name { get; set; }
            public int Number { get; set; }
            public DateTime Date { get; set; }
            public Foo Prop { get; set; }
        }

        public class LongToFoo : LoopValueInjection
        {
            protected override bool TypesMatch(Type sourceType, Type targetType)
            {
                return sourceType == typeof(long) && targetType == typeof(Foo);
            }

            protected override object SetValue(object v)
            {
                return new Foo { Name = "name " + v, Prop = (long)v, Number = 123 };
            }

            protected override bool AllowSetValue(object value)
            {
                return (long)value != 7;
            }
        }

        [Test]
        public void LoopValueInjectionTest()
        {
            var anonymous = new { Name = "cool", Number = 32, Date = DateTime.Now, Something = "aaa", Prop = "aa" };
            var foo = new Foo();

            foo.InjectFrom(anonymous);

            foo.Date.IsEqualTo(anonymous.Date);
            foo.Name.IsEqualTo(anonymous.Name);
            foo.Number.IsEqualTo(anonymous.Number);
            foo.Prop.IsEqualTo(0);
        }

        [Test]
        public void LongToFooTest()
        {
            var foo = new Foo { Name = "a", Number = 32, Prop = 5 };
            var bar = new Bar();

            bar.InjectFrom(foo)
                .InjectFrom<LongToFoo>(foo);

            bar.Name.IsEqualTo(foo.Name);
            bar.Number.IsEqualTo(foo.Number);

            bar.Prop.Prop.IsEqualTo(foo.Prop);
            bar.Prop.Name.IsEqualTo("name " + foo.Prop);
            bar.Prop.Number.IsEqualTo(123);
        }

        [Test]
        public void AllowSetValueTest()
        {
            var foo = new Foo { Prop = 7 };
            var bar = new Bar();

            bar.InjectFrom<LongToFoo>(bar);
            bar.Prop.IsEqualTo(null);
        }

        public class FooToInt : LoopValueInjection<Foo, int>
        {
            protected override int SetValue(Foo sourcePropertyValue)
            {
                return sourcePropertyValue.Number;
            }
        }

        [Test]
        public void GenericLoopTest()
        {
            var o = new { Number = new Foo { Number = 37 }, NonExistent = new Foo() };
            var foo = new Foo();

            foo.InjectFrom<FooToInt>(o);

            foo.Number.IsEqualTo(37);
        }
    }
}