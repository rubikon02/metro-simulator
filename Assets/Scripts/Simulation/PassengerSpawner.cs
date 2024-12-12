using Map;
using Map.DataRepresentation;
using UnityEngine;

namespace Simulation {
    public class PassengerSpawner : MonoBehaviour {
        [SerializeField] private Passenger passengerPrefab;
        [SerializeField] private float spawnInterval = 2f;

        private void Start() {
            InvokeRepeating(nameof(GeneratePassenger), 0, spawnInterval);
        }

        private void GeneratePassenger() {
            // var startStop = GetRandomStop();
            var destinationStop = GetRandomStop();
            var startStop = SubwayLineGenerator.I.stopGroups[0];
            while (startStop == destinationStop) {
                destinationStop = GetRandomStop();
            }
            var passenger = Instantiate(passengerPrefab, startStop.transform.position, Quaternion.identity);
            startStop.AddPassenger(passenger);
            // passenger.transform.parent = startStop.transform;
            passenger.SetDestination(destinationStop);
        }

        private StopGroup GetRandomStop() {
            var stops = SubwayLineGenerator.I.stopGroups;
            return stops[Random.Range(0, stops.Count)];
        }
    }
}
