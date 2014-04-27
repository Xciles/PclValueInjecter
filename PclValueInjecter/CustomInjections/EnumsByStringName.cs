using System;

namespace Xciles.PclValueInjecter.CustomInjections
{
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
