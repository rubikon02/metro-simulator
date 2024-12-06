using System.Collections.Generic;
using UnityEngine;

namespace Map.DataRepresentation {
    public class LineDirection : MonoBehaviour {
        public List<Stop> stops;
        public Path path;
        public List<Platform> platforms;
        public int id;
        public LineDirection oppositeDirection;
    }
}
