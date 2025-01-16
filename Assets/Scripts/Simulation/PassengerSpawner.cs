using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Map;
using Map.DataRepresentation;
using UI;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Simulation {
    [Serializable]
    public class DayTrafficAmount {
        public string day;
        public float average;
        public AnimationCurve trafficCurve = new();
    }

    public class PassengerSpawner : MonoSingleton<PassengerSpawner> {
        [Header("Stats")]
        [SerializeField] private int spawnedCount;
        [SerializeField] private int despawnedCount;
        [SerializeField] private int existingCount;
        [SerializeField] private float trafficIntensity;
        [SerializeField] private float spawnSpeed;
        [Header("Settings")]
        [Tooltip("People per second")]
        [SerializeField] private float busiestDaySpawnSpeed = 11f;
        [SerializeField] private float spawnInterval = 10f;
        [SerializeField] private int existingPassengerLimit = 1;
        [Header("Prefabs")]
        [SerializeField] private Passenger passengerPrefab;
        [Header("Traffic Data")]
        [SerializeField] private List<DayTrafficAmount> weekTrafficAmounts = new();


        public void OnPassengerRemoved(int count = 1) {
            despawnedCount += count;
            existingCount -= count;
        }

        public void StartSpawning() {
            LoadTrafficData("Assets/Data/DayTrafficAmounts.csv");
            StartCoroutine(SpawnPassengers());
        }

        public int GetExistingCount() {
            return existingCount;
        }

        public int GetDespawnedCount() {
            return despawnedCount;
        }

        private void LoadTrafficData(string filePath) {
            string[] lines = File.ReadAllLines(filePath);
            string[] headers = lines[0].Split(';').Skip(1).ToArray();

            for (int i = 1; i < lines.Length; i++) {
                string[] values = lines[i].Split(';');
                for (int j = 1; j < values.Length; j++) {
                    if (weekTrafficAmounts.Count < headers.Length) {
                        weekTrafficAmounts.Add(new DayTrafficAmount { day = headers[j - 1] });
                    }
                    float trafficValue = float.Parse(values[j]);

                    Keyframe keyframe = new(i - 1, trafficValue / 100f);
                    weekTrafficAmounts[j - 1].trafficCurve.AddKey(keyframe);
                }
            }

            // Set 24:00 data to be the same as 00:00 the next day
            for (int i = 0; i < weekTrafficAmounts.Count; i++) {
                var nextDay = weekTrafficAmounts[(i + 1) % weekTrafficAmounts.Count];
                var lastKeyframe = weekTrafficAmounts[i].trafficCurve.keys.Last();
                var nextDayFirstKeyframe = nextDay.trafficCurve.keys.First();
                Keyframe nextDayKeyframe = new(lastKeyframe.time + 1, nextDayFirstKeyframe.value / 100f);
                weekTrafficAmounts[i].trafficCurve.AddKey(nextDayKeyframe);
            }

            // Adjust spawnSpeed to fractional traffic amounts
            foreach (var dayTraffic in weekTrafficAmounts) {
                float total = dayTraffic.trafficCurve.keys.Sum(key => key.value);
                dayTraffic.average = total / dayTraffic.trafficCurve.length;
            }
            float busiestDayAverage = weekTrafficAmounts.Max(d => d.average);
            spawnSpeed = busiestDaySpawnSpeed / busiestDayAverage;

            // Smooth the curve
            foreach (var dayTraffic in weekTrafficAmounts) {
                // General smooth
                for (int i = 0; i < dayTraffic.trafficCurve.length; i++) {
                    dayTraffic.trafficCurve.SmoothTangents(i, 0.5f);
                }

                // Fix unnatural edgecases
                for (int i = 0; i < dayTraffic.trafficCurve.length; i++) {
                    var key = dayTraffic.trafficCurve[i];
                    float inTangent = key.inTangent;
                    float outTangent = key.outTangent;

                    if (i > 0) {
                        var prevKey = dayTraffic.trafficCurve[i - 1];
                        if (Mathf.Approximately(prevKey.value, key.value)) {
                            inTangent = 0;
                            outTangent = 0;
                        } else if (prevKey.value == 0 && key.value > 0) {
                            inTangent = Mathf.Max(inTangent, 0);
                        }
                    }

                    if (i < dayTraffic.trafficCurve.length - 1) {
                        var nextKey = dayTraffic.trafficCurve[i + 1];
                        if (Mathf.Approximately(key.value, nextKey.value)) {
                            inTangent = 0;
                            outTangent = 0;
                        } else if (key.value == 0 && nextKey.value > 0) {
                            outTangent = Mathf.Max(outTangent, 0);
                        }
                    }

                    dayTraffic.trafficCurve.MoveKey(i, new Keyframe(key.time, key.value, inTangent, outTangent));
                }
            }
        }

        private IEnumerator SpawnPassengers() {
            while (true) {
                var day = TimeIndicator.I.CurrentTime.DayOfWeek;
                var currentTime = TimeIndicator.I.CurrentTime;
                var dayTraffic = weekTrafficAmounts.FirstOrDefault(d => d.day == day.ToString());

                if (dayTraffic != null) {
                    float timeInHours = currentTime.Hour + currentTime.Minute / 60f;
                    trafficIntensity = dayTraffic.trafficCurve.Evaluate(timeInHours);

                    float batchSize = spawnSpeed * spawnInterval * trafficIntensity;
                    for (int i = 0; i < batchSize; i++) {
                        GeneratePassenger();
                    }
                }

                yield return TimeIndicator.WaitForSecondsScaled(spawnInterval);
            }
        }

        private void GeneratePassenger() {
            if(existingPassengerLimit > 0 && existingCount >= existingPassengerLimit) return;
            var startStop = GetRandomStop();

            StopGroup GetRandomDestinationStopStrategy() => GetRandomStop();
            // StopGroup GetRandomDestinationStopStrategy() => GetRandomStopOnTheSameLine(startStop);

            var destinationStop = GetRandomDestinationStopStrategy();
            while (startStop == destinationStop) {
                destinationStop = GetRandomDestinationStopStrategy();
            }

            var transfers = MetroPathfinder.FindShortestPath(startStop, destinationStop);
            transfers.RemoveAt(0);

            var passenger = Instantiate(passengerPrefab, startStop.transform.position, Quaternion.identity);
            passenger.SetStart(startStop);
            passenger.SetDestination(destinationStop);
            passenger.SetTransfers(transfers);
            passenger.enabled = Config.I.physicalPassengers;
            startStop.AddPassenger(passenger);
            spawnedCount++;
            existingCount++;
        }

        private StopGroup GetRandomStop() {
            var stops = SubwayLineGenerator.I.stopGroups;
            // if (stops.Count == 0) Debug.LogError("No stop groups found");
            return stops[Random.Range(0, stops.Count)];
        }

        private StopGroup GetRandomStopOnTheSameLine(StopGroup stopGroup) {
            var stopsOnSameLine = stopGroup
                .GetSubwayLines()
                .SelectMany(line => line.directions)
                .SelectMany(direction => direction.stops)
                .Select(stop => stop.group)
                .ToList();

            return stopsOnSameLine[Random.Range(0, stopsOnSameLine.Count)];
        }
    }

}
