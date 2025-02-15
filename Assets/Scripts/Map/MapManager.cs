using Map.Data;
using Simulation;
using UnityEngine;
using Utils;

namespace Map {
    public class MapManager : MonoSingleton<MapManager> {
        private OsmData osmData;
        private OsmStopsData osmStopsData;
        private Coordinates OriginCoords => osmData.Bounds.Center;
        private Vector3 OriginPosition => MercatorProjection.CoordsToPosition(OriginCoords);

        public static Vector3 WorldPosition(Coordinates position) {
            return WorldPosition(position.lon, position.lat);
        }

        public static Vector3 WorldPosition(float longitude, float latitude) {
            return MercatorProjection.CoordsToPosition(longitude, latitude) - I.OriginPosition;
        }

        private void Start() {
            osmData = MapFileReader.Load();
            osmStopsData = MapFileReader.LoadStops();
            SubwayLineGenerator.I.Generate(osmData);
            SubwayLineGenerator.I.SetStopGroupNames(osmStopsData);
            SubwayLineGenerator.I.SetStopLines();
            SubwayLineGenerator.I.ConnectOppositeDirections();
            VehicleSpawner.I.StartSpawning();
            PassengerSpawner.I.StartSpawning();
        }
    }
}
