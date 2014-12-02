using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Xciles.PclValueInjecter.Extensions
{
    public static class TypeExtensions
    {
        public static Boolean IsAnonymousType(this Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetTypeInfo().GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            var nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }

        public static bool IsEnumerable(this Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                if (type.GetGenericTypeDefinition().GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerable)))
                    return true;
            }
            return false;
        }
    }
}