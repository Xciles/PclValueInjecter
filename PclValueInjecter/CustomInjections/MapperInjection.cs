using System.Reflection;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter.CustomInjections
{
    public class MapperInjection : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return (c.SourceProp.Name == c.TargetProp.Name &&
                   !c.SourceProp.Type.GetTypeInfo().IsValueType && c.SourceProp.Type != typeof(string) &&
                   !c.SourceProp.Type.GetTypeInfo().IsGenericType && !c.TargetProp.Type.GetTypeInfo().IsGenericType)
                   ||
                   (c.SourceProp.Type.IsEnumerable() &&
                   c.TargetProp.Type.IsEnumerable() && c.SourceProp.Name == c.TargetProp.Name);
        }

        protected override object SetValue(ConventionInfo c)
        {
            if (c.SourceProp.Value == null) return null;
            return Mapper.Map(c.SourceProp.Value, c.TargetProp.Value, c.SourceProp.Type, c.TargetProp.Type);
        }
    }
}