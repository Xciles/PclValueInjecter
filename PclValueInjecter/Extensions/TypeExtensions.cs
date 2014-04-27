using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Xciles.PclValueInjecter.Extensions
{
    public static class TypeExtensions
    {
        public static Boolean IsAnonymousType(this Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
            var nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }

        public static bool IsEnumerable(this Type type)
        {
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
                    return true;
            }
            return false;
        }
    }
}