using System;
using System.Collections.Generic;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter
{
    public static class Tunnelier
    {
        public static PropertyWithComponent Digg(IList<string> trail, object o)
        {
            if (trail.Count == 1)
            {
                var prop = o.GetProps().GetByName(trail[0]);
                return new PropertyWithComponent { Component = o, Property = prop };
            }
            else
            {
                var prop = o.GetProps().GetByName(trail[0]);

                if (prop.GetValue(o) == null) prop.SetValue(o, Activator.CreateInstance(prop.PropertyType));

                var val = prop.GetValue(o);

                trail.RemoveAt(0);
                return Digg(trail, val);
            }
        }

        public static PropertyWithComponent GetValue(IList<string> trail, object o)
        {
            if (trail.Count == 1)
            {
                var prop = o.GetProps().GetByName(trail[0]);
                return new PropertyWithComponent { Component = o, Property = prop };
            }

            var propx = o.GetProps().GetByName(trail[0]);
            var val = propx.GetValue(o);
            if (val == null) return null;
            trail.RemoveAt(0);
            return GetValue(trail, val);
        }
    }
}