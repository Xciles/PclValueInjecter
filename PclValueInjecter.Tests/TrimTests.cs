using System;
using NUnit.Framework;
using Omu.ValueInjecter;

namespace Tests
{
    public class Attr
    {
        [Test]
        public void Test()
        {
            var attr = typeof(Aaaa).GetCustomAttributes(true);
            attr.Length.IsEqualTo(1);
        }

        public class Foo
        {
            public Aaaa Aa { get; set; }
        }

        public class Oo : Attribute
        {}

        [Oo]
        public class Aaaa {}

    }
    public class TrimTests
    {
        [Test]
        public void TrimFromAnonymousToFoo()
        {
            var a = new {Aa = " hello ", Bb = "  yo "};
            var foo = new Foo();
            foo.InjectFrom<TrimStrings>(a);
            Assert.AreEqual("hello", foo.Aa);
            Assert.AreEqual("yo", foo.Bb);
        }

        [Test]
        public void TrimFooToFoo()
        {
            var foo = new Foo {Aa = " a ", Bb = " x "};
            foo.InjectFrom<TrimStrings>(foo);
            Assert.AreEqual("a", foo.Aa);
            Assert.AreEqual("x", foo.Bb);
        }

        [Test]
        public void SelfTrim()
        {
            var foo = new Foo {Aa = " a ", Bb = " b "};
            foo.InjectFrom<TrimSelf>();
            Assert.AreEqual("a", foo.Aa);
            Assert.AreEqual("b", foo.Bb);
        }

        public class TrimStrings : LoopValueInjection<string,string>
        {
            protected override string SetValue(string sourcePropertyValue)
            {
                return sourcePropertyValue.Trim();
            }
        }

        public class TrimSelf : NoSourceValueInjection
        {
            protected override void Inject(object target)
            {
                var props = target.GetProps();
                for (var i = 0; i < props.Count; i++)
                {
                    if (props[i].PropertyType != typeof(string)) continue;
                    var value = props[i].GetValue(target);

                    if(value != null) props[i].SetValue(target, value.ToString().Trim());
                }
            }
        }

        public class Foo
        {
            public string Aa { get; set; }
            public string Bb { get; set; }
        }
    }
}