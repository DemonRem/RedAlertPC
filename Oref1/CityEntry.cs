using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oref1
{
    public class CityEntry
    {
        public string Id { get; set; }
        public string City { get; set; }

        public override string ToString()
        {
            return Id + "\t" + City;
        }
    }
}
