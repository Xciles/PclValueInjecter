using System;
using System.Linq;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter
{
    public class UnflatLoopValueInjection : LoopValueInjectionBase
    {
        protected override void Inject(object source, object target)
        {
            foreach (PropertyDescriptor sourceProp in source.GetProps())
            {
                var prop = sourceProp;
                var endpoints = UberFlatter.Unflat(sourceProp.Name, target, t => TypesMatch(prop.PropertyType, t));

                if(endpoints.Count() == 0) continue;

                var value = sourceProp.GetValue(source);

                if (AllowSetValue(value))
                    foreach (var endpoint in endpoints)
                        endpoint.Property.SetValue(endpoint.Component, SetValue(value));
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

    public abstract class UnflatLoopValueInjection<TSourceProperty, TTargetProperty> : LoopValueInjectionBase
    {
        protected override void Inject(object source, object target)
        {
            foreach (PropertyDescriptor sourceProp in source.GetProps())
            {
                if(sourceProp.PropertyType != typeof(TSourceProperty)) continue;
                var endpoints = UberFlatter.Unflat(sourceProp.Name, target, t => t == typeof(TTargetProperty));
                if (endpoints.Count() == 0) continue;
                var value = sourceProp.GetValue(source);

                if (AllowSetValue(value))
                    foreach (var endpoint in endpoints)
                        endpoint.Property.SetValue(endpoint.Component, SetValue((TSourceProperty)value));
            }
        }

        protected abstract TTargetProperty SetValue(TSourceProperty sourcePropertyValue);

    }
}