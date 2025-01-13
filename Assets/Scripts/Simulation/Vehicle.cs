using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Map.DataRepresentation;
using System.Linq;
using TMPro;
using UI.TimeIndicator;
using Unity.VisualScripting;
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
        [SerializeField] private float passengerLoadingSpeed = 2f;
        private StopGroup currentStopGroup;
        [SerializeField] private GameObject passengersContainer;
        [SerializeField] private GameObject passengerCountersContainer;
        [SerializeField] private List<TextMeshProUGUI> passengerCounterTexts;
        [SerializeField] private List<Passenger> passengers = new();

        private void Start() {
            if (direction == null || direction.path == null) return;

            pathPositions = direction.path.GetPositions();
            transform.position = pathPositions[0];
            transform.LookAt(pathPositions[1]);
            passengerCounterTexts = new List<TextMeshProUGUI>(passengerCountersContainer.GetComponentsInChildren<TextMeshProUGUI>());
        }

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

            stopped = true;
            currentStopGroup = other.GetComponent<Stop>().group;
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

            var cellSize = passenger.gameObject.GetComponentInChildren<Renderer>().bounds.size;

            int n = passengers.Count;
            int k = Mathf.CeilToInt((Mathf.Sqrt(n) - 1) / 2);
            int m = 2 * k + 1;
            int p = n - (m - 2) * (m - 2);

            int x = 0, z = 0;
            if (p <= m - 1) {
                x = k;
                z = -k + p;
            } else if (p <= 2 * (m - 1)) {
                x = k - (p - (m - 1));
                z = k;
            } else if (p <= 3 * (m - 1)) {
                x = -k;
                z = k - (p - 2 * (m - 1));
            } else {
                x = -k + (p - 3 * (m - 1));
                z = -k;
            }

            passenger.transform.localPosition = new Vector3(x * cellSize.x, passenger.transform.localPosition.y, z * cellSize.z);
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
        }

        private void UpdateTexts() {
            foreach (var text in passengerCounterTexts) {
                text.text = passengers.Count.ToString();
            }
        }
    }
}