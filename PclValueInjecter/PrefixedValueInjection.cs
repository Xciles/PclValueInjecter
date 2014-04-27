using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter
{
    public abstract class PrefixedValueInjection : LoopValueInjectionBase
    {
        protected string TargetPref;

        protected string SourcePref;

        public PrefixedValueInjection TargetPrefix(string prefix)
        {
            TargetPref = prefix;
            return this;
        }

        public PrefixedValueInjection SourcePrefix(string prefix)
        {
            SourcePref = prefix;
            return this;
        }

        /// <summary>
        /// get a string representing the target property name using the source property name and prefixes
        /// </summary>
        /// <param name="s">source property original name</param>
        /// <returns></returns>
        protected string SearchTargetName(string s)
        {
            return TargetPref + s.RemovePrefix(SourcePref);
        }

        /// <summary>
        /// get a string representing the source property name using the target property name and prefixes
        /// </summary>
        /// <param name="s">target property original name</param>
        /// <returns></returns>
        protected string SearchSourceName(string s)
        {
            return SourcePref + s.RemovePrefix(TargetPref);
        }
    }
}