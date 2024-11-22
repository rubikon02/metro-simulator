using System;
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
        public PlatformPoint platformPointPrefab;

        public void Generate() {
            Debug.Log("Map generation started");
            foreach (var element in MapManager.I.OsmData.Elements) {
                var subwayLine = Instantiate(
                    subwayLinePrefab,
                    MercatorProjection.CoordsToPosition(element.Bounds.Center) - MapManager.I.OriginPosition,
                    Quaternion.identity
                );
                subwayLine.id = element.ID;
                subwayLine.name = element.Tags.Name;

                foreach (var member in element.Members) {
                    if (member.Role == "stop") {
                        var stop = GenerateStop(member);
                        stop.transform.parent = subwayLine.transform;
                        subwayLine.stops.Add(stop);
                    } else if (member.Role == "platform" && member.Geometry != null) {
                        var platform = GeneratePlatform(member);
                        platform.transform.parent = subwayLine.transform;
                        subwayLine.platforms.Add(platform);
                    } else if (member.Role == "") {
                        var path = GeneratePath(member, subwayLine.name);
                        path.transform.parent = subwayLine.transform;
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

        private Platform GeneratePlatform(Member member) {
            var platform = Instantiate(
                platformPrefab,
                MercatorProjection.CoordsToPosition(member.Bounds.Center) - MapManager.I.OriginPosition,
                Quaternion.identity
            );
            foreach (var coordinates in member.Geometry) {
                var position = MercatorProjection.CoordsToPosition(coordinates);
                var deltaPosition = position - MapManager.I.OriginPosition;
                var platformPoint = Instantiate(platformPointPrefab, deltaPosition, Quaternion.identity);
                platformPoint.transform.parent = platform.transform;
                platformPoint.coordinates = coordinates;
                platformPoint.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
            }
            return platform;
        }

        private Path GeneratePath(Member member, string lineName) {
            var path = Instantiate(
                pathPrefab,
                MercatorProjection.CoordsToPosition(member.Bounds.Center) - MapManager.I.OriginPosition,
                Quaternion.identity
            );
            path.reference = member.Ref;

            Color lineColor;
            switch(lineName) {
                case "U1 Oberlaa – Leopoldau":
                case "U1 Leopoldau – Oberlaa":
                    lineColor = Color.green;
                    break;
                case "U2 Schottentor – Aspernstraße":
                case "U2 Aspernstraße – Schottentor":
                    lineColor = Color.red;
                    break;
                case "U2 Schottentor – Seestadt":
                case "U2 Seestadt – Schottentor":
                    lineColor = Color.clear;
                    break;
                case "U3 Ottakring – Simmering":
                case "U3 Simmering – Ottakring":
                    lineColor = Color.yellow;
                    break;
                case "U4 Hütteldorf – Heiligenstadt":
                case "U4 Heiligenstadt – Hütteldorf":
                    lineColor = Color.blue;
                    break;
                case "U6 Siebenhirten – Floridsdorf":
                case "U6 Floridsdorf – Siebenhirten":
                    lineColor = Color.cyan;
                    break;

                default:
                    lineColor = Color.black;
                    break;
            }
            path.Generate(member.Geometry, lineColor);
            return path;
        }
    }
}
