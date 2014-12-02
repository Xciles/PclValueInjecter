using System;
using System.Reflection;

namespace Xciles.PclValueInjecter.CustomInjections
{
    public class EnumsByStringName : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name
                && c.SourceProp.Type.GetTypeInfo().IsEnum
                && c.TargetProp.Type.GetTypeInfo().IsEnum;
        }

        protected override object SetValue(ConventionInfo c)
        {
            return Enum.Parse(c.TargetProp.Type, c.SourceProp.Value.ToString(), true);
        }
    }
}
