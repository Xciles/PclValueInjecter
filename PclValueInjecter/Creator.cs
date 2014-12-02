using System;
using System.Collections.Generic;
using System.Reflection;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter
{
    internal static class Creator
    {
        internal static object Create(Type type)
        {
            if (type.IsEnumerable())
            {
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GetTypeInfo().GenericTypeArguments[0]));
            }

            if (type.GetTypeInfo().IsInterface)
                throw new Exception("don't know any implementation of this type: " + type.Name);

            return Activator.CreateInstance(type);
        }
    }
}