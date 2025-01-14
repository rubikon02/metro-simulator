using System.Collections.Generic;
using Map.DataRepresentation;
using UnityEngine;

namespace Simulation {
    public class Passenger : MonoBehaviour {
        [SerializeField] private StopGroup destination;
        [SerializeField] private StopGroup start;
        [SerializeField] private List<Transfer> transfers;
        [SerializeField] private Renderer renderer;
        public Vector3 Size => renderer.bounds.size;

        public void SetStart(StopGroup startStop) {
            start = startStop;
        }

        public void SetDestination(StopGroup newDestination) {
            destination = newDestination;
        }

        public void SetTransfers(List<Transfer> newTransfers) {
            transfers = newTransfers;
        }

        public StopGroup GetDestination() {
            return destination;
        }

        public StopGroup GetCurrentTransferStop() {
            return transfers[0].stop;
        }

        public int GetCurrentTransferDirectionId() {
            return transfers[0].direction.id;
        }

        public void RemoveTransfer() {
            transfers.RemoveAt(0);
        }
    }
}
