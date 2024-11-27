using System;
using UnityEngine;

namespace Map.Data {
    [Serializable]
    public class Bounds {
        public float MinLon { get; set; }
        public float MaxLon { get; set; }
        public float MinLat { get; set; }
        public float MaxLat { get; set; }

        public Coordinates Center => new() {lon = (MinLon + MaxLon) / 2, lat = (MinLat + MaxLat) / 2};
    }
}
