using System;
using Map.Data;
using UnityEngine;

namespace Map {
    public static class MercatorProjection {
        public static double lonToX(double lon) {
            throw new NotImplementedException();
        }

        public static double latToY(double lat) {
            throw new NotImplementedException();
        }

        public static Vector3 CoordsToPosition(Coordinates coordinates) {
            return CoordsToPosition(coordinates.Lon, coordinates.Lat);
        }

        public static Vector3 CoordsToPosition(float longitude, float latitude) {
            float x = (float)lonToX(longitude);
            float y = (float)latToY(latitude);
            return new Vector3(x, 0, y);
        }
    }
}
