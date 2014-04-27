namespace Xciles.PclValueInjecter
{
    public abstract class PrefixedValueInjection : LoopValueInjectionBase
    {
        protected string targetPref;

        protected string sourcePref;

        public PrefixedValueInjection TargetPrefix(string prefix)
        {
            targetPref = prefix;
            return this;
        }

        public PrefixedValueInjection SourcePrefix(string prefix)
        {
            sourcePref = prefix;
            return this;
        }

        /// <summary>
        /// get a string representing the target property name using the source property name and prefixes
        /// </summary>
        /// <param name="s">source property original name</param>
        /// <returns></returns>
        protected string SearchTargetName(string s)
        {
            return targetPref + s.RemovePrefix(sourcePref);
        }

        /// <summary>
        /// get a string representing the source property name using the target property name and prefixes
        /// </summary>
        /// <param name="s">target property original name</param>
        /// <returns></returns>
        protected string SearchSourceName(string s)
        {
            return sourcePref + s.RemovePrefix(targetPref);
        }
    }
}