using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Core
{
    public class Facet
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Facet() {}

        public Facet(string name, string value)
        {
            Name = name;
            Value = value;
        }

    }
}
