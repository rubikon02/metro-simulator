using System.Collections.Generic;
using Simulation;
using UnityEngine;

namespace Map.DataRepresentation {
    public class SubwayLine : MonoBehaviour {
        public List<LineDirection> directions;
        public GameObject vehicleContainer;
        public List<Vehicle> vehicles;
    }
}
