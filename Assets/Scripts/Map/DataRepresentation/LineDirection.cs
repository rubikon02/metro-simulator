using System.Collections.Generic;
using System.Linq;
using Simulation;
using UnityEngine;

namespace Map.DataRepresentation {
    public class LineDirection : MonoBehaviour {
        public List<Stop> stops;
        public Path path;
        public List<Platform> platforms;
        public int id;
        public LineDirection oppositeDirection;
        public SubwayLine line;

        private Queue<Vehicle> waitingVehicles = new();
        [SerializeField] private Vehicle vehiclePrefab;
        [SerializeField] private bool spawnedAll = false;

        public void Initialize() {
            waitingVehicles = new Queue<Vehicle>();
        }

        public void SendVehicle() {
            if (waitingVehicles.Count > 0) {
                spawnedAll = true;
                if (waitingVehicles.TryDequeue(out var vehicle)) {
                    vehicle.StartMoving();
                }
            } else if (!spawnedAll) {
                SpawnVehicle();
            }
        }

        private void SpawnVehicle() {
            var vehicle = Instantiate(
                vehiclePrefab,
                path.GetPositions().First(),
                Quaternion.identity
            );
            vehicle.direction = this;
            vehicle.transform.parent = line.vehicleContainer.transform;
            line.vehicles.Add(vehicle);
        }

        public void EnqueueVehicle(Vehicle vehicle) {
            waitingVehicles.Enqueue(vehicle);
        }
    }
}