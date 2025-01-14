using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map.DataRepresentation;
using TMPro;
using UI;
using Utils;

namespace Simulation {
    public class Vehicle : MonoBehaviour {
        public LineDirection direction;
        public float speed = 5f;
        public float doorOpeningTime = 5f;
        [Tooltip("Rotation speed in relation to speed")]
        public float rotationSpeedRatio = 0.015f;
        private List<Vector3> pathPositions;
        private int targetPositionIndex = 1;

        private bool stopped;
        private StopGroup currentStopGroup;
        [SerializeField] private GameObject passengersContainer;
        [SerializeField] private GameObject passengerCountersContainer;
        [SerializeField] private GameObject passengerPlatform;
        [SerializeField] private GameObject passengerPrefab;
        [SerializeField] private List<TextMeshProUGUI> passengerCounterTexts;
        [SerializeField] private List<Passenger> passengers = new();

        private Vector3 cellSize;
        private int gridWidth;
        private int gridHeight;
        private float xOffsetToCenter;
        private Vector3 halfPlatformSize;
        private Vector3 halfCellSize;

        private void Start() {
            if (direction == null || direction.path == null) return;

            pathPositions = direction.path.GetPositions();
            transform.position = pathPositions[0];
            transform.LookAt(pathPositions[1]);
            passengerCounterTexts =
                new List<TextMeshProUGUI>(passengerCountersContainer.GetComponentsInChildren<TextMeshProUGUI>());

            // Calculate the grid size based on the passenger platform size and passenger size
            cellSize = passengerPrefab.GetComponentInChildren<Renderer>().bounds.size;
            var platformSize = passengerPlatform.transform.localScale * 10; // default plane is 10x10
            halfPlatformSize = platformSize / 2;
            halfCellSize = cellSize / 2;
            gridWidth = Mathf.FloorToInt(platformSize.x / cellSize.x);
            gridHeight = Mathf.FloorToInt(platformSize.z / cellSize.z);
            xOffsetToCenter = (platformSize.x - (gridWidth * cellSize.x)) / 2;
        }

        // private void OnDrawGizmos() {
        //     Debug.Log(passengerPlatform.GetComponent<Renderer>().bounds.size);
        // }

        private void FixedUpdate() {
            if (pathPositions == null || pathPositions.Count == 0) return;

            if (!stopped) {
                UpdatePosition();
            }
        }

        private void UpdatePosition() {
            Vector3 targetPosition = pathPositions[targetPositionIndex];
            float step = speed * Time.deltaTime * TimeIndicator.I.SimulationSpeed;
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            while (distanceToTarget <= step) {
                transform.position = targetPosition;
                targetPositionIndex++;
                if (targetPositionIndex >= pathPositions.Count) {
                    targetPositionIndex = 0;
                    direction = direction.oppositeDirection;
                    pathPositions = direction.path.GetPositions();
                    transform.rotation = Quaternion.LookRotation(pathPositions[1] - pathPositions[0]);
                    StartCoroutine(HandleStop());
                } else {
                    step -= distanceToTarget;
                }
                targetPosition = pathPositions[targetPositionIndex];
                distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            var movement = targetPosition - transform.position;
            if (movement == Vector3.zero) return;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), speed * rotationSpeedRatio * Time.deltaTime * TimeIndicator.I.SimulationSpeed);
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.gameObject.CompareTag("Stop")) return;

            var newStopGroup = other.GetComponent<Stop>().group;
            if (newStopGroup == currentStopGroup) return;
            stopped = true;
            currentStopGroup = newStopGroup;
            StartCoroutine(HandleStop());
        }

        private IEnumerator HandleStop() {
            yield return TimeIndicator.WaitForSecondsScaled(doorOpeningTime);
            DropOffPassengers();
            GatherPassengers();
            UpdateTexts();
            yield return TimeIndicator.WaitForSecondsScaled(doorOpeningTime);
            stopped = false;
        }

        private void GatherPassengers() {
            var passengersToGather = currentStopGroup.passengers.FindAll(p => p.GetCurrentTransferDirectionId() == direction.id);
            foreach (Passenger passenger in passengersToGather) {
                currentStopGroup.RemovePassenger(passenger);
                passengers.Add(passenger);
                if (Config.I.physicalPassengers) {
                    GatherPhysicalPassenger(passenger);
                }
            }
        }

        private void GatherPhysicalPassenger(Passenger passenger) {
            passenger.transform.parent = passengersContainer.transform;

            int index = passengers.Count - 1;
            int x = index % gridWidth;
            int z = index / gridWidth;


            passenger.transform.localPosition = new Vector3(
                x * cellSize.x - halfPlatformSize.x + halfCellSize.x + xOffsetToCenter,
                0,
                -(z * cellSize.z - halfPlatformSize.z + halfCellSize.z)
            );
        }

        private void DropOffPassengers() {
            List<Passenger> passengersToDropOff = passengers.FindAll(p => p.GetCurrentTransferStop() == currentStopGroup);
            foreach (Passenger passenger in passengersToDropOff) {
                passengers.Remove(passenger);
                currentStopGroup.AddPassenger(passenger);
                passenger.RemoveTransfer();

                if (passenger.GetDestination() == currentStopGroup) {
                    currentStopGroup.RemovePassenger(passenger);
                    passenger.DestroyDelayed();
                }
            }
            if (Config.I.physicalPassengers) ReorderPassengers();
        }

        private void ReorderPassengers() {
            for (int i = 0; i < passengers.Count; i++) {
                int x = i % gridWidth;
                int z = i / gridWidth;

                passengers[i].transform.localPosition = new Vector3(
                    x * cellSize.x - halfPlatformSize.x + halfCellSize.x + xOffsetToCenter,
                    0,
                    -(z * cellSize.z - halfPlatformSize.z + halfCellSize.z)
                );
            }
        }

        private void UpdateTexts() {
            foreach (var text in passengerCounterTexts) {
                text.text = passengers.Count.ToString();
            }
        }
    }
}