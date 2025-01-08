using System;
using Map.DataRepresentation;
using UnityEngine;

namespace Simulation {
    public class Passenger : MonoBehaviour {
        [SerializeField] private StopGroup destination;
        [SerializeField] private float waitingTime;
        private MeshRenderer meshRenderer;

        private void Awake() {
              Transform capsuleTransform = transform.Find("Capsule");
            if (capsuleTransform != null) {
                meshRenderer = capsuleTransform.GetComponent<MeshRenderer>();
            } else {
                Debug.LogError("Capsule object not found in Passenger prefab!");
            }
        }

        private void FixedUpdate() {
            waitingTime += Time.deltaTime;
        }

        public void SetDestination(StopGroup newDestination) {
            destination = newDestination;
        }

        public StopGroup GetDestination() {
            return destination;
        }

        public void SetColor(Color color) {
            if (meshRenderer != null) {
                meshRenderer.material.color = color;
            }
        }

    }
}
