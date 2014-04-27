using System;
using System.Collections.Generic;

namespace Xciles.PclValueInjecter.Tests
{
    public class Country
    {
        public string Name { get; set; }

        public int Id { get; set; }
    }

    public interface ICountryRepository
    {
        Country Get(int id);
        IEnumerable<Country> GetAll();
    }

    public class CountryRepository : ICountryRepository
    {
        public Country Get(int id)
        {
            return new Country { Id = id, Name = "Country" + id };
        }

        public IEnumerable<Country> GetAll()
        {
            for (var i = 0; i < 100; i++)
            {
                yield return Get(i);
            }
        }
    }

    /// <summary>
    /// my uber IoC
    /// </summary>
    public class IoC
    {
        public static CountryRepository Resolve<T>() where T : ICountryRepository
        {
            return new CountryRepository();
        }
    }


    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public Address HomeAddress { get; set; }

        
        public Country Country { get; set; }
        public Country Country2 { get; set; }
    }


    public class Address
    {
        public string Street { get; set; }
        public int HouseNumber { get; set; }

        public Country Country1 { get; set; }
        public Country Country2 { get; set; }
        public Country Country3 { get; set; }
        public string S1 { get; set; }
        public int I1 { get; set; }
    }

    public class Foo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int NotImportant { get; set; }
    }

    public class Bar
    {
        public int StreetNo { get; set; }
        public DateTime Date { get; set; }
        public int Whatever2 { get; set; }
    }

    public class FooBar
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int StreetNo { get; set; }
        public DateTime Date { get; set; }
        public string Something { get; set; }
    }
   
    public class AddressViewModel
    {
        //that would be 2 hidden inputs in the html
        public int Country1 { get; set; }
        public int Country2 { get; set; }
        public int Country3 { get; set; }

        //simple textbox
        public string S1 { get; set; }
    }

    public class PersonViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Country lookup
        //I use object because you give to the form IEnumerable<SelectListItem> 
        //and receive string[] which is the selected keys
        public object Country { get; set; }
        public object Country2 { get; set; }
    }
}
