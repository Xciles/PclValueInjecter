using NUnit.Framework;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void RemovePrefix()
        {
            const string s = "txtName";
            s.RemovePrefix("txt").IsEqualTo("Name");
        }

        [Test]
        public void RemoveSuffix()
        {
            const string s = "NameRaw";
            s.RemoveSuffix("Raw").IsEqualTo("Name");
        }
        // todo add more ;)
    }
}