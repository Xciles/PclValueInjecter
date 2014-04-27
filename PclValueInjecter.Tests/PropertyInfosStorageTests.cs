using System.ComponentModel;
using System.Diagnostics;
using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    [TestFixture]
    public class PropertyInfosStorageTests
    {
        public class Foo
        {
            public string Name { get; set; }
        }

        [Test]
        public void GetsPropertiesFromType()
        {
            PropertyInfosStorage.GetProps(typeof(Foo)).IsNotNull();
        }

        [Test]
        public void GetsPropertiesFromInstance()
        {
            PropertyInfosStorage.GetProps(new Foo()).IsNotNull();
        }

        [Test]
        public void SpeedTestTypeDescriptor()
        {
            TypeDescriptor.GetProperties(typeof(Bar));
            TypeDescriptor.GetProperties(typeof(FooBar));
            TypeDescriptor.GetProperties(typeof(Address));
            TypeDescriptor.GetProperties(typeof(Person));
            TypeDescriptor.GetProperties(typeof(PersonViewModel));
            var w = new Stopwatch();

            w.Start();
            for (int i = 0; i < 10000; i++)
            {
                TypeDescriptor.GetProperties(typeof(Foo));
            }
            w.Stop();
            System.Console.Out.WriteLine(w.Elapsed);
        }

        [Test]
        public void SpeedTestPropertyInfosStorage()
        {
            PropertyInfosStorage.GetProps(typeof (Bar));
            PropertyInfosStorage.GetProps(typeof (FooBar));
            PropertyInfosStorage.GetProps(typeof (Address));
            PropertyInfosStorage.GetProps(typeof (Person));
            PropertyInfosStorage.GetProps(typeof (PersonViewModel));
            var w = new Stopwatch();
            w.Reset();
            w.Start();
            for (int i = 0; i < 10000; i++)
            {
                PropertyInfosStorage.GetProps(typeof(Foo));
            }
            w.Stop();
            System.Console.Out.WriteLine(w.Elapsed);
        }
    }
}