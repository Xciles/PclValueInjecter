using Xciles.PclValueInjecter.CustomInjections;

namespace Xciles.PclValueInjecter
{
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
}