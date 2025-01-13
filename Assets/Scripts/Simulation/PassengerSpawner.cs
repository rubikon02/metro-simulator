using System.Collections;
using System.Linq;
using Map;
using Map.DataRepresentation;
using UI.TimeIndicator;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Simulation {
    public class PassengerSpawner : MonoSingleton<PassengerSpawner> {
        [Header("Stats")]
        [SerializeField] private int spawnedCount;
        [SerializeField] private int despawnedCount;
        [SerializeField] private int existingCount;
        [Header("Settings")]
        [Tooltip("People per second")]
        [SerializeField] private float spawnSpeed = 100f;
        [SerializeField] private int existingPassengerLimit = 1;
        [Header("Prefabs")]
        [SerializeField] private Passenger passengerPrefab;

        public void OnPassengerRemoved() {
            despawnedCount++;
            existingCount--;
        }

        public void StartSpawning() {
            StartCoroutine(SpawnPassengers());
        }

        private IEnumerator SpawnPassengers() {
            while (true) {
                for (int i = 0; i < spawnSpeed; i++) {
                    GeneratePassenger();
                }
                yield return TimeIndicator.WaitForSecondsScaled(1f);
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
            passenger.SetColor(Color.magenta);
            startStop.AddPassenger(passenger);
            spawnedCount++;
            existingCount++;
        }

        private StopGroup GetRandomStop() {
            var stops = SubwayLineGenerator.I.stopGroups;
            if (stops.Count == 0) Debug.LogError("No stop groups found");
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
