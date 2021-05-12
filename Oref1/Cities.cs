using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;

namespace Oref1
{
    public static class Cities
    {
        public static ReadOnlyCollection<CityEntry> ListOfCities { get; private set; }

        private static Dictionary<string, ReadOnlyCollection<string>> _citiesById;

        static Cities()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ListOfCities = new ReadOnlyCollection<CityEntry>(serializer.Deserialize<CityEntry[]>(Settings.Default.CitiesJson));

            _citiesById = ListOfCities.ToLookup(city => city.Id)
                                      .ToDictionary(group => group.Key,
                                                    group => new ReadOnlyCollection<string>(group.Select(x => x.City).ToArray()));


        }

        public static ReadOnlyCollection<string> GetCitiesById(string id)
        {
            ReadOnlyCollection<string> cities;

            if (!_citiesById.TryGetValue(id, out cities))
            {
                cities = new ReadOnlyCollection<string>(new string[] { id });
            }

            return cities;
        }
    }
}
