using System;
using UnityEngine;

namespace Map.Data {
    [Serializable]
    public class Bounds {
        public float MinLon { get; set; }
        public float MaxLon { get; set; }
        public float MinLat { get; set; }
        public float MaxLat { get; set; }

        public Vector3 Center => new((MinLon + MaxLon) / 2, 0, (MinLat + MaxLat) / 2);
    }
}
