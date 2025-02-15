using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
        [SerializeField] private CapsuleCollider collider;

        [SerializeField] private List<TextMeshProUGUI> passengerCounterTexts;
        [SerializeField] private List<Passenger> passengers = new();

        private static Vector3 cellSize;
        private static int gridWidth;
        private static int gridHeight;
        private static float xOffsetToCenter;
        private static Vector3 halfPlatformSize;
        private static Vector3 halfCellSize;

        private static float initialColliderSize;

        public void StartMoving() {
            currentStopGroup ??= direction.stops[0].group;
            stopped = false;
            GatherPassengers();
            UpdateTexts();
        }

        private void Start() {
            if (direction == null || direction.path == null) return;

            initialColliderSize = collider.height;
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

            currentStopGroup = direction.stops[0].group;

            StartMoving();
        }

        private void FixedUpdate() {
            if (pathPositions == null || pathPositions.Count == 0) return;

            if (!stopped) {
                UpdatePosition();
            }
        }

        public void ResizeCollider(float factor) {
            collider.height = initialColliderSize * factor;
        }

        private void UpdatePosition() {
            Vector3 targetPosition = pathPositions[targetPositionIndex];
            float step = speed * Time.deltaTime * TimeIndicator.I.SimulationSpeed;
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            while (distanceToTarget <= step) {
                transform.position = targetPosition;
                targetPositionIndex++;
                if (targetPositionIndex >= pathPositions.Count) {
                    stopped = true;
                    targetPositionIndex = 0;
                    direction = direction.oppositeDirection;
                    pathPositions = direction.path.GetPositions();
                    transform.rotation = Quaternion.LookRotation(pathPositions[1] - pathPositions[0]);
                    direction.EnqueueVehicle(this);
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
            if (gridHeight == 0) {
                Start();
            }
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
            if (passengersToGather.Count == 0) return;
            passengers.AddRange(passengersToGather);
            currentStopGroup.RemovePassengers(passengersToGather);
            if (Config.I.physicalPassengers) {
                GatherPhysicalPassengers(passengersToGather);
            }
        }

        private void GatherPhysicalPassengers(List<Passenger> passengersToGather) {
            for (int i = 0; i < passengersToGather.Count; i++) {
                passengersToGather[i].transform.parent = passengersContainer.transform;
                int index = passengers.Count - passengersToGather.Count + i;
                SetPassengerGridPosition(index);
            }
        }

        private void DropOffPassengers() {
            List<Passenger> passengersToDropOff = passengers.FindAll(p => p.GetCurrentTransferStop() == currentStopGroup);
            List<Passenger> passengersAtFinalDestination = passengers.FindAll(p => p.GetDestination() == currentStopGroup);
            foreach (Passenger passenger in passengersToDropOff) {
                passengers.Remove(passenger);
                passenger.RemoveTransfer();
            }
            currentStopGroup.AddPassengers(passengersToDropOff.Except(passengersAtFinalDestination).ToList());
            RemoveBatch(passengersAtFinalDestination);
            if (Config.I.physicalPassengers) ReorderPassengers();
        }

        private void RemoveBatch(List<Passenger> passengersToRemove) {
            foreach (Passenger passenger in passengersToRemove) {
                passengers.Remove(passenger);
            }
            PassengerSpawner.I.OnPassengerRemoved(passengersToRemove.Count);
            foreach (Passenger passenger in passengersToRemove) Destroy(passenger.gameObject);
        }

        private void ReorderPassengers() {
            for (int i = 0; i < passengers.Count; i++) {
                SetPassengerGridPosition(i);
            }
        }

        private void SetPassengerGridPosition(int passengerIndex) {
            int totalCells = gridWidth * gridHeight;
            int floor = passengerIndex / totalCells;
            int indexAtFloor = passengerIndex % totalCells;
            int x = indexAtFloor % gridWidth;
            int z = indexAtFloor / gridWidth;

            passengers[passengerIndex].transform.localPosition = new Vector3(
                x * cellSize.x - halfPlatformSize.x + halfCellSize.x + xOffsetToCenter,
                floor * cellSize.y,
                -(z * cellSize.z - halfPlatformSize.z + halfCellSize.z)
            );
        }

        private void UpdateTexts() {
            foreach (var text in passengerCounterTexts) {
                text.text = passengers.Count.ToString();
            }
        }
    }
}