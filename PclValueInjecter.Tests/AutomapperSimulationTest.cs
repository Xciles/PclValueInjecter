using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Omu.ValueInjecter;

namespace Tests
{
    public class AutomapperSimulationTest
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
            MapperFactory.ClearMappers();
        }

        [Test]
        public void MapShouldMapToExistingObject()
        {
            var f2 = new Foo { Xyz = 43 };
            Mapper.Map(new { Name = "hi" }, f2);

            Assert.AreEqual("hi", f2.Name);
            Assert.AreEqual(43, f2.Xyz);
        }

        [Test]
        public void MapShouldCreateNewMappedObject()
        {
            var foo = new Foo { Name = "f1", Props = "p", Xyz = 123 };
            var foo2 = Mapper.Map<Foo, Foo>(foo);

            Assert.AreEqual(foo.Name, foo2.Name);
            Assert.AreEqual(foo.Props, foo2.Props);
            Assert.AreEqual(foo.Xyz, foo2.Xyz);
        }

        [Test]
        public void ShouldMapChildPropertiesFooToBar()
        {
            var foo = new Foo { Child = new Foo { Name = "aaa" } };
            var bar = new Bar();

            Mapper.Map(foo, bar);

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

            var foos2 = Mapper.Map<IEnumerable<Foo>, IList<Foo>>(foos);
            Assert.AreEqual(3, foos2.Count());
            Assert.AreEqual("f1", foos2.First().Name);
            Assert.AreEqual("f2", foos2.Skip(1).First().Name);
            Assert.AreEqual("f3", foos2.Last().Name);
        }

        public class FooBar : TypeMapper<Foo, Bar>
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
            MapperFactory.AddMapper(new FooBar());
            var foo = new Foo { Name = "a", Props = "b", Xyz = 123 };
            var bar = new Bar();

            Mapper.Map(foo, bar);

            Assert.AreEqual("a123b", bar.NoConvention);
            Assert.AreEqual(foo.Name, bar.Name);
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

        [Test]
        public void MapShouldMapCollectionPropertiesAndUseFooBarTypeMapper()
        {
            MapperFactory.AddMapper(new FooBar());
            var foo = new Foo
            {
                Foos = new List<Foo>
                           {
                               new Foo{Name = "f1",Props = "v",Xyz = 19},
                               new Foo{Name = "f2",Props = "i",Xyz = 7},
                               new Foo{Name = "f3",Props = "v",Xyz = 3},
                           }
            };

            var bar = Mapper.Map<Foo, Bar>(foo);

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

    // the static mapper class
    public static class Mapper
    {
        //map source to an existing target
        public static TTarget Map<TSource, TTarget>(TSource source, TTarget target)
        {
            target = MapperFactory.GetMapper<TSource, TTarget>().Map(source, target);
            return target;
        }

        //create a new target and map source on it 
        public static TTarget Map<TSource, TTarget>(TSource source)
        {
            var target = (TTarget)Creator.Create(typeof(TTarget));
            return MapperFactory.GetMapper<TSource, TTarget>().Map(source, target);
        }

        public static object Map(object source, object target, Type sourceType, Type targetType)
        {
            target = target ?? Creator.Create(targetType);
            var getMapper = typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(sourceType, targetType);
            var mapper = getMapper.Invoke(null, null);
            var map = mapper.GetType().GetMethod("Map");
            return map.Invoke(mapper, new[] { source, target });
        }
    }

    public static class MapperFactory
    {
        private static readonly IDictionary<Type, object> Mappers = new Dictionary<Type, object>();

        public static ITypeMapper<TSource, TTarget> GetMapper<TSource, TTarget>()
        {
            //if we have a specified TypeMapper for <TSource,Target> return it
            if (Mappers.ContainsKey(typeof(ITypeMapper<TSource, TTarget>)))
                return Mappers[typeof(ITypeMapper<TSource, TTarget>)] as ITypeMapper<TSource, TTarget>;

            //if both Source and Target types are Enumerables return new EnumerableTypeMapper<TSource,TTarget>()
            if (typeof(TSource).IsEnumerable() && typeof(TTarget).IsEnumerable())
            {
                return (ITypeMapper<TSource, TTarget>)Activator.CreateInstance(typeof(EnumerableTypeMapper<,>).MakeGenericType(typeof(TSource), typeof(TTarget)));
            }

            //return the default TypeMapper
            return new TypeMapper<TSource, TTarget>();
        }

        public static void AddMapper<TS, TT>(ITypeMapper<TS, TT> o)
        {
            Mappers.Add(typeof(ITypeMapper<TS, TT>), o);
        }

        public static void ClearMappers()
        {
            Mappers.Clear();
        }
    }

    public interface ITypeMapper<TSource, TTarget>
    {
        TTarget Map(TSource source, TTarget target);
    }

    public class TypeMapper<TSource, TTarget> : ITypeMapper<TSource, TTarget>
    {
        public virtual TTarget Map(TSource source, TTarget target)
        {
            target.InjectFrom(source)
                .InjectFrom<NullablesToNormal>(source)
                .InjectFrom<NormalToNullables>(source)
                .InjectFrom<IntToEnum>(source)
                .InjectFrom<EnumToInt>(source)
                .InjectFrom<MapperInjection>(source);// apply mapper.map for Foo, Bar, IEnumerable<Foo> etc.

            return target;
        }
    }

    public class EnumerableTypeMapper<TSource, TTarget> : ITypeMapper<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        public TTarget Map(TSource source, TTarget target)
        {
            if (source == null) return null;
            var targetArgumentType = typeof(TTarget).GetGenericArguments()[0];

            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(targetArgumentType));
            var add = list.GetType().GetMethod("Add");

            foreach (var o in source as IEnumerable)
            {
                var t = Creator.Create(targetArgumentType);
                add.Invoke(list, new[] { Mapper.Map(o, t, o.GetType(), targetArgumentType) });
            }
            return (TTarget)list;
        }
    }

    public static class TypeExtensions
    {
        //returns true if type is IEnumerable<> or ICollection<>, IList<> ...
        public static bool IsEnumerable(this Type type)
        {
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
                    return true;
            }
            return false;
        }
    }

    public static class Creator
    {
        public static object Create(Type type)
        {
            if(type.IsEnumerable())
            {
                return Activator.CreateInstance(typeof (List<>).MakeGenericType(type.GetGenericArguments()[0]));
            }

            if(type.IsInterface)
                throw new Exception("don't know any implementation of this type: "+ type.Name);

            return Activator.CreateInstance(type);
        }
    }

    public class MapperInjection : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                !c.SourceProp.Type.IsValueType && c.SourceProp.Type != typeof(string) &&
                !c.SourceProp.Type.IsGenericType && !c.TargetProp.Type.IsGenericType 
                ||
                 c.SourceProp.Type.IsEnumerable() &&
                   c.TargetProp.Type.IsEnumerable();
        }

        protected override object SetValue(ConventionInfo c)
        {
            if (c.SourceProp.Value == null) return null;
            return Mapper.Map(c.SourceProp.Value, c.TargetProp.Value, c.SourceProp.Type, c.TargetProp.Type);
        }
    }

    public class EnumToInt : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                c.SourceProp.Type.IsSubclassOf(typeof(Enum)) && c.TargetProp.Type == typeof(int);
        }
    }

    public class IntToEnum : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                c.SourceProp.Type == typeof(int) && c.TargetProp.Type.IsSubclassOf(typeof(Enum));
        }
    }

    //e.g. int? -> int
    public class NullablesToNormal : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                   Nullable.GetUnderlyingType(c.SourceProp.Type) == c.TargetProp.Type;
        }
    }

    //e.g. int -> int?
    public class NormalToNullables : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                   c.SourceProp.Type == Nullable.GetUnderlyingType(c.TargetProp.Type);
        }
    }
}