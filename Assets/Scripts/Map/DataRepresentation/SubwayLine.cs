using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Map.DataRepresentation {
    public class SubwayLine : MonoBehaviour {
        public List<Stop> stops;
        public List<Path> paths;
        public List<Platform> platforms;
        public int id;
    }
}
