namespace Xciles.PclValueInjecter
{
    public static class StaticValueInjecter
    {
        /// <summary>
        /// Injects values from source to target
        /// </summary>
        /// <typeparam name="T">ValueInjection used</typeparam>
        /// <param name="target">target where the value is going to be injected</param>
        /// <param name="source">source from where the value is taken</param>
        /// <returns>the modified target</returns>
        public static object InjectFrom<T>(this object target, params object[] source)
            where T : IValueInjection, new()
        {
            foreach (var o in source)
               target = new T().Map(o, target);
            return target;
        }

        /// <summary>
        /// Injects values from source to target
        /// </summary>
        /// <param name="target">target where the value is going to be injected</param>
        /// <param name="injection">ValueInjection used</param>
        /// <param name="source">source from where the value is taken</param>
        /// <returns>the modified target</returns>
        public static object InjectFrom(this object target, IValueInjection injection, params object[] source)
        {
            foreach (var o in source)
                target = injection.Map(o, target);
            return target;
        }

        /// <summary>
        /// Injects values into the target
        /// </summary>
        /// <typeparam name="T">ValueInjection(INoSourceValueInjection) used for that</typeparam>
        /// <param name="target">target where the value is going to be injected</param>
        /// <returns>the modified target</returns>
        public static object InjectFrom<T>(this object target) where T : INoSourceValueInjection, new()
        {
            return new T().Map(target);
        }

        /// <summary>
        /// Injects value into target without source
        /// </summary>
        /// <param name="target">the target where the value is going to be injected</param>
        /// <param name="injection"> the injection(INoSourceValueInjection) used to inject value</param>
        /// <returns>the modified target</returns>
        public static object InjectFrom(this object target, INoSourceValueInjection injection)
        {
            return injection.Map(target);
        }

        /// <summary>
        /// it the same as calling InjectFrom&lt;LookupValueInjection&gt;()
        /// </summary>
        public static object InjectFrom(this object target, params object[] source)
        {
            return InjectFrom<SameNameType>(target, source);
        }
    }
}