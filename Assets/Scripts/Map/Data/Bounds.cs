using System;
using UnityEngine;

namespace Map.Data {
    [Serializable]
    public class Bounds {
        public float MinLon { get; set; }
        public float MaxLon { get; set; }
        public float MinLat { get; set; }
        public float MaxLat { get; set; }

        public Coordinates Center => new() {Lon = (MinLon + MaxLon) / 2, Lat = (MinLat + MaxLat) / 2};
    }
}
