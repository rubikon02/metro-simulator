using System;
using System.Collections.Generic;
using System.Linq;

namespace Map.Data {
    [Serializable]
    public class OsmStopsData {
        public List<Feature> Features { get; set; }
    }
}
