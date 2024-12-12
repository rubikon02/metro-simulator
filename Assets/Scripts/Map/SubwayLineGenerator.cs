using System.Collections.Generic;
using System.Linq;
using Map.Data;
using Map.DataRepresentation;
using UnityEngine;
using Utils;

namespace Map {
    public class SubwayLineGenerator : MonoSingleton<SubwayLineGenerator> {
        [Header("Settings")]
        [SerializeField] private bool useCustomColors = false;
        [SerializeField] private float stopMergeDistance = 400;

        [Header("Containers")]
        public GameObject subwayLinesContainer;
        public GameObject stopGroupsContainer;

        [Header("Map element prefabs")]
        public SubwayLine subwayLinePrefab;
        public LineDirection lineDirectionPrefab;
        public Path pathPrefab;
        public Stop stopPrefab;
        public StopGroup stopGroupPrefab;
        public Platform platformPrefab;
        public PlatformPoint platformPointPrefab;


        public readonly List<SubwayLine> subwayLines = new();
        public readonly List<StopGroup> stopGroups = new();

        public void Generate(OsmData osmData) {
            Debug.Log("Map generation started");


            foreach (var element in osmData.Elements) {
                var lineDirection = Instantiate(
                    lineDirectionPrefab,
                    MapManager.WorldPosition(element.Bounds.Center),
                    Quaternion.identity
                );
                lineDirection.id = element.ID;
                lineDirection.name = element.Tags.Name;

                foreach (var member in element.Members) {
                    if (member.Role == "stop") {
                        var stop = GenerateStop(member);
                        stop.transform.parent = lineDirection.transform;
                        lineDirection.stops.Add(stop);
                    } else if (member.Role == "platform" && member.Geometry != null) {
                        // var platform = GeneratePlatform(member);
                        // platform.transform.parent = lineDirection.transform;
                        // lineDirection.platforms.Add(platform);
                    } else if (member.Role == "") {
                        lineDirection.path = (lineDirection.path?.GetPositions().Last() == MapManager.WorldPosition(member.Geometry!.First()))
                            ? lineDirection.path.AddPart(member.Geometry)
                            : GeneratePath(member, lineDirection.name, element.Tags.Colour);

                        lineDirection.path.transform.parent = lineDirection.transform;
                    }
                }


                var subwayLine = subwayLines.FirstOrDefault(line => line.name == element.Tags.Ref);
                if (!subwayLine) {
                    subwayLine = Instantiate(
                        subwayLinePrefab,
                        MapManager.WorldPosition(element.Bounds.Center),
                        Quaternion.identity
                    );
                    subwayLine.name = element.Tags.Ref;
                    subwayLine.transform.parent = subwayLinesContainer.transform;
                    subwayLines.Add(subwayLine);
                }

                subwayLine.directions.Add(lineDirection);
                lineDirection.transform.parent = subwayLine.transform;
            }

            Debug.Log("Map generation finished");
        }

        public void SetStopGroupNames(OsmStopsData osmStopsData) {
            Dictionary<Vector3, string> stopNames = osmStopsData.Features
                .Where(feat => feat.Properties.PublicTransport == "station")
                .ToDictionary(
                    feat => MapManager.WorldPosition(feat.Geometry.Coordinates),
                    feat => feat.Properties.Name
                );

            foreach (var stopGroup in stopGroups) {
                var stopName = stopNames.FirstOrDefault(el =>
                    Vector3.Distance(stopGroup.transform.position, el.Key) <= stopMergeDistance
                );
                stopGroup.SetName(stopName.Value ?? string.Empty);

                if (string.IsNullOrEmpty(stopName.Value)) {
                    Debug.LogWarning("No stop name found for a group");
                }
            }
        }

        public void ConnectOppositeDirections() {
            foreach (var subwayLine in subwayLines) {
                foreach (var directionA in subwayLine.directions) {
                    foreach (var directionB in subwayLine.directions) {
                        if (directionA.stops.First().name == directionB.stops.Last().name && directionA.stops.Last().name == directionB.stops.First().name) {
                            directionB.oppositeDirection = directionA;
                            directionA.oppositeDirection = directionB;
                        }
                    }
                }
            }
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

            var nearestGroup = stopGroups.Find(group =>
                Vector3.Distance(group.transform.position, stop.transform.position) <= stopMergeDistance);
            if (nearestGroup) {
                nearestGroup.AddStop(stop);
            } else {
                var stopGroup = Instantiate(
                    stopGroupPrefab,
                    stop.transform.position,
                    Quaternion.identity
                );
                stopGroup.AddStop(stop);
                stopGroup.transform.parent = stopGroupsContainer.transform;
                stopGroups.Add(stopGroup);
            }

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
