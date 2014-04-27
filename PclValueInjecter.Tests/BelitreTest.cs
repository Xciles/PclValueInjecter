using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Xciles.PclValueInjecter.Tests
{
    public class BelitreTest
    {
        public class M1
        {
            public string Name { get; set; }
        }

        public class M2
        {
            public string Name { get; set; }
        }

        [Test]
        public void Cast()
        {
            object source = new List<M1> {new M1 {Name = "o"}};
            object target = new List<M2>();


            var targetArgumentType = target.GetType().GetGenericArguments()[0];

            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(targetArgumentType));
            var add = list.GetType().GetMethod("Add");

            foreach (var o in source as IEnumerable)
            {
                var t = Activator.CreateInstance(targetArgumentType);
                add.Invoke(list, new[] { t.InjectFrom(o) });
            }

            target = list;

            Assert.AreEqual("o", (target as List<M2>).First().Name);
        }

        public class Customer
        {
            public int CustomerId { get; set; }
            public string FirstName { get; set; }
            public string SecondName { get; set; }
            public IEnumerable<CustomerCountry> CustomerCountries { get; set; }
        }

        public class CustomerCountry
        {
            public int CountryId { get; set; }
            public int PayLoad { get; set; }
        }

        public class Country
        {
            public int CountryId { get; set; }
            public string Name { get; set; }
        }

        public class CustomerDto
        {
            public int CustomerId { get; set; }
            public string FirstName { get; set; }
            public string SecondName { get; set; }
            public IEnumerable<CountryDTO> Countries { get; set; }
        }

        [Test]
        public void Aaa()
        {
            var x = Activator.CreateInstance(typeof (int));
            Console.WriteLine(x);
        }

        [Test]
        public void Doit()
        {
            var c = new Customer
                        {
                            CustomerId = 3,
                            FirstName = "fn",
                            SecondName = "sn",
                            CustomerCountries =
                                new[] { new CustomerCountry { CountryId = 1 }, new CustomerCountry { CountryId = 2 } }
                        };

            var countries = new[]
                                {
                                    new Country {CountryId = 1, Name = "Moldova"},
                                    new Country {CountryId = 2, Name = "Japan"}
                                };

            var d = new CustomerDto();
            d.InjectFrom(c);
            Assert.AreEqual(c.CustomerId, d.CustomerId);
            Assert.AreEqual(c.FirstName, d.FirstName);
            Assert.AreEqual(c.SecondName, d.SecondName);

            d.InjectFrom(new My(countries), c);

            Assert.AreEqual(c.CustomerCountries.Count(), d.Countries.Count());
            Assert.AreEqual(countries.First().Name, d.Countries.First().Name);
        }

        public class My : ConventionInjection
        {
            private readonly IEnumerable<Country> countries;

            public My(IEnumerable<Country> countries)
            {
                this.countries = countries;
            }

            protected override bool Match(ConventionInfo c)
            {
                return c.SourceProp.Name == "CustomerCountries"
                       && c.TargetProp.Name == "Countries"
                       && c.SourceProp.Value != null;
            }

            protected override object SetValue(ConventionInfo c)
            {
                var src = c.SourceProp.Value as IEnumerable<CustomerCountry>;

                return src.Select(
                    o =>
                    new CountryDTO
                        {
                            CountryId = o.CountryId, 
                            Name = countries.Single(v => v.CountryId == o.CountryId).Name
                        });
            }
        }
    }
}