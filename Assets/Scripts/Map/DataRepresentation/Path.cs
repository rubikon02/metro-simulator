using System.Collections.Generic;
using Map.Data;
using UnityEngine;

namespace Map.DataRepresentation {
    public class Path : MapElement {
        public List<PathPoint> pathPoints;
        public PathPoint pathPointPrefab;

        public void Generate(List<Coordinates> geometry) {
            foreach (var coordinates in geometry) {
                var position = MercatorProjection.CoordsToPosition(coordinates);
                var deltaPosition = position - MapManager.I.OriginPosition;
                var pathPoint = Instantiate(pathPointPrefab, deltaPosition, Quaternion.identity);
                pathPoint.transform.parent = transform;
                pathPoint.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                pathPoint.coordinates = coordinates;
                pathPoints.Add(pathPoint);
            }
        }
    }
}
