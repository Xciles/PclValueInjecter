using System;
using NUnit.Framework;
using Omu.ValueInjecter;

namespace Tests
{
    [TestFixture]
    public class JohnLandheer
    {
        [Test]
        public void Test()
        {
            var child = new Child
                            {
                                Id = 1,
                                Name = "John",
                                Mother = new Parent { Id = 3 },
                                Father = new Parent { Id = 9 },
                                Brother = new Child { Id = 5 },
                                Sister = new Child { Id = 7 }
                            };
            var childEdit = new ChildEdit();

            childEdit.InjectFrom(child)
                .InjectFrom<EntityToInt>(child);

            Assert.AreEqual(1, childEdit.Id);
            Assert.AreEqual("John", childEdit.Name);
            Assert.AreEqual(3, childEdit.MotherId);
            Assert.AreEqual(9, childEdit.FatherId);
            Assert.AreEqual(5, childEdit.BrotherId);
            Assert.AreEqual(7, childEdit.SisterId);
            Assert.AreEqual(0, childEdit.Sister2Id);

            var c = new Child();

            c.InjectFrom(childEdit)
                .InjectFrom<IntToEntity>(childEdit);

            Assert.AreEqual(1, c.Id);
            Assert.AreEqual("John", c.Name);
            Assert.AreEqual(3, c.Mother.Id);
            Assert.AreEqual(9, c.Father.Id);
            Assert.AreEqual(5, c.Brother.Id);
            Assert.AreEqual(7, c.Sister.Id);
            Assert.AreEqual(null, c.Sister2);
        }

        public class Entity
        {
            public int Id { get; set; }
        }

        public class Parent : Entity
        {
            public string Name { get; set; }
        }

        public class Child : Entity
        {
            public string Name { get; set; }
            public Parent Mother { get; set; }
            public Parent Father { get; set; }
            public Child Brother { get; set; }
            public Child Sister { get; set; }
            public Child Sister2 { get; set; }
        }

        public class ChildEdit
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int MotherId { get; set; }
            public int FatherId { get; set; }
            public int BrotherId { get; set; }
            public int SisterId { get; set; }
            public int Sister2Id { get; set; }
        }

        public class EntityToInt : ConventionInjection
        {
            protected override bool Match(ConventionInfo c)
            {
                return c.TargetProp.Name == c.SourceProp.Name + "Id" &&
                c.SourceProp.Type.IsSubclassOf(typeof (Entity)) && c.TargetProp.Type == typeof (int)
                && c.SourceProp.Value != null;
            }

            protected override object SetValue(ConventionInfo c)
            {
                return ((Entity) c.SourceProp.Value).Id;
            }

           
        }
        
        public class IntToEntity : LoopValueInjection
        {
            protected override bool TypesMatch(Type sourceType, Type targetType)
            {
                return sourceType == typeof(int) && targetType.IsSubclassOf(typeof(Entity));
            }

            protected override string TargetPropName(string sourcePropName)
            {
                return sourcePropName.RemoveSuffix("Id");
            }

            protected override bool AllowSetValue(object value)
            {
                return (int)value > 0;
            }

            protected override object SetValue(object v)
            {
                // you could as well do repoType = IoC.Resolve(typeof(IRepo<>).MakeGenericType(TargetPropType))
                var repoType =  typeof (Repo<>).MakeGenericType(TargetPropType);
                var repo = Activator.CreateInstance(repoType);
                return repoType.GetMethod("Get").Invoke(repo, new[] {v});
            }
        }

        class Repo<T> : IRepo<T> where T : Entity, new()
        {
            public T Get(int id)
            {
                return new T{Id = id};
            }
        }

        private interface IRepo<T>
        {
            T Get(int id);
        }
    }
}