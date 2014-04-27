using System;
using System.Runtime.CompilerServices;

namespace Xciles.PclValueInjecter
{
    public static class Extensions
    {
        public static string RemovePrefix(this string o, string prefix)
        {
            return o.RemovePrefix(prefix, StringComparison.Ordinal);
        }

        public static string RemovePrefix(this string o, string prefix, StringComparison comparison)
        {
            if (prefix == null) return o;
            return !o.StartsWith(prefix, comparison) ? o : o.Remove(0, prefix.Length);
        }

        public static string RemoveSuffix(this string o, string suffix)
        {
            if(suffix == null) return o;
            return !o.EndsWith(suffix) ? o : o.Remove(o.Length - suffix.Length, suffix.Length);
        }

        public static Boolean IsAnonymousType(this Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
            var nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }
    }
}