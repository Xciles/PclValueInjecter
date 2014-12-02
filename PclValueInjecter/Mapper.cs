using System;
using System.Reflection;

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
            var getMapper = typeof(MapperFactory).GetTypeInfo().GetDeclaredMethod("GetMapper").MakeGenericMethod(sourceType, targetType);
            var mapper = getMapper.Invoke(null, null);
            var map = mapper.GetType().GetTypeInfo().GetDeclaredMethod("Map");
            return map.Invoke(mapper, new[] { source, target });
        }
    }
}
