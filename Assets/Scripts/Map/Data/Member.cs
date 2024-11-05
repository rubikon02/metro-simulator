using System;
using System.Collections.Generic;

namespace Map.Data {
    [Serializable]
    public class Member {
        public string Type { get; set; }
        public long Ref { get; set; }
        public string Role { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public List<Coordinates> Geometry { get; set; }
    }
}
