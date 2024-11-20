using System.Linq;
using Map.Data;
using Map.DataRepresentation;
using UnityEngine;
using Utils;

namespace Map {
    public class SubwayLineGenerator : MonoSingleton<SubwayLineGenerator> {
        [Header("Map element prefabs")]
        public SubwayLine subwayLinePrefab;
        public Path pathPrefab;
        public Stop stopPrefab;
        public Platform platformPrefab;
        public PathPoint pathPointPrefab;

        public void Generate() {
            Debug.Log("Map generation started");
            foreach (var element in MapManager.I.OsmData.Elements) {
                var subwayLine = Instantiate(
                    subwayLinePrefab,
                    MercatorProjection.CoordsToPosition(element.Bounds.Center) - MapManager.I.OriginPosition,
                    Quaternion.identity
                );
                subwayLine.id = element.ID;

                foreach (var member in element.Members) {
                    if (member.Role == "stop") {
                        var stop = GenerateStop(member);
                        subwayLine.stops.Add(stop);
                    } else if (member.Role == "platform") {
                        // GeneratePlatform(member);
                    } else if (member.Role == "") {
                        var path = GeneratePath(member);
                        subwayLine.paths.Add(path);
                    }
                }
            }
            Debug.Log("Map generation finished");
        }

        private Stop GenerateStop(Member member) {
            var position = MercatorProjection.CoordsToPosition(member.Lon, member.Lat);
            var deltaPosition = position - MapManager.I.OriginPosition;
            var stop = Instantiate(stopPrefab, deltaPosition, Quaternion.identity);
            stop.reference = member.Ref;
            stop.coordinates = new Coordinates { lat = member.Lat, lon = member.Lon };
            stop.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            return stop;
        }

        // private void GeneratePlatform(Member member) {
        //     if (member.Geometry == null) return;
        //     foreach (var coords in member.Geometry) {
        //         var position = MercatorProjection.CoordsToPosition(coords);
        //         var deltaPosition = position - MapManager.I.OriginPosition;
        //         var stop = Instantiate(platformPrefab, deltaPosition, Quaternion.identity);
        //         stop.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
        //     }
        // }

        private Path GeneratePath(Member member) {
            var path = Instantiate(
                pathPrefab,
                MercatorProjection.CoordsToPosition(member.Bounds.Center) - MapManager.I.OriginPosition,
                Quaternion.identity
            );
            path.reference = member.Ref;

            foreach (var coordinates in member.Geometry) {
                var position = MercatorProjection.CoordsToPosition(coordinates);
                var deltaPosition = position - MapManager.I.OriginPosition;
                var pathPoint = Instantiate(pathPointPrefab, deltaPosition, Quaternion.identity);
                pathPoint.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                pathPoint.coordinates = coordinates;
                path.pathPoints.Add(pathPoint);
            }

            return path;
        }
    }
}
