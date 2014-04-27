using System;
using NUnit.Framework;
using Omu.ValueInjecter;

namespace Tests
{
    public class ExactValueInjectionTests
    {
        public class Foo
        {
            public string HelloWorld { get; set; }

            public int AId { get; set; }
            public int BId { get; set; }
            public int CId { get; set; }
            
            public int HaoDId { get; set; }
        }

        public class Bar
        {
            public string WorldHello { get; set; }

            public string DisplayA { get; set; }
            public string DisplayB { get; set; }
            public string DisplayC { get; set; }

            public string _oO_DisplayD { get; set; }
        }

        public class A { }
        public class B { }
        public class C { }
        public class D { }

        public class InverseHello : ExactValueInjection
        {
            public override string SourceName()
            {
                return "HelloWorld";
            }

            public override string TargetName()
            {
                return "WorldHello";
            }
        }

        public class IdToDisplay<T> : ExactValueInjection
        {
            public override string SourceName()
            {
                return typeof(T).Name + "Id";
            }

            public override string TargetName()
            {
                return "Display" + typeof(T).Name;
            }

            protected override bool TypesMatch(Type sourceType, Type targetType)
            {
                return sourceType == typeof(int) && targetType == typeof(string);
            }

            protected override object SetValue(object v)
            {
                return "display for id " + (int)v + " type " + typeof(T).Name;
            }
        }


        [Test]
        public void InverseHelloTest()
        {
            var f = new Foo() { HelloWorld = "hi" };
            var b = new Bar();

            b.InjectFrom<InverseHello>(f);
            b.WorldHello.IsEqualTo(f.HelloWorld);
        }

        [Test]
        public void IdToDisplayTest()
        {
            var f = new Foo { AId = 1, BId = 2, CId = 3, HaoDId = 4};
            var b = new Bar();

            b.InjectFrom<IdToDisplay<A>>(f)
             .InjectFrom<IdToDisplay<B>>(f)
             .InjectFrom<IdToDisplay<C>>(f)
             .InjectFrom(new IdToDisplay<D>()
                            .TargetPrefix("_oO_")
                            .SourcePrefix("Hao"), f);

            b.DisplayA.IsEqualTo("display for id 1 type A");
            b.DisplayB.IsEqualTo("display for id 2 type B");
            b.DisplayC.IsEqualTo("display for id 3 type C");
            b._oO_DisplayD.IsEqualTo("display for id 4 type D");

        }
    }
}