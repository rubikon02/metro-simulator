using System;
using System.Collections.Generic;

namespace Map.Data {
    [Serializable]
    public class Feature {
        public string Type { get; set; }
        public Properties Properties { get; set; }
        public Geometry Geometry { get; set; }
        public string ID { get; set; }
    }
}
