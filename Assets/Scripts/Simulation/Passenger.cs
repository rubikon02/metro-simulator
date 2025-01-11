using System.Collections.Generic;
using System.Linq;
using Map.DataRepresentation;
using UnityEngine;
using UnityEngine.Serialization;

namespace Simulation {
    public class Passenger : MonoBehaviour {
        [SerializeField] private StopGroup destination;
        [SerializeField] private List<Transfer> transfers;
        [SerializeField] private float waitingTime;
        [SerializeField] private MeshRenderer capsule;

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
            return transfers.FirstOrDefault()?.stop ?? null;
        }

        public int GetCurrentTransferDirectionId() {
            return transfers.FirstOrDefault()?.direction.id ?? -1;
        }

        public void RemoveTransfer() {
            if (transfers.Count == 0) return;
            transfers.RemoveAt(0);
        }

        public void SetColor(Color color) {
            capsule.material.color = color;
        }
    }
}
