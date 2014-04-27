using System;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter
{
    /// <summary>
    /// maps the properties with the same name from the source object of type TSourceProperty to the ones from the target object of type TTargetProperty
    /// </summary>
    /// <typeparam name="TSourceProperty">source properties type</typeparam>
    /// <typeparam name="TTargetProperty">target properties type</typeparam>
    public abstract class LoopValueInjection<TSourceProperty, TTargetProperty> : PrefixedValueInjection
    {
        protected override void Inject(object source, object target)
        {
            var targetProps = target.GetProps();
            var sourceProps = source.GetProps();

            for (var i = 0; i < source.GetProps().Count; i++)
            {
                var s = sourceProps[i];
                if (s.PropertyType != typeof(TSourceProperty)) continue;
                var t = targetProps.GetByNameType<TTargetProperty>(SearchTargetName(s.Name));
                if (t == null) continue;

                var value = s.GetValue(source);
                if (AllowSetValue(value))
                    t.SetValue(target, SetValue((TSourceProperty)value));
            }
        }

        protected abstract TTargetProperty SetValue(TSourceProperty sourcePropertyValue);
    }

    ///<summary>
    /// maps the properties with the same name and type(by default, but you can override TypesMatch to change this) from source to the ones in the target
    ///</summary>
    public class LoopValueInjection : CustomizableValueInjection
    {
        protected Type TargetPropType;
        protected Type SourcePropType;

        protected virtual bool UseSourceProp(string sourcePropName)
        {
            return true;
        }

        protected virtual string TargetPropName(string sourcePropName)
        {
            return sourcePropName;
        }

        protected override void Inject(object source, object target)
        {
            var sourceProps = source.GetProps();
            for (var i = 0; i < sourceProps.Count; i++)
            {
                var s = sourceProps[i];
                if (!UseSourceProp(s.Name)) continue;

                var t = target.GetProps().GetByName(SearchTargetName(TargetPropName(s.Name)));
                if (t == null) continue;
                if (!TypesMatch(s.PropertyType, t.PropertyType)) continue;
                TargetPropType = t.PropertyType;
                SourcePropType = s.PropertyType;
                var value = s.GetValue(source);
                if (AllowSetValue(value))
                    t.SetValue(target, SetValue(value));
            }
        }
    }
}