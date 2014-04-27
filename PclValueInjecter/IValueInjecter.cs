namespace Xciles.PclValueInjecter
{
    public interface IValueInjecter
    {
        object Inject<T>(object target, params object[] source) where T : IValueInjection, new();
        object Inject(IValueInjection injection, object target, params object[] source);
        object Inject<T>(object target) where T : INoSourceValueInjection, new();
        object Inject(INoSourceValueInjection injection, object target);
        object Inject(object target, params object[] source);
    }
}