using System.Linq;
using Map.Data;
using Map.DataRepresentation;
using UnityEngine;
using Utils;

namespace Map {
    public class SubwayLineGenerator : MonoSingleton<SubwayLineGenerator> {
        [Header("Map element prefabs")]
        public Stop stopPrefab;
        public Platform platformPrefab;
        public PathPoint pathPointPrefab;

        public void Generate() {
            Debug.Log("Map generation started");
            foreach (var element in MapManager.I.OsmData.Elements) {
                foreach (var member in element.Members) {
                    if (member.Role == "stop") {
                        GenerateStop(member);
                    } else if (member.Role == "platform") {
                        GeneratePlatform(member);
                    } else if (member.Role == "") {
                        GeneratePath(member);
                    }
                }
            }
            Debug.Log("Map generation finished");
        }

        private void GenerateStop(Member member) {
            var position = MercatorProjection.CoordsToPosition(member.Lon, member.Lat);
            var deltaPosition = position - MapManager.I.OriginPosition;
            var stop = Instantiate(stopPrefab, deltaPosition, Quaternion.identity);
            stop.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }

        private void GeneratePlatform(Member member) {
            if (member.Geometry == null) return;
            foreach (var coords in member.Geometry) {
                var position = MercatorProjection.CoordsToPosition(coords);
                var deltaPosition = position - MapManager.I.OriginPosition;
                var stop = Instantiate(platformPrefab, deltaPosition, Quaternion.identity);
                stop.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
            }
        }

        private void GeneratePath(Member member) {
            if (member.Geometry == null) return;
            foreach (var coords in member.Geometry) {
                var position = MercatorProjection.CoordsToPosition(coords);
                var deltaPosition = position - MapManager.I.OriginPosition;
                var stop = Instantiate(pathPointPrefab, deltaPosition, Quaternion.identity);
                stop.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
            }
        }
    }
}
