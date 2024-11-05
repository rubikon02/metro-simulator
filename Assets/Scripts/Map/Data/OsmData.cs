using System;
using System.Collections.Generic;
using System.Linq;

namespace Map.Data {
    [Serializable]
    public class OsmData {
        public List<Element> Elements { get; set; }

        public Bounds Bounds => new() {
            MinLon = Elements.Min(el => el.Bounds.MinLon),
            MaxLon = Elements.Max(el => el.Bounds.MaxLon),
            MinLat = Elements.Min(el => el.Bounds.MinLat),
            MaxLat = Elements.Max(el => el.Bounds.MaxLat),
        };
    }
}
