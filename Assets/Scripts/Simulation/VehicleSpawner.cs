using System.Collections;
using System.Linq;
using Map;
using UnityEngine;
using Utils;

namespace Simulation {
    public class VehicleSpawner : MonoSingleton<VehicleSpawner> {
        [SerializeField] private Vehicle vehiclePrefab;
        [SerializeField] private float spawnInterval = 10f;
        [SerializeField] private int maxVehiclesPerLine = 20;

        public void StartSpawning() {
            StartCoroutine(SpawnVehicles());
        }

        private IEnumerator SpawnVehicles() {
            while (true) {
                foreach (var subwayLine in SubwayLineGenerator.I.subwayLines) {
                    foreach (var direction in subwayLine.directions) {
                        if (subwayLine.vehicles.Count < maxVehiclesPerLine) {
                            var vehicle = Instantiate(
                                vehiclePrefab,
                                direction.path.GetPositions().First(),
                                Quaternion.identity
                            );
                            vehicle.direction = direction;
                            vehicle.transform.parent = subwayLine.vehicleContainer.transform;
                            subwayLine.vehicles.Add(vehicle);
                        }
                    }
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}
