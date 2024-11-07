using System.Collections.Generic;
using UnityEngine;

namespace Map.DataRepresentation {
    public class SubwayLine : MonoBehaviour {
        public List<Stop> Stops;
        public List<PathPoint> Path;
        public List<Platform> Platforms;
    }
}
