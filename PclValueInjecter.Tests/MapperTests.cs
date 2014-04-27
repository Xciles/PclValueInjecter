using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    public class MapperTests
    {
        public class Foo
        {
            public string Name { get; set; }
            public int Xyz { get; set; }
            public string Props { get; set; }
            public Foo Child { get; set; }
            public IEnumerable<Foo> Foos { get; set; }
        }

        public class Bar
        {
            public string Name { get; set; }
            public string NoConvention { get; set; }
            public Bar Child { get; set; }
            public ICollection<Bar> Foos { get; set; }
        }

        [TearDown]
        public void End()
        {
            PclValueInjecter.MapperFactory.ClearMappers();
        }

        [Test]
        public void MapShouldMapToExistingObject()
        {
            var f2 = new Foo { Xyz = 43 };
            PclValueInjecter.Mapper.Map(new { Name = "hi" }, f2);

            Assert.AreEqual("hi", f2.Name);
            Assert.AreEqual(43, f2.Xyz);
        }

        [Test]
        public void MapShouldCreateNewMappedObject()
        {
            var foo = new Foo { Name = "f1", Props = "p", Xyz = 123 };
            var foo2 = PclValueInjecter.Mapper.Map<Foo, Foo>(foo);

            Assert.AreEqual(foo.Name, foo2.Name);
            Assert.AreEqual(foo.Props, foo2.Props);
            Assert.AreEqual(foo.Xyz, foo2.Xyz);
        }

        [Test]
        public void ShouldMapChildPropertiesFooToBar()
        {
            var foo = new Foo { Child = new Foo { Name = "aaa" } };
            var bar = new Bar();

            PclValueInjecter.Mapper.Map(foo, bar);

            Assert.AreEqual("aaa", bar.Child.Name);
        }

        [Test]
        public void MapShouldMapCollections()
        {
            var foos = new List<Foo>
                           {
                               new Foo {Name = "f1"},
                               new Foo {Name = "f2"},
                               new Foo {Name = "f3"},
                           };

            var foos2 = PclValueInjecter.Mapper.Map<IEnumerable<Foo>, IList<Foo>>(foos);
            Assert.AreEqual(3, foos2.Count());
            Assert.AreEqual("f1", foos2.First().Name);
            Assert.AreEqual("f2", foos2.Skip(1).First().Name);
            Assert.AreEqual("f3", foos2.Last().Name);
        }

        [Test]
        public void MapShouldMapCollectionTypeProperties()
        {
            var foo = new Foo
            {
                Foos = new List<Foo>
                           {
                               new Foo{Name = "f1"},
                               new Foo{Name = "f2"},
                               new Foo{Name = "f3"},
                           }
            };

            var bar = Mapper.Map<Foo, Bar>(foo);

            Assert.AreEqual(foo.Foos.Count(), bar.Foos.Count());
            Assert.AreEqual("f1", bar.Foos.First().Name);
            Assert.AreEqual("f3", bar.Foos.Last().Name);
        }




        public class FooBar : PclValueInjecter.TypeMapper<Foo, Bar>
        {
            public override Bar Map(Foo source, Bar target)
            {
                base.Map(source, target);
                target.NoConvention = source.Name + source.Xyz + source.Props;
                return target;
            }
        }

        [Test]
        public void MapShouldUseFooBarTypeMapperForMapping()
        {
            PclValueInjecter.MapperFactory.AddMapper(new FooBar());
            var foo = new Foo { Name = "a", Props = "b", Xyz = 123 };
            var bar = new Bar();

            PclValueInjecter.Mapper.Map(foo, bar);

            Assert.AreEqual("a123b", bar.NoConvention);
            Assert.AreEqual(foo.Name, bar.Name);
        }

        [Test]
        public void MapShouldMapCollectionPropertiesAndUseFooBarTypeMapper()
        {
            PclValueInjecter.MapperFactory.AddMapper(new FooBar());
            var foo = new Foo
            {
                Foos = new List<Foo>
                           {
                               new Foo{Name = "f1",Props = "v",Xyz = 19},
                               new Foo{Name = "f2",Props = "i",Xyz = 7},
                               new Foo{Name = "f3",Props = "v",Xyz = 3},
                           }
            };

            var bar = PclValueInjecter.Mapper.Map<Foo, Bar>(foo);

            Assert.AreEqual(foo.Foos.Count(), bar.Foos.Count());

            var ffoos = foo.Foos.ToArray();
            var bfoos = bar.Foos.ToArray();

            for (var i = 0; i < ffoos.Count(); i++)
            {
                Assert.AreEqual(ffoos[i].Name, bfoos[i].Name);
                Assert.AreEqual(ffoos[i].Name + ffoos[i].Xyz + ffoos[i].Props, bfoos[i].NoConvention);
            }
        }

    }
}
