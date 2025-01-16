using System.Linq;
using Map;
using Map.DataRepresentation;
using UI;
using UnityEngine;
using Utils;

namespace Simulation {
    public class VehicleSpawner : MonoSingleton<VehicleSpawner> {
        [SerializeField] private float spawnInterval = 10f;

        private float spawnTimer = 0f;
        private bool isSpawning = false;
        private LineDirection[] directions;

        private void FixedUpdate() {
            if (!isSpawning) return;

            spawnTimer += Time.deltaTime * TimeIndicator.I.SimulationSpeed;

            if (spawnTimer >= spawnInterval) {
                foreach (var direction in directions) {
                    direction.SendVehicle();
                }
                spawnTimer = 0f;
            }
        }

        public void StartSpawning() {
            spawnTimer = 0f;
            isSpawning = true;

            directions = SubwayLineGenerator.I.subwayLines.SelectMany(line => line.directions).ToArray();
            foreach (var direction in directions) {
                direction.Initialize();
            }
        }
    }
}