using NUnit.Framework;
using Omu.ValueInjecter;

namespace Tests
{
    [TestFixture]
    public class ValueInjecterTests
    {
        public class Foo
        {
            public string Name { get; set; }
        }

        [Test]
        public void DefaultInject()
        {
            var f = new Foo();
            var s = new { Name = "athene" };

            f.InjectFrom(s);
            f.Name.IsEqualTo(s.Name);

            var v = new ValueInjecter();
            f = new Foo();
            v.Inject(f, s);
            f.Name.IsEqualTo("athene");
        }

        [Test]
        public void InjectWithoutSource()
        {
            var o = new Foo();
            o.InjectFrom<NoSource>();
            o.Name.IsEqualTo("hi");

            var oo = new Foo();
            oo.InjectFrom(new NoSource());
            oo.Name.IsEqualTo("hi");


            var v = new ValueInjecter();

            var f = new Foo();
            v.Inject<NoSource>(f);
            f.Name.IsEqualTo("hi");

            f = new Foo();
            v.Inject(new NoSource(), f);
            f.Name.IsEqualTo("hi");
        }

        public class NoSource : NoSourceValueInjection
        {
            protected override void Inject(object target)
            {
                var targetProps = target.GetProps();
                for (int i = 0; i < targetProps.Count; i++)
                {
                    var prop = targetProps[i];
                    if (prop.PropertyType == typeof(string))
                        prop.SetValue(target, "hi");
                }
            }
        }


    }
}