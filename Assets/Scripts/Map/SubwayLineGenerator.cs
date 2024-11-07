using System.Linq;
using Map.DataRepresentation;
using UnityEngine;
using Utils;

namespace Map {
    public class SubwayLineGenerator : MonoSingleton<SubwayLineGenerator> {
        [Header("Map element prefabs")]
        public Stop stopPrefab;
        public PathPoint pathPrefab;

        public void Generate() {
            Debug.Log("Map generation started");
            GenerateStops();
            GeneratePath();
        }

        private void GenerateStops() {
            Debug.Log("Generating stops");
            foreach (var element in MapManager.I.OsmData.Elements) {
                foreach (var member in element.Members.Where(m => m.Role == "stop")) {
                    var position = MercatorProjection.CoordsToPosition(member.Lon, member.Lat);
                    var deltaPosition = position - MapManager.I.OriginPosition;
                    Instantiate(stopPrefab, deltaPosition, Quaternion.identity);
                }
            }
            Debug.Log("Stops generated");
        }

        private void GeneratePath() {
            Debug.Log("Generating path");
            foreach (var element in MapManager.I.OsmData.Elements) {
                foreach (var member in element.Members.Where(m => m.Type == "way")) {

                    foreach (var coord in member.Geometry) {
                        var position = MercatorProjection.CoordsToPosition(coord.Lon, coord.Lat);
                        var deltaPosition = position - MapManager.I.OriginPosition;
                        Instantiate(pathPrefab, deltaPosition, Quaternion.identity);
                    }
                }
            }
            Debug.Log("Path generated");
        }
    }
}
