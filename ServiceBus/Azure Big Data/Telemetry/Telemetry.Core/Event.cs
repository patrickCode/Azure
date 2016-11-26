using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Core
{
    public class Event
    {
        public string Name { get; set; }
        public Dictionary<string, object> Facets; 
        public Event(string name, IEnumerable<Facet> facets)
        {
            Name = name;

            //flattening the facet
            Facets = new Dictionary<string, object>();
            foreach (var facet in facets)
            {
                Facets.Add(facet.Name, facet.Value);
            }
        }
    }
}
