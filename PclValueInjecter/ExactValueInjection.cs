namespace Xciles.PclValueInjecter
{
    ///<summary>
    ///</summary>
    public abstract class ExactValueInjection : CustomizableValueInjection
    {
        protected override void Inject(object source, object target)
        {
            var sp = source.GetProps().GetByName(sourcePref + SourceName());
            var tp = target.GetProps().GetByName(SearchTargetName(TargetName()));

            if (tp == null) return;

            if (!TypesMatch(sp.PropertyType, tp.PropertyType)) return;

            var value = sp.GetValue(source);
            if (AllowSetValue(value))
                tp.SetValue(target, SetValue(value));

        }

        public abstract string SourceName();

        public virtual string TargetName()
        {
            return SourceName();
        }
    }
}