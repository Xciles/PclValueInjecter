namespace Xciles.PclValueInjecter
{
    public class ValueInjecter : IValueInjecter
    {
        /// <summary>
        /// inject values from source to target
        /// </summary>
        /// <typeparam name="T">ValueInjection used </typeparam>
        /// <param name="target">target where the values is going to be injected</param>
        /// <param name="source">source from where the values are taken</param>
        /// <returns>the modified target</returns>
        public object Inject<T>(object target, params object[] source) where T : IValueInjection, new()
        {
            foreach (var o in source)
                target = new T().Map(o, target);
            return target;
        }

        /// <summary>
        /// inject values from source to target
        /// </summary>
        /// <param name="injection">the injection used</param>
        /// <param name="target">target where the values is going to be injected</param>
        /// <param name="source">source from where the values are taken</param>
        /// <returns>the modified target</returns>
        public object Inject(IValueInjection injection, object target, params object[] source)
        {
            foreach (var o in source)
                target = injection.Map(o, target);
            return target;
        }

        /// <summary>
        /// inject values into the target
        /// </summary>
        /// <typeparam name="T">ValueInjection used </typeparam>
        /// <param name="target">target where the values is going to be injected</param>
        /// <returns>the modified target</returns>
        public object Inject<T>(object target) where T : INoSourceValueInjection, new()
        {
            return new T().Map( target);
        }

        /// <summary>
        /// inject values into the target
        /// </summary>
        /// <param name="injection">ValueInjection used</param>
        /// <param name="target">target where the values is going to be injected</param>
        /// <returns>the modified target</returns>
        public object Inject(INoSourceValueInjection injection, object target)
        {
            return injection.Map(target);
        }

        /// <summary>
        /// the same as calling Inject&lt;LoopValueInjection>()
        /// </summary>
        public object Inject(object target, params object[] source)
        {
            return Inject<SameNameType>(target, source);
        }
    }
}