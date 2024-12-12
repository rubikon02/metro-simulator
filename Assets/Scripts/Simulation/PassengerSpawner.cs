using System;
using System.Linq;
using Map;
using Map.DataRepresentation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Simulation {
    public class PassengerSpawner : MonoBehaviour {
        [SerializeField] private Passenger passengerPrefab;
        [SerializeField] private float spawnInterval = 2f;

        private void Start() {
            InvokeRepeating(nameof(GeneratePassenger), 0, spawnInterval);
        }

        private void GeneratePassenger() {
            // var startStop = GetRandomStop();
            var startStop = SubwayLineGenerator.I.stopGroups[0];

            // StopGroup GetRandomDestinationStopStrategy() => GetRandomStop();
            StopGroup GetRandomDestinationStopStrategy() => GetRandomStopOnTheSameLine(startStop);

            var destinationStop = GetRandomDestinationStopStrategy();
            while (startStop == destinationStop) {
                destinationStop = GetRandomDestinationStopStrategy();
            }
            var passenger = Instantiate(passengerPrefab, startStop.transform.position, Quaternion.identity);
            startStop.AddPassenger(passenger);
            passenger.SetDestination(destinationStop);
        }

        private StopGroup GetRandomStop() {
            var stops = SubwayLineGenerator.I.stopGroups;
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
