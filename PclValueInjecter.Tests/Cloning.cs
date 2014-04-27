using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Omu.ValueInjecter;

namespace Tests
{
    public class Cloning
    {
        public class Foo
        {
            public string Name { get; set; }
            public int Number { get; set; }
            public int? NullInt { get; set; }
            public Foo F1 { get; set; }
            public IEnumerable<Foo> Foos { get; set; }
            public Foo[] FooArr { get; set; }
            public int[] IntArr { get; set; }
            public IEnumerable<int> Ints { get; set; }
        }

        [Test]
        public void Test()
        {
            var o = new Foo
                        {
                            Name = "foo",
                            Number = 12,
                            NullInt = 16,
                            F1 = new Foo { Name = "foo one" },
                            Foos = new List<Foo>
                                       {
                                           new Foo {Name = "j1"},
                                           new Foo {Name = "j2"},
                                       },
                            FooArr = new Foo[]
                                         {
                                             new Foo {Name = "a1"},
                                             new Foo {Name = "a2"},
                                             new Foo {Name = "a3"},
                                         },
                            IntArr = new[] { 1, 2, 3, 4, 5 },
                            Ints = new[] { 7, 8, 9 },
                        };

            var c = new Foo().InjectFrom<CloneInjection>(o) as Foo;

            Assert.AreEqual(o.Name, c.Name);
            Assert.AreEqual(o.Number, c.Number);
            Assert.AreEqual(o.NullInt, c.NullInt);
            Assert.AreEqual(o.IntArr, c.IntArr);
            Assert.AreEqual(o.Ints, c.Ints);

            Assert.AreNotEqual(o.F1, c.F1);
            Assert.AreNotEqual(o.Foos, c.Foos);
            Assert.AreNotEqual(o.FooArr, c.FooArr);

            //Foo F1
            Assert.AreEqual(o.F1.Name, c.F1.Name);

            //Foo[] FooArr
            Assert.AreEqual(o.FooArr.Length, c.FooArr.Length);
            Assert.AreNotEqual(o.FooArr[0], c.FooArr[0]);
            Assert.AreEqual(o.FooArr[0].Name, c.FooArr[0].Name);

            //IEnumerable<Foo> Foos
            Assert.AreEqual(o.Foos.Count(), c.Foos.Count());
            Assert.AreNotEqual(o.Foos.First(), c.Foos.First());
            Assert.AreEqual(o.Foos.First().Name, c.Foos.First().Name);
        }

        public class CloneInjection : ConventionInjection
        {
            protected override bool Match(ConventionInfo c)
            {
                return c.SourceProp.Name == c.TargetProp.Name && c.SourceProp.Value != null;
            }

            protected override object SetValue(ConventionInfo c)
            {
                //for value types and string just return the value as is
                if (c.SourceProp.Type.IsValueType || c.SourceProp.Type == typeof(string))
                    return c.SourceProp.Value;

                //handle arrays
                if (c.SourceProp.Type.IsArray)
                {
                    var arr = c.SourceProp.Value as Array;
                    var clone = arr.Clone() as Array;

                    for (int index = 0; index < arr.Length; index++)
                    {
                        var a = arr.GetValue(index);
                        if (a.GetType().IsValueType || a.GetType() == typeof(string)) continue;
                        clone.SetValue(Activator.CreateInstance(a.GetType()).InjectFrom<CloneInjection>(a), index);
                    }
                    return clone;
                }


                if (c.SourceProp.Type.IsGenericType)
                {
                    //handle IEnumerable<> also ICollection<> IList<> List<>
                    if (c.SourceProp.Type.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
                    {
                        var t = c.SourceProp.Type.GetGenericArguments()[0];
                        if (t.IsValueType || t == typeof (string)) return c.SourceProp.Value;

                        var tlist = typeof(List<>).MakeGenericType(t);
                        var list = Activator.CreateInstance(tlist);

                        var addMethod = tlist.GetMethod("Add");
                        foreach (var o in c.SourceProp.Value as IEnumerable)
                        {
                            var e = Activator.CreateInstance(t).InjectFrom<CloneInjection>(o);
                            addMethod.Invoke(list, new[] { e }); // in 4.0 you can use dynamic and just do list.Add(e);
                        }
                        return list;
                    }

                    //unhandled generic type, you could also return null or throw
                    return c.SourceProp.Value;
                }

                //for simple object types create a new instace and apply the clone injection on it
                return Activator.CreateInstance(c.SourceProp.Type)
                    .InjectFrom<CloneInjection>(c.SourceProp.Value);
            }
        }
        
    }
}