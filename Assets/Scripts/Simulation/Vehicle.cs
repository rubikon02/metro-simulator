using System.Collections.Generic;
using UnityEngine;
using Map.DataRepresentation;

namespace Simulation {
    public class Vehicle : MonoBehaviour {
        public SubwayLine line;
        public float speed = 5f;
        public float rotationSpeed = 0.5f;
        private List<Vector3> pathPositions;
        private int targetPositionIndex = 1;

        private void Start() {
            if (line == null || line.path == null) return;

            pathPositions = line.path.GetPositions();
            transform.position = pathPositions[0];
            transform.LookAt(pathPositions[1]);
        }

        private void FixedUpdate() {
            if (pathPositions == null || pathPositions.Count == 0) return;

            Vector3 targetPosition = pathPositions[targetPositionIndex];
            float step = speed * Time.deltaTime;
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            while (distanceToTarget <= step) {
                transform.position = targetPosition;
                targetPositionIndex++;
                if (targetPositionIndex >= pathPositions.Count) {
                    targetPositionIndex = 0;
                }
                step -= distanceToTarget;
                targetPosition = pathPositions[targetPositionIndex];
                distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position), rotationSpeed * Time.deltaTime);
        }
    }
}