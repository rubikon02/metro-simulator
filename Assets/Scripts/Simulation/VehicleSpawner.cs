using System.Collections.Generic;
using System.Linq;
using Map;
using Map.DataRepresentation;
using UI;
using UnityEngine;
using Utils;

namespace Simulation {
    public class VehicleSpawner : MonoSingleton<VehicleSpawner> {
        [SerializeField] private Vehicle vehiclePrefab;
        [SerializeField] private float spawnInterval = 10f;

        private float spawnTimer = 0f;
        private bool isSpawning = false;
        private Dictionary<LineDirection, Vehicle> firstVehicles = new();

        private void Update() {
            if (!isSpawning) return;

            spawnTimer += Time.deltaTime * TimeIndicator.I.SimulationSpeed;
            if (spawnTimer >= spawnInterval) {
                SpawnVehicles();
                spawnTimer = 0f;
            }

            foreach (var direction in firstVehicles.Keys.ToList()) {
                var firstVehicle = firstVehicles[direction];
                if (firstVehicle != null) {
                    float distanceToEnd = Vector3.Distance(firstVehicle.transform.position, direction.path.GetPositions().Last());
                    // Debug.Log(distanceToEnd);
                    if (distanceToEnd <= 500f) {
                        firstVehicles.Remove(direction);
                    }
                }
            }
        }

        public void StartSpawning() {
            spawnTimer = 0f;
            isSpawning = true;
            SpawnVehicles(firstSpawn: true);
        }

        private void SpawnVehicles(bool firstSpawn = false) {
            foreach (var subwayLine in SubwayLineGenerator.I.subwayLines) {
                foreach (var direction in subwayLine.directions) {
                    if (!firstSpawn && !firstVehicles.ContainsKey(direction)) continue;

                    var vehicle = Instantiate(
                        vehiclePrefab,
                        direction.path.GetPositions().First(),
                        Quaternion.identity
                    );
                    vehicle.direction = direction;
                    vehicle.transform.parent = subwayLine.vehicleContainer.transform;
                    subwayLine.vehicles.Add(vehicle);

                    if (firstSpawn) firstVehicles.Add(direction, vehicle);
                }
            }
        }
    }
}