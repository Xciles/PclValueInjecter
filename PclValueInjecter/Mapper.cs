using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xciles.PclValueInjecter.CustomInjections;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter
{
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

    public static class Creator
    {
        public static object Create(Type type)
        {
            if (type.IsEnumerable())
            {
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GetGenericArguments()[0]));
            }

            if (type.IsInterface)
                throw new Exception("don't know any implementation of this type: " + type.Name);

            return Activator.CreateInstance(type);
        }
    }

}
