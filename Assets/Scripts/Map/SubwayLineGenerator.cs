using System.Collections.Generic;
using Map.Data;
using Map.DataRepresentation;
using UnityEngine;
using Utils;

namespace Map {
    public class SubwayLineGenerator : MonoSingleton<SubwayLineGenerator> {
        [Header("Settings")]
        public bool useCustomColors = true;

        [Header("Map element prefabs")]
        public SubwayLine subwayLinePrefab;
        public Path pathPrefab;
        public Stop stopPrefab;
        public Platform platformPrefab;
        public PlatformPoint platformPointPrefab;

        public void Generate(OsmData osmData) {
            Debug.Log("Map generation started");
            foreach (var element in osmData.Elements) {
                var subwayLine = Instantiate(
                    subwayLinePrefab,
                    MapManager.WorldPosition(element.Bounds.Center),
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
                        var path = GeneratePath(member, subwayLine.name, element.Tags.Colour);
                        path.transform.parent = subwayLine.transform;
                        subwayLine.paths.Add(path);
                    }
                }
            }
            Debug.Log("Map generation finished");
        }

        private Stop GenerateStop(Member member) {
            var stop = Instantiate(
                stopPrefab,
                MapManager.WorldPosition(member.Lon, member.Lat),
                Quaternion.identity
            );
            stop.reference = member.Ref;
            stop.coordinates = new Coordinates { lat = member.Lat, lon = member.Lon };
            stop.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            return stop;
        }

        private Platform GeneratePlatform(Member member) {
            var platform = Instantiate(
                platformPrefab,
                MapManager.WorldPosition(member.Bounds.Center),
                Quaternion.identity
            );
            foreach (var coordinates in member.Geometry) {
                var platformPoint = Instantiate(
                    platformPointPrefab,
                    MapManager.WorldPosition(coordinates),
                    Quaternion.identity
                );
                platformPoint.transform.parent = platform.transform;
                platformPoint.coordinates = coordinates;
                platformPoint.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
            }
            return platform;
        }

        private Path GeneratePath(Member member, string lineName, string hexColor) {
            var path = Instantiate(
                pathPrefab,
                MapManager.WorldPosition(member.Bounds.Center),
                Quaternion.identity
            );
            path.reference = member.Ref;

            Color lineColor;
            if (useCustomColors) {
                if (!lineColors.TryGetValue(lineName, out lineColor)) {
                    lineColor = Color.black;
                    Debug.LogError("No color found for line: " + lineName);
                }
            } else {
                if (!ColorUtility.TryParseHtmlString(hexColor, out lineColor)) {
                    lineColor = Color.black;
                    Debug.LogError("Wrong html color format: " + hexColor);
                }
            }

            path.Generate(member.Geometry, lineColor);
            return path;
        }

        private readonly Dictionary<string, Color> lineColors = new() {
            { "U1 Oberlaa – Leopoldau", Color.green },
            { "U1 Leopoldau – Oberlaa", Color.green },
            { "U2 Schottentor – Aspernstraße", Color.clear },
            { "U2 Aspernstraße – Schottentor", Color.clear },
            { "U2 Schottentor – Seestadt", Color.red },
            { "U2 Seestadt – Schottentor", Color.red },
            { "U3 Ottakring – Simmering", Color.yellow },
            { "U3 Simmering – Ottakring", Color.yellow },
            { "U4 Hütteldorf – Heiligenstadt", Color.blue },
            { "U4 Heiligenstadt – Hütteldorf", Color.blue },
            { "U6 Siebenhirten – Floridsdorf", Color.cyan },
            { "U6 Floridsdorf – Siebenhirten", Color.cyan },
        };
    }
}
