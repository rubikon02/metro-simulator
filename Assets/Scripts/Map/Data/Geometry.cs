using System;
using System.Collections.Generic;
using System.Linq;

namespace Map.Data {
    [Serializable]
    public class Geometry {
        public string Type { get; set; }
        public List<float> coordinates { get; set; }

        public Coordinates Coordinates => new() {
            lon = coordinates[0],
            lat = coordinates[1],
        };
    }
}
