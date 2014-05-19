using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    public class MapperTests
    {
        public enum EFoo
        {
            Foo,
            Bar
        }
        public enum EBar
        {
            Foo,
            Bar
        }

        public class Foo
        {
            public string Name { get; set; }
            public int? Tint { get; set; }
            public int Xyz { get; set; }
            public EFoo SomeType { get; set; }
            public string Props { get; set; }
            public Foo Child { get; set; }
            public IEnumerable<Foo> Foos { get; set; }
            public IList<int> FooIds { get; set; }
        }

        public class Bar
        {
            public string Name { get; set; }
            public int Tint { get; set; }
            public EBar SomeType { get; set; }
            public string NoConvention { get; set; }
            public Bar Child { get; set; }
            public ICollection<Bar> Foos { get; set; }
            public IList<int> BarIds { get; set; }
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
                               new Foo {Name = "f1", SomeType = EFoo.Bar, FooIds = new List<int>() { 1,2,3,4}},
                               new Foo {Name = "f2", SomeType = EFoo.Foo, FooIds = new List<int>() { 1,2,3,4}},
                               new Foo {Name = "f3", SomeType = EFoo.Bar, FooIds = new List<int>() { 1,2,3,4}},
                           };

            var foos2 = PclValueInjecter.Mapper.Map<IEnumerable<Foo>, IList<Foo>>(foos);
            Assert.AreEqual(3, foos2.Count());
            Assert.AreEqual("f1", foos2.First().Name);
            Assert.AreEqual("f2", foos2.Skip(1).First().Name);
            Assert.AreEqual("f3", foos2.Last().Name);

            Assert.AreEqual(EFoo.Bar, foos2.First().SomeType);
            Assert.AreEqual(EFoo.Foo, foos2.Skip(1).First().SomeType);
            Assert.AreEqual(EFoo.Bar, foos2.Last().SomeType);

            for (var i = 0; i < foos2.Count(); i++)
            {
                for (int j = 0; j < foos2[i].FooIds.Count(); j++)
                {
                    Assert.AreEqual(foos2[i].FooIds[j], foos[i].FooIds[j]);
                }
            }
        }

        [Test]
        public void MapShouldMapCollectionTypeProperties()
        {
            var foo = new Foo
            {
                Foos = new List<Foo>
                           {
                               new Foo{Name = "f1", SomeType = EFoo.Bar},
                               new Foo{Name = "f2", SomeType = EFoo.Foo},
                               new Foo{Name = "f3", SomeType = EFoo.Bar},
                           }
            };

            var bar = PclValueInjecter.Mapper.Map<Foo, Bar>(foo);

            Assert.AreEqual(foo.Foos.Count(), bar.Foos.Count());
            Assert.AreEqual("f1", bar.Foos.First().Name);
            Assert.AreEqual("f2", bar.Foos.Skip(1).First().Name);
            Assert.AreEqual("f3", bar.Foos.Last().Name);

            Assert.AreEqual(EBar.Bar, bar.Foos.First().SomeType);
            Assert.AreEqual(EBar.Foo, bar.Foos.Skip(1).First().SomeType);
            Assert.AreEqual(EBar.Bar, bar.Foos.Last().SomeType);
        }

        [Test]
        public void MapPlainLists()
        {
            var a = new List<int>() { 1, 2, 3, 4 };

            var b = PclValueInjecter.Mapper.Map<IEnumerable<int>, IList<int>>(a);

            var a1 = new List<double>() { 1.0, 2.0, 3.0, 4.0 };

            var b1 = PclValueInjecter.Mapper.Map<IEnumerable<double>, IList<double>>(a1);

            var c = new List<string>() { "a", "b", "c", "d" };

            var d = PclValueInjecter.Mapper.Map<IEnumerable<string>, IList<string>>(c);
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
