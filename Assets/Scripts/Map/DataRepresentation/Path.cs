using System.Collections.Generic;
using System.Linq;
using Map.Data;
using UnityEngine;

namespace Map.DataRepresentation {
    public class Path : MapElement {
        [SerializeField] private LineRenderer lineRenderer;

        public void Generate(List<Coordinates> geometry, Color lineColor) {
            lineRenderer.positionCount = geometry.Count;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.SetPositions(geometry.Select(MapManager.WorldPosition).ToArray());
        }

        public Path AddPart(List<Coordinates> geometry) {
            var newPositions = geometry.Select(MapManager.WorldPosition).ToArray();
            lineRenderer.positionCount += newPositions.Length;

            var currentPositions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(currentPositions);

            newPositions.CopyTo(currentPositions, currentPositions.Length - newPositions.Length);
            lineRenderer.SetPositions(currentPositions);

            return this;
        }

        public List<Vector3> GetPositions() {
            var positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);
            return positions.ToList();
        }
    }
}
