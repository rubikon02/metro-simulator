using System.Collections.Generic;
using Map.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Map.DataRepresentation {
    public class Path : MapElement {
        public List<PathPoint> pathPoints;
    }
}
