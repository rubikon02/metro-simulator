using System.Collections.Generic;
using System.Linq;
using Map.Data;
using UnityEngine;

namespace Map.DataRepresentation {
    public class Path : MapElement {
        public LineRenderer lineRenderer;

        public void Generate(List<Coordinates> geometry, Color lineColor) {
            lineRenderer.positionCount = geometry.Count;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.SetPositions(geometry.Select(MapManager.WorldPosition).ToArray());
        }
    }
}
