using System;
using NUnit.Framework;
using Omu.ValueInjecter;
using Tests;

namespace Tests
{
    public class CountryDTO
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
    }



    }

    public class SixFootUnderTest
    {
        [Test]
        public void Test()
        {
            var o = new SimpleClass1
                        {
                            CountryRaw = "United States",
                            GenderRaw = "Female",
                            Person = new Person { GenderRaw = "Male" }
                        };

            var oo = new SimpleClass1();

            oo.InjectFrom(o)
                .InjectFrom<StrRawToEnum>(o);
            oo.Person.InjectFrom<StrRawToEnum>(o.Person);

            oo.Country.IsEqualTo(Country.UnitedStates);
            oo.Gender.IsEqualTo(Gender.Female);
            oo.Person.Gender.IsEqualTo(Gender.Male);
        }

        public class SimpleClass1
        {
            public string CountryRaw { get; set; }

            public Country Country { get; set; }

            public string GenderRaw { get; set; }

            public Gender Gender { get; set; }

            public Person Person { get; set; }
        }

        public class Person
        {
            public string GenderRaw { get; set; }

            public Gender Gender { get; set; }

            public string Surname { get; set; }
        }


        public class StrRawToEnum : LoopValueInjection
        {
            protected override bool UseSourceProp(string sourcePropName)
            {
                return sourcePropName.EndsWith("Raw");
            }

            protected override string TargetPropName(string sourcePropName)
            {
                return sourcePropName.RemoveSuffix("Raw");
            }

            protected override bool TypesMatch(Type sourceType, Type targetType)
            {
                return sourceType == typeof(string) && targetType.IsEnum;
            }

            protected override object SetValue(object v)
            {
                return Enum.Parse(TargetPropType, v.ToString().Replace(" ", ""), true);
            }
        }

        public enum Country
        {
            UnitedStates = 1,
            NewZealand = 2
        }


        public enum Gender
        {
            Male,
            Female,
            Unknown
        }
    }
