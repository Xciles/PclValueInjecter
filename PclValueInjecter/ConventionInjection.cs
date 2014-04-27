using System;
using Xciles.PclValueInjecter.Extensions;

namespace Xciles.PclValueInjecter
{
    public abstract class ConventionInjection : ValueInjection
    {
        protected abstract bool Match(ConventionInfo c);

        protected virtual object SetValue(ConventionInfo c)
        {
            return c.SourceProp.Value;
        }

        protected override void Inject(object source, object target)
        {
            var sourceProps = source.GetProps();
            var targetProps = target.GetProps();

            var ci = new ConventionInfo
                         {
                             Source =
                                 {
                                     Type = source.GetType(), 
                                     Value = source
                                 },
                             Target =
                                 {
                                     Type = target.GetType(),
                                     Value = target
                                 }
                         };

            for (var i = 0; i < sourceProps.Count; i++)
            {
                var s = sourceProps[i];
                ci.SourceProp.Name = s.Name;
                ci.SourceProp.Value = s.GetValue(source);
                ci.SourceProp.Type = s.PropertyType;

                for(var j=0; j < targetProps.Count; j++)
                {
                    var t = targetProps[j];
                    ci.TargetProp.Name = t.Name;
                    ci.TargetProp.Value = t.GetValue(target);
                    ci.TargetProp.Type = t.PropertyType;
                    if (Match(ci))
                        t.SetValue(target, SetValue(ci));
                }
            }
        }
    }

    public class ConventionInfo
    {
        public ConventionInfo()
        {
            Source = new TypeInfo();
            Target = new TypeInfo();
            SourceProp = new PropInfo();
            TargetProp = new PropInfo();
        }

        public TypeInfo Source { get; set; }
        public TypeInfo Target { get; set; }

        public PropInfo SourceProp { get; set; }
        public PropInfo TargetProp { get; set; }

        public class PropInfo
        {
            public string Name { get; set; }
            public Type Type { get; set; }
            public object Value { get; set; }
        }

        public class TypeInfo
        {
            public Type Type { get; set; }
            public object Value { get; set; }
        }
    }
}