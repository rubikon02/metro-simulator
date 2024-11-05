using Map.Data;
using UnityEngine;
using Utils;

namespace Map {
    public class MapManager : MonoSingleton<MapManager> {
        public OsmData OsmData { get; private set; }
        public Coordinates OriginCoords => OsmData.Bounds.Center;
        public Vector3 OriginPosition => MercatorProjection.CoordsToPosition(OriginCoords);

        private void Start() {
            OsmData = MapFileReader.Load();
            SubwayLineGenerator.I.Generate();
        }
    }
}
