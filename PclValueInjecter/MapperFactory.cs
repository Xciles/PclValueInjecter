using System;
using System.Collections.Generic;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter
{
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

        public static void AddMapper<TSource, TTarget>(ITypeMapper<TSource, TTarget> o)
        {
            Mappers.Add(typeof(ITypeMapper<TSource, TTarget>), o);
        }

        public static void ClearMappers()
        {
            Mappers.Clear();
        }
    }
}