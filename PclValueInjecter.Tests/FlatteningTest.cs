using System;
using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    [TestFixture]
    public class FlatteningTest
    {
        public class Boo
        {
            public Boo Parent { get; set; }
        }

        public class Foo
        {
            public Foo Foo1 { get; set; }
            public Foo Foo2 { get; set; }
            public string Name { get; set; }
            public string NameZype { get; set; }
            public int Age { get; set; }
        }

        public class FlatFoo
        {
            public string Foo1Foo2Foo1Name { get; set; }
            public string Foo1Name { get; set; }
            public string Foo2Name { get; set; }
            public string Foo2NameZype { get; set; }
            public string Foo1Age { get; set; }
            public bool Age { get; set; }

        }

        public class IntToStringFlat : FlatLoopValueInjection<int, string>
        {
            protected override string SetValue(int sourceValue)
            {
                return sourceValue.ToString();
            }
        }

        public class IntToStrUnflat : UnflatLoopValueInjection<string, int>
        {
            protected override int SetValue(string sourceValue)
            {
                return Convert.ToInt32(sourceValue);
            }
        }

        [Test]
        public void Unflattening()
        {
            var flat = new FlatFoo
                           {
                               Foo1Foo2Foo1Name = "cool",
                               Foo1Name = "abc",
                               Foo2Name = "123",
                           };

            var unflat = new Foo();

            unflat.InjectFrom<UnflatLoopValueInjection>(flat);

            unflat.Foo1.Foo2.Foo1.Name.IsEqualTo("cool");
            unflat.Foo1.Name.IsEqualTo("abc");
            unflat.Foo2.Name.IsEqualTo("123");
        }

        [Test]
        public void Flattening()
        {
            var unflat = new Foo
                             {
                                 Name = "foo",
                                 Foo1 = new Foo
                                            {
                                                Name = "abc",
                                                Foo2 = new Foo { Foo1 = new Foo { Name = "inner" } }
                                            },
                                 Foo2 = new Foo { Name = "123", NameZype = "aaa" },
                             };

            var flat = new FlatFoo();

            flat.InjectFrom<FlatLoopValueInjection>(unflat);

            flat.Foo2NameZype.IsEqualTo("aaa");
        }

        [Test]
        public void GenericFlatTest()
        {
            var foo = new Foo { Foo1 = new Foo { Age = 18 } };
            var flat = new FlatFoo();

            flat.InjectFrom<IntToStringFlat>(foo);
            flat.Foo1Age.IsEqualTo("18");
        }

        [Test]
        public void GenericUnflatTest()
        {
            var flat = new FlatFoo { Foo1Age = "16" };
            var foo = new Foo();

            foo.InjectFrom<IntToStrUnflat>(flat);
            foo.Foo1.Age.IsEqualTo(16);
        }


    }
}