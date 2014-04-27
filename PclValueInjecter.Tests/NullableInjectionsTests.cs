using System;
using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    [TestFixture]
    public class NullableInjectionsTests
    {
        public class E
        {
            public bool b { get; set; }
            public int i { get; set; }
            public int j { get; set; }
            public int x { get; set; }
            public string s { get; set; }
        }

        public class D
        {
            public bool? b { get; set; }
            public int? i { get; set; }
            public int? j { get; set; }
            public int x { get; set; }
            public int? s { get; set; }
        }
             
        
        public class NullablesToNormal : ConventionInjection
        {
            protected override bool Match(ConventionInfo c)
            {
                return c.SourceProp.Name == c.TargetProp.Name &&
                       Nullable.GetUnderlyingType(c.SourceProp.Type) == c.TargetProp.Type;
            }
        }

        public class NormalToNullables: ConventionInjection
        {
            protected override bool Match(ConventionInfo c)
            {
                return c.SourceProp.Name == c.TargetProp.Name &&
                       c.SourceProp.Type == Nullable.GetUnderlyingType(c.TargetProp.Type);
            }
        }
       
        [Test]
        public void NullablesToNormalTest()
        {
            var d = new D { b = true, i = 1, x = 3213213 };
            var e = new E();

            e.InjectFrom<NullablesToNormal>(d);

            e.i.IsEqualTo(1);
            e.b.IsEqualTo(true);
            e.x.IsEqualTo(0);
            e.j.IsEqualTo(0);
            e.s.IsEqualTo(null);
        }

        [Test]
        public void NormalToNullablesTest()
        {
            var e = new E { b = true, i = 1, x = 3213213, j = 123 };
            var d = new D();

            d.InjectFrom<NormalToNullables>(e);

            d.i.IsEqualTo(1);
            d.b.IsEqualTo(true);
            d.x.IsEqualTo(0);
            d.j.IsEqualTo(123);
            d.s.IsEqualTo(null);
        }

    }
}
