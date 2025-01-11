using System.Collections.Generic;
using Map.DataRepresentation;
using UnityEngine;

namespace Simulation {
    public class Passenger : MonoBehaviour {
        [SerializeField] private StopGroup destination;
        [SerializeField] private List<Transfer> transfers;
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

        public void SetTransfers(List<Transfer> newTransfers) {
            transfers = new List<Transfer>(newTransfers);
        }

        public StopGroup GetDestination() {
            return destination;
        }

        public StopGroup GetCurrentTransferStop() {
            return transfers.Count > 0 ? transfers[0].stop : null;
        }

        public int GetCurrentTransferDirectionId() {
            return transfers.Count > 0 ? transfers[0].direction.id : -1;
        }

        public void RemoveTransfer() {
            transfers.RemoveAt(0);
        }

        public void SetColor(Color color) {
            if (meshRenderer != null) {
                meshRenderer.material.color = color;
            }
        }

    }
}
