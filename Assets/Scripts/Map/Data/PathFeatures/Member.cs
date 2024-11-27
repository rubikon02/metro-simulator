using System;
using System.Collections.Generic;
using System.Linq;

namespace Map.Data {
    [Serializable]
    public class Member {
        public string Type { get; set; }
        public long Ref { get; set; }
        public string Role { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
        public List<Coordinates> Geometry { get; set; }

        public Bounds Bounds => new() {
            MinLon = Geometry.Min(el => el.lon),
            MaxLon = Geometry.Max(el => el.lon),
            MinLat = Geometry.Min(el => el.lat),
            MaxLat = Geometry.Max(el => el.lat),
        };
    }
}
