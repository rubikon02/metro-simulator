using System.Linq;
using Map;
using Map.DataRepresentation;
using UI.TimeIndicator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Simulation {
    public class PassengerSpawner : MonoBehaviour {
        [SerializeField] private Passenger passengerPrefab;
        [SerializeField] private float spawnInterval = 2f;
        // private bool generateOnlyOne = false;

        private void Start() {
            InvokeRepeating(nameof(GeneratePassenger), 0, spawnInterval / TimeIndicator.I.SimulationSpeed);
        }

        private void GeneratePassenger() {
            // if(generateOnlyOne) return;
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
            passenger.SetDestination(destinationStop);
            passenger.SetTransfers(transfers);
            passenger.SetColor(Color.magenta);
            startStop.AddPassenger(passenger);
            // generateOnlyOne = true;
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
