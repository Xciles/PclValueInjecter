using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xciles.PclValueInjecter
{
    public static class TrailFinder
    {
        public static IEnumerable<IList<string>> GetTrails(string upn, IEnumerable<PropertyInfo> all, Func<Type, bool> f, StringComparison comparison)
        {
            return all.SelectMany(p => GetTrails(upn, p, f, new List<string>(), comparison));
        }

        public static IEnumerable<IList<string>> GetTrails(string upn, PropertyInfo prop, Func<Type, bool> f, IList<string> root, StringComparison comparison)
        {
            if (string.Equals(upn, prop.Name, comparison) && f(prop.PropertyType))
            {
                var l = new List<string> { prop.Name };
                yield return l;
                yield break;
            }

            if (upn.StartsWith(prop.Name, comparison))
            {
                root.Add(prop.Name);
                foreach (var pro in prop.PropertyType.GetInfos())
                {
                    foreach (var trail in GetTrails(upn.RemovePrefix(prop.Name, comparison), pro, f, root, comparison))
                    {
                        var r = new List<string> { prop.Name };
                        r.AddRange(trail);
                        yield return r;
                    }
                }
            }
        }
    }
}