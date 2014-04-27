//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Web.Mvc;
//using NUnit.Framework;

//namespace Xciles.PclValueInjecter.Tests
//{
//    [TestFixture]
//    public class UberTest
//    {
//        [TestFixtureSetUp]
//        public void Start()
//        {
//            //speed tweak
//            //it is going to make the TypeDescriptor (which is used by the ValueInjecter) work a bit faster
//            //but prevents some implicit conversions while unboxing (if you don't have this kind of valueinjections than you can use it)
//            PropertyInfosStorage.RegisterActionForEachType(HyperTypeDescriptionProvider.Add);
//        }

//        [Test]
//        public void SameNameTypeTest()
//        {
//            var foo = new Foo { FirstName = "F1", LastName = "L1", NotImportant = 123 };
//            var bar = new Bar { Date = DateTime.Now, StreetNo = 32, Whatever2 = 654 };
//            var fooBar = new FooBar { Something = "something" };

//            //interface for Mocking 
//            IValueInjecter injecter = new ValueInjecter();

//            //Act
//            injecter.Inject(fooBar,foo,bar);

//            Assert.AreEqual(fooBar.FirstName, foo.FirstName);
//            Assert.AreEqual(fooBar.LastName, foo.LastName);
//            Assert.AreEqual(fooBar.Date, bar.Date);
//            Assert.AreEqual(fooBar.StreetNo, bar.StreetNo);
//            Assert.AreEqual(fooBar.Something, "something");
//        }

//        [Test]
//        public void CountryToIntTest()
//        {
//            var address = new Address
//                              {
//                                  Country1 = new Country { Id = 7, Name = "Country7" },
//                                  Country2 = new Country { Id = 9, Name = "Country9" },
//                                  I1 = 1,
//                                  S1 = "s"
//                              };
//            var avm = new AddressViewModel();
//            IValueInjecter injecter = new ValueInjecter();

//            //Act
//            injecter.Inject(avm, address);
//            injecter.Inject<CountryToInt>(avm, address);

//            Assert.AreEqual(avm.Country1, 7);
//            Assert.AreEqual(avm.Country2, 9);
//            Assert.AreEqual(avm.S1, "s");
//        }

//        [Test]
//        public void IntToCountryTest()
//        {
//            var avm = new AddressViewModel { Country1 = 7, Country2 = 9, S1 = "s" };
//            var address = new Address();

//            //using static value injecter here
//            //Act
//            address.InjectFrom(avm)
//                   .InjectFrom<IntToCountry>(avm);

//            Assert.AreEqual(address.Country1.Id, 7);
//            Assert.AreEqual(address.Country2.Id, 9);
//            Assert.AreEqual(address.Country1.Name, "Country7");
//            Assert.AreEqual(address.Country2.Name, "Country9");
//            Assert.AreEqual(address.S1, "s");
//        }

//        [Test]
//        public void LookupToCountryTest()
//        {
//            var personViewModel = new PersonViewModel { Id = 1, Name = "Jimmy", Country = new[] { "3" }, Country2 = new[] { "9" } };
//            var person = new Person();

//            //Act
//            person.InjectFrom(personViewModel)
//                  .InjectFrom<LookupToCountry>(personViewModel);

//            Assert.AreEqual(person.Id, personViewModel.Id);
//            Assert.AreEqual(person.Name, personViewModel.Name);
//            Assert.AreEqual(person.Country.Id, 3);
//            Assert.AreEqual(person.Country2.Id, 9);
//        }

//        [Test]
//        public void CountryToLookupTest()
//        {
//            var person = new Person
//            {
//                Id = 1,
//                Name = "Jimmy",
//                Country = new Country { Id = 3, Name = "Country3" },
//                Country2 = new Country { Id = 9, Name = "Country9" },

//            };
//            var personViewModel = new PersonViewModel();

//            //Act
//            personViewModel.InjectFrom(person)
//                           .InjectFrom<CountryToLookup>(person);

//            Assert.AreEqual(personViewModel.Id, personViewModel.Id);
//            Assert.AreEqual(personViewModel.Name, personViewModel.Name);

//            var selectCountry = (personViewModel.Country as IEnumerable<SelectListItem>).Where(o => o.Selected).Single();
//            var selectCountry2 = (personViewModel.Country2 as IEnumerable<SelectListItem>).Where(o => o.Selected).Single();

//            Assert.AreEqual(person.Country.Id, Convert.ToInt32(selectCountry.Value));
//            Assert.AreEqual(person.Country.Name, selectCountry.Text);
//            Assert.AreEqual(person.Country2.Id, Convert.ToInt32(selectCountry2.Value));
//            Assert.AreEqual(person.Country2.Name, selectCountry2.Text);
//        }

//        [Test]
//        public void SpeedTestAgainstAutomapper()
//        {
//            var watch = new Stopwatch();
//            watch.Start();
//            for (var i = 0; i < 10000; i++)
//            {
//                new Foo().InjectFrom(new Foo { FirstName = "fname", LastName = "lname" });
//            }
//            watch.Stop();
//            Console.Out.WriteLine("ValueInjecter: {0} ", watch.Elapsed);

//            AutoMapper.Mapper.CreateMap<Foo, Foo>();
//            var awatch = new Stopwatch();
//            awatch.Start();
//            for (var i = 0; i < 10000; i++)
//            {
//                Mapper.Map(new Foo { FirstName = "fname", LastName = "lname" }, new Foo());
//            }
//            awatch.Stop();
//            Console.Out.WriteLine("Automapper: {0} ", awatch.Elapsed);

//        }

//        public class CountryToInt : LoopValueInjection<Country, int>
//        {
//            protected override int SetValue(Country sourcePropertyValue)
//            {
//                return sourcePropertyValue != null ? sourcePropertyValue.Id : 0;
//            }
//        }

//        public class IntToCountry : LoopValueInjection<int, Country>
//        {
//            protected override Country SetValue(int sourcePropertyValue)
//            {
//                return IoC.Resolve<ICountryRepository>().Get(sourcePropertyValue);
//            }
//        }

//        public class CountryToLookup : LoopValueInjection<Country, object>
//        {
//            protected override object SetValue(Country sourcePropertyValue)
//            {
//                var key = sourcePropertyValue != null ? sourcePropertyValue.Id : 0;

//                return IoC.Resolve<ICountryRepository>().GetAll()
//                       .Select(o => new SelectListItem
//                       {
//                           Text = o.Name,
//                           Value = o.Id.ToString(),
//                           Selected = o.Id == key
//                       });
//            }
//        }

//        public class LookupToCountry : LoopValueInjection<object, Country>
//        {
//            protected override Country SetValue(object sourcePropertyValue)
//            {
//                var key = Convert.ToInt32((((string[])sourcePropertyValue)[0]));
//                return IoC.Resolve<ICountryRepository>().Get(key);
//            }
//        }
//    }
//}
