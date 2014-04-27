namespace Xciles.PclValueInjecter
{
    public interface IValueInjection
    {
        object Map(object source, object target);
    }
}