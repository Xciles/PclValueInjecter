using System;
using NUnit.Framework;
using Omu.ValueInjecter;

namespace Tests
{
    public class StepByStep
    {
        public class Source
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Target
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void DefaultInjection() // inject properties which are of the same name and type
        {
            var s = new Source { Id = 3, Name = "Bill" };
            var t = new Target();

            t.InjectFrom(s);

            Console.WriteLine(t.Id); // 3
            Console.WriteLine(t.Name);// Bill

            //we can also inject from multiple sources at the same time
            t.InjectFrom(new { Id = 75 }, new { Name = "Xavier" });
            Console.WriteLine(t.Id);//75
            Console.WriteLine(t.Name);//Xavier
        }

        public class Foo
        {
            public int Id { get; set; }
        }

        public class Bar
        {
            public int Id { get; set; }
        }

        public class FooBar
        {
            public int Foo_Id { get; set; }
            public int Bar_Id { get; set; }
        }

        public class IdToType_Id : ConventionInjection
        {
            protected override bool Match(ConventionInfo c)
            {
                return c.SourceProp.Name == "Id" && c.TargetProp.Name == c.Source.Type.Name + "_Id";
            }
        }

        [Test]
        public void Test()
        {
            var foo = new Foo { Id = 3 };
            var bar = new Bar { Id = 7 };

            var foobar = new FooBar();
            foobar.InjectFrom<IdToType_Id>(foo, bar);

            Console.WriteLine(foobar.Foo_Id);//3
            Console.WriteLine(foobar.Bar_Id);//7
        }


        public class Forward
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
            public long Number { get; set; }
        }

        public class Backward
        {
            public int dI { get; set; }
            public string emaN { get; set; }
            public string noitacoL { get; set; }
            public long rebmuN { get; set; }
        }

        public class Reverse : ConventionInjection
        {
            protected override bool Match(ConventionInfo c)
            {
                return c.SourceProp.Name == ReverseString(c.TargetProp.Name);
            }

            private string ReverseString(string s)
            {
                var arr = s.ToCharArray();
                Array.Reverse(arr);
                return new string(arr);
            }
        }

        [Test]
        public void BackwardTest()
        {
            var f = new Forward { Id = 33, Name = "phil", Location = "here", Number = 12345 };
            var b = new Backward();
            b.InjectFrom<Reverse>(f);

            Console.WriteLine(b.dI); // 33
            Console.WriteLine(b.emaN);// phil
            Console.WriteLine(b.noitacoL);// here
            Console.WriteLine(b.rebmuN);// 12345
        }

        public class Words
        {
            public int Word1 { get; set; }
            public int Word2 { get; set; }
            public int Word3 { get; set; }
            public int Word0 { get; set; }
            public double Word { get; set; }
        }

        public class StrToInt : ConventionInjection
        {
            protected override bool Match(ConventionInfo c)
            {
                return c.SourceProp.Name == c.TargetProp.Name &&
                       c.SourceProp.Type == typeof(string) && c.TargetProp.Type == typeof(int) &&
                       c.SourceProp.Value != null && 
                       c.SourceProp.Value.ToString() != "abc";
            }

            protected override object SetValue(ConventionInfo c)
            {
                return c.SourceProp.Value.ToString().Length;
            }
        }

        [Test]
        public void WordsTest()
        {
            var s = new { Word1 = "hi", Word2 = "how", Word3 = "how are you?", Word0 = "abc", Word = "oO" };
            var w = new Words {Word1 = 935, Word0 = 500, Word = 300};
            w.InjectFrom<StrToInt>(s);

            Console.WriteLine(w.Word1);//2 (hi.Length)
            Console.WriteLine(w.Word2);//3  
            Console.WriteLine(w.Word3);//12
            Console.WriteLine(w.Word0);//500 (ignored because s.Word0 was "abc" )
            Console.WriteLine(w.Word);//300 (ignored because Words.Word is of type double)
        }
    }

}
