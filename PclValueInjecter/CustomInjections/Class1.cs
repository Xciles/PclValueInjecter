using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter.CustomInjections
{
    public class NullablesToNormal : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                   Nullable.GetUnderlyingType(c.SourceProp.Type) == c.TargetProp.Type;
        }
    }

    public class NormalToNullables : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name &&
                   c.SourceProp.Type == Nullable.GetUnderlyingType(c.TargetProp.Type);
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

    public class EnumsByStringName : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name
                && c.SourceProp.Type.IsEnum
                && c.TargetProp.Type.IsEnum;
        }

        protected override object SetValue(ConventionInfo c)
        {
            return Enum.Parse(c.TargetProp.Type, c.SourceProp.Value.ToString(), true);
        }
    }
}
