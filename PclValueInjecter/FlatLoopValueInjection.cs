using System;
using System.Linq;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter
{
    public class FlatLoopValueInjection : LoopValueInjectionBase
    {
        protected override void Inject(object source, object target)
        {
            foreach (PropertyDescriptor t in target.GetProps())
            {
                var t1 = t;
                var es = UberFlatter.Flat(t.Name, source, type => TypesMatch(type, t1.PropertyType));

                if (es.Count() == 0) continue;
                var endpoint = es.First();
                if(endpoint == null) continue;
                var val = endpoint.Property.GetValue(endpoint.Component);

                if (AllowSetValue(val))
                    t.SetValue(target, SetValue(val));
            }
        }

        protected virtual bool TypesMatch(Type sourceType, Type targetType)
        {
            return targetType == sourceType;
        }

        protected virtual object SetValue(object sourcePropertyValue)
        {
            return sourcePropertyValue;
        }
    }


    public abstract class FlatLoopValueInjection<TSourceProperty, TTargetProperty> : LoopValueInjectionBase
    {
        protected override void Inject(object source, object target)
        {
            foreach (PropertyDescriptor t in target.GetProps())
            {
                if (t.PropertyType != typeof(TTargetProperty)) continue;

                var values = UberFlatter.Flat(t.Name, source, type => type == typeof(TSourceProperty));

                if (values.Count() == 0) continue;

                var val = values.First().Property.GetValue(values.First().Component);

                if (AllowSetValue(val))
                    t.SetValue(target, SetValue((TSourceProperty)val));
            }
        }

        protected abstract TTargetProperty SetValue(TSourceProperty sourceValues);

    }
}