using System;
using Map.DataRepresentation;
using UnityEngine;

namespace Simulation {
    public class Passenger : MonoBehaviour {
        [SerializeField] private StopGroup destination;
        [SerializeField] private float waitingTime;

        private void FixedUpdate() {
            waitingTime += Time.deltaTime;
        }

        public void SetDestination(StopGroup newDestination) {
            destination = newDestination;
        }
    }
}
