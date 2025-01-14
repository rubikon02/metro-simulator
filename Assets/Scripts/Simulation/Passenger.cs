using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map.DataRepresentation;
using UI;
using UnityEngine;

namespace Simulation {
    public class Passenger : MonoBehaviour {
        [SerializeField] private StopGroup destination;
        [SerializeField] private StopGroup start;
        [SerializeField] private List<Transfer> transfers;
        [SerializeField] private MeshRenderer capsule;

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
            transfers.RemoveAt(0);
        }

        public void SetColor(Color color) {
            capsule.material.color = color;
        }

        public void DestroyDelayed() {
            StartCoroutine(DestroyDelayedCoroutine());
        }

        private IEnumerator DestroyDelayedCoroutine() {
            yield return TimeIndicator.WaitForSecondsScaled(5f);
            PassengerSpawner.I.OnPassengerRemoved();
            Destroy(gameObject);
        }
    }
}
