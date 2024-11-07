using Map.Data;
using UnityEngine;

namespace Map {
    /// <summary>
    /// C# Implementation based on http://wiki.openstreetmap.org/wiki/Mercator#C_implementation
    /// </summary>
    public static class MercatorProjection {
        private static readonly float R_MAJOR = 6378137f;
        private static readonly float R_MINOR = 6356752.3142f;
        private static readonly float RATIO = R_MINOR / R_MAJOR;
        private static readonly float ECCENT = Mathf.Sqrt(1f - (RATIO * RATIO));
        private static readonly float COM = 0.5f * ECCENT;

        private static readonly float DEG2RAD = Mathf.PI / 180f;
        private static readonly float RAD2Deg = 180f / Mathf.PI;
        private static readonly float PI_2 = Mathf.PI / 2f;

        public static float lonToX(float lon) {
            return R_MAJOR * DegToRad(lon);
        }

        public static float latToY(float lat) {
            lat = Mathf.Min(89.5f, Mathf.Max(lat, -89.5f));
            float phi = DegToRad(lat);
            float sinphi = Mathf.Sin(phi);
            float con = ECCENT * sinphi;
            con = Mathf.Pow(((1f - con) / (1f + con)), COM);
            float ts = Mathf.Tan(0.5f * ((Mathf.PI * 0.5f) - phi)) / con;
            return 0 - R_MAJOR * Mathf.Log(ts);
        }

        public static float xToLon(float x) {
            return RadToDeg(x) / R_MAJOR;
        }

        public static float yToLat(float y) {
            float ts = Mathf.Exp(-y / R_MAJOR);
            float phi = PI_2 - 2 * Mathf.Atan(ts);
            float dphi = 1f;
            int i = 0;
            while ((Mathf.Abs(dphi) > 0.000000001) && (i < 15)) {
                float con = ECCENT * Mathf.Sin(phi);
                dphi = PI_2 - 2 * Mathf.Atan(ts * Mathf.Pow((1f - con) / (1f + con), COM)) - phi;
                phi += dphi;
                i++;
            }

            return RadToDeg(phi);
        }

        public static Vector3 CoordsToPosition(Coordinates coordinates) {
            return CoordsToPosition(coordinates.Lon, coordinates.Lat);
        }

        public static Vector3 CoordsToPosition(float longitude, float latitude) {
            float x = lonToX(longitude);
            float y = latToY(latitude);
            return new Vector3(x, 0, y);
        }

        private static float RadToDeg(float rad) {
            return rad * RAD2Deg;
        }

        private static float DegToRad(float deg) {
            return deg * DEG2RAD;
        }
    }
}
