using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map.DataRepresentation;

namespace Simulation {
    public class Vehicle : MonoBehaviour {
        public LineDirection direction;
        public float speed = 5f;
        public float rotationSpeed = 0.5f;
        private List<Vector3> pathPositions;
        private int targetPositionIndex = 1;

        private bool stopped;
        private float passengerLoadingSpeed = 2f;
        private StopGroup currentStopGroup;
        [SerializeField] private GameObject passengersContainer;
        [SerializeField] private List<Passenger> passengers = new();

        private void Start() {
            if (direction == null || direction.path == null) return;

            pathPositions = direction.path.GetPositions();
            transform.position = pathPositions[0];
            transform.LookAt(pathPositions[1]);
        }

        private void FixedUpdate() {
            if (pathPositions == null || pathPositions.Count == 0) return;

            if (!stopped) {
                UpdatePosition();
            }
        }

        private void UpdatePosition() {
            Vector3 targetPosition = pathPositions[targetPositionIndex];
            float step = speed * Time.deltaTime;
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            while (distanceToTarget <= step) {
                transform.position = targetPosition;
                targetPositionIndex++;
                if (targetPositionIndex >= pathPositions.Count) {
                    targetPositionIndex = 0;
                    direction = direction.oppositeDirection;
                    pathPositions = direction.path.GetPositions();
                    transform.rotation = Quaternion.LookRotation(pathPositions[1] - pathPositions[0]);
                } else {
                    step -= distanceToTarget;
                }
                targetPosition = pathPositions[targetPositionIndex];
                distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position), rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.gameObject.CompareTag("Stop")) return;

            stopped = true;
            // Debug.Log(other);
            currentStopGroup = other.GetComponent<Stop>().group;
            StartCoroutine(DropOffPassengers());
            StartCoroutine(GatherPassengers());
        }

        private IEnumerator GatherPassengers() {
            while (currentStopGroup.passengers.FindAll(p => p.GetDestination() != currentStopGroup).Count > 0) {
                var passenger = currentStopGroup.passengers[0];
                currentStopGroup.passengers.Remove(passenger);
                passengers.Add(passenger);

                var cellSize = passenger.gameObject.GetComponentInChildren<Renderer>().bounds.size;
                passenger.transform.parent = passengersContainer.transform;

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



                yield return new WaitForSeconds(1f / passengerLoadingSpeed);
            }
            stopped = false;
        }

        private IEnumerator DropOffPassengers() {
            List<Passenger> passengersToDropOff = passengers.FindAll(p => p.GetDestination() == currentStopGroup);
            while (passengersToDropOff.Count > 0) {
                var passenger = passengersToDropOff[0];
                passengers.Remove(passenger);
                currentStopGroup.AddPassenger(passenger);
                passengersToDropOff.RemoveAt(0);
                passenger.SetColor(Color.gray);

                yield return new WaitForSeconds(5f);
                passenger.gameObject.SetActive(false);
                yield return new WaitForSeconds(1f / passengerLoadingSpeed);
            }
        }
    }
}