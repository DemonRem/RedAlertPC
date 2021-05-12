using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Oref1
{
    public class AlertEventArgs : EventArgs
    {
        public AlertEventArgs(string id, ReadOnlyCollection<string> cities)
        {
            Id = id;
            Cities = cities;
        }

        public string Id { get; private set; }
        public ReadOnlyCollection<string> Cities { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Id + ": ");
            bool first = true;

            foreach (string city in Cities)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }

                sb.Append(city);
            }

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is AlertEventArgs))
            {
                return false;
            }

            return Id.Equals((obj as AlertEventArgs).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
