using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    [TestFixture]
    public class PrefixedValueInjectionTests
    {
        public class Foo
        {
            public string txtName { get; set; }
        }

        [Test]
        public void InjectWithPrefix()
        {
            var s = new { lblName = "furious" };

            var o = new Foo();
            o.InjectFrom(new LoopValueInjection()
                             .TargetPrefix("txt")
                             .SourcePrefix("lbl"), 
                         s);
            o.txtName.IsEqualTo("furious");
        }

        [Test]
        public void SearchNameTest()
        {
            var s = new { lblName = "furious" };

            var o = new Foo();
            o.InjectFrom(new NameToName()
                             .TargetPrefix("txt")
                             .SourcePrefix("lbl"),
                         s);
            o.txtName.IsEqualTo("furious");
        }

        public class NameToName: PrefixedValueInjection
        {
            protected override void Inject(object source, object target)
            {
                var s = source.GetProps().GetByName(SearchSourceName("txtName"));
                var t = target.GetProps().GetByName(SearchTargetName("lblName"));
                t.SetValue(target, s.GetValue(source));
            }
        }
       
    }
}