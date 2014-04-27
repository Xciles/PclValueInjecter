using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    public static class TestingTools
    {
        public static void IsEqualTo(this object o, object to)
        {
            Assert.AreEqual(to, o);
        }

        public static void IsNotNull(this object o)
        {
            Assert.IsNotNull(o);
        }

    }
}