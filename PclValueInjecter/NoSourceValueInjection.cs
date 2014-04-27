namespace Xciles.PclValueInjecter
{
    ///<summary>
    /// inject value without source
    ///</summary>
    public abstract class NoSourceValueInjection : INoSourceValueInjection
    {
        public object Map(object target)
        {
            Inject(target);
            return target;
        }

        protected abstract void Inject(object target);
    }
}