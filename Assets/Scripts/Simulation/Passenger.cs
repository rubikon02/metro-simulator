using System.Collections.Generic;
using System.Linq;
using Map.DataRepresentation;
using UI.TimeIndicator;
using UnityEngine;

namespace Simulation {
    public class Passenger : MonoBehaviour {
        [SerializeField] private StopGroup destination;
        [SerializeField] private StopGroup start;
        [SerializeField] private List<Transfer> transfers;
        [SerializeField] private float waitingTime;
        [SerializeField] private MeshRenderer capsule;

        // Performance heavy
        // private void FixedUpdate() {
        //     waitingTime += Time.deltaTime * TimeIndicator.I.SimulationSpeed;
        // }

        public void SetStart(StopGroup startStop) {
            start = startStop;
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
            return transfers.FirstOrDefault()?.stop;
        }

        public int GetCurrentTransferDirectionId() {
            return transfers.FirstOrDefault()?.direction.id ?? -1;
        }

        public void RemoveTransfer() {
            if (transfers.Count == 0) return;
            Debug.Log($"{transfers.First().stop.name}, {transfers.First().direction.name}");
            transfers.RemoveAt(0);
        }

        public void SetColor(Color color) {
            capsule.material.color = color;
        }
    }
}
