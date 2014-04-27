using System;
using System.ComponentModel;
using System.Diagnostics;
using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    [TestFixture]
    public class DescriptorToolsTest
    {
        public class Foo
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void Aa()
        {
            var props = typeof(Foo).GetProps();
            var w = new Stopwatch();
            w.Start();
            for (int i = 0; i < 100; i++)
            {
                props.Find("Name", true);
            }

            w.Stop();
            Console.WriteLine(w.Elapsed);

            w.Reset();
            w.Start();
            for (int i = 0; i < 100; i++)
            {
                props.GetByName("Name");
            }

            w.Stop();
            Console.WriteLine(w.Elapsed);
        }

        [Test]
        public void GetByNameIgnoresCase()
        {
            TypeDescriptor.GetProperties(typeof(Foo)).GetByName("name", true).IsNotNull();
            TypeDescriptor.GetProperties(typeof(Foo)).GetByName("name", false).IsEqualTo(null);
        }

        [Test]
        public void GetByName()
        {
            new Foo().GetProps().GetByName("Name").IsNotNull();
            new Foo().GetProps().GetByName("unexistent").IsEqualTo(null);
        }

        [Test]
        public void GetByNameType()
        {
            new Foo().GetProps().GetByNameType<string>("Name").IsNotNull();
            new Foo().GetProps().GetByNameType<int>("Name").IsEqualTo(null);
        }

    }
}