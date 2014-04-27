using System.Reflection;

namespace Xciles.PclValueInjecter.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static object GetValue(this PropertyInfo propertyInfo, object obj)
        {
            return propertyInfo.GetValue(obj, null);
        }

        public static void SetValue(this PropertyInfo propertyInfo, object obj, object value)
        {
            propertyInfo.SetValue(obj, value, null);
        }
    }
}