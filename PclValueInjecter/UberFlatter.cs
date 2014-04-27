using System;
using System.Collections.Generic;
using System.Linq;

namespace Xciles.PclValueInjecter
{
    /// <summary>
    /// performs flattening and unflattening
    /// first version of this class was made by Vadim Plamadeala ☺
    /// </summary>
    public static class UberFlatter
    {
        public static IEnumerable<PropertyWithComponent> Unflat(string flatPropertyName, object target, Func<Type, bool> f, StringComparison comparison)
        {
            var trails = TrailFinder.GetTrails(flatPropertyName, target.GetType().GetInfos(), f, comparison);

            return trails.Select(trail => Tunnelier.Digg(trail, target));
        }

        public static IEnumerable<PropertyWithComponent> Unflat(string flatPropertyName, object target, Func<Type, bool> f)
        {
            return Unflat(flatPropertyName, target, f, StringComparison.Ordinal);
        }

        public static IEnumerable<PropertyWithComponent> Unflat(string flatPropertyName, object target)
        {
            return Unflat(flatPropertyName, target, o => true);
        }

        public static IEnumerable<PropertyWithComponent> Flat(string flatPropertyName, object source, Func<Type, bool> f)
        {
            return Flat(flatPropertyName, source, f, StringComparison.Ordinal);
        }

        public static IEnumerable<PropertyWithComponent> Flat(string flatPropertyName, object source, Func<Type, bool> f, StringComparison comparison)
        {
            var trails = TrailFinder.GetTrails(flatPropertyName, source.GetType().GetInfos(), f, comparison);

            return trails.Select(trail => Tunnelier.GetValue(trail, source));
        }

        public static IEnumerable<PropertyWithComponent> Flat(string flatPropertyName, object source)
        {
            return Flat(flatPropertyName, source, o => true);
        }
    }
}