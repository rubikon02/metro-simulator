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
    public class TimeTrafficAmount {
        [Tooltip("Time in HH:MM format")]
        public string time;
        [Tooltip("Traffic intensity in relation to each other")]
        public float traffic;
    }
    [Serializable]
    public class DayTrafficAmount {
        public string day;
        public AnimationCurve trafficCurve = new();
        public List<TimeTrafficAmount> hourValues = new();
    }

    public class PassengerSpawner : MonoSingleton<PassengerSpawner> {
        [Header("Stats")]
        [SerializeField] private int spawnedCount;
        [SerializeField] private int despawnedCount;
        [SerializeField] private int existingCount;
        [Header("Settings")]
        [Tooltip("People per second")]
        [SerializeField] private float spawnSpeed = 100f;
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

        private void LoadTrafficData(string filePath) {
            string[] lines = File.ReadAllLines(filePath);
            string[] headers = lines[0].Split(';').Skip(1).ToArray();

            for (int i = 1; i < lines.Length; i++) {
                string[] values = lines[i].Split(';');
                string time = values[0];
                for (int j = 1; j < values.Length; j++) {
                    if (weekTrafficAmounts.Count < headers.Length) {
                        weekTrafficAmounts.Add(new DayTrafficAmount { day = headers[j - 1] });
                    }
                    float trafficValue = float.Parse(values[j]);
                    weekTrafficAmounts[j - 1].hourValues.Add(new TimeTrafficAmount { time = time, traffic = trafficValue });

                    Keyframe keyframe = new(i - 1, trafficValue);
                    weekTrafficAmounts[j - 1].trafficCurve.AddKey(keyframe);
                }
            }

            for (int i = 0; i < weekTrafficAmounts.Count; i++) {
                var nextDay = weekTrafficAmounts[(i + 1) % weekTrafficAmounts.Count];
                var lastKeyframe = weekTrafficAmounts[i].trafficCurve.keys.Last();
                var nextDayFirstKeyframe = nextDay.trafficCurve.keys.First();
                Keyframe nextDayKeyframe = new(lastKeyframe.time + 1, nextDayFirstKeyframe.value);
                weekTrafficAmounts[i].trafficCurve.AddKey(nextDayKeyframe);
            }

            foreach (var dayTraffic in weekTrafficAmounts) {
                for (int i = 0; i < dayTraffic.trafficCurve.length; i++) {
                    dayTraffic.trafficCurve.SmoothTangents(i, 0.5f);
                }
            }
        }

        private IEnumerator SpawnPassengers() {
            while (true) {
                for (int i = 0; i < spawnSpeed * spawnInterval; i++) {
                    GeneratePassenger();
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
