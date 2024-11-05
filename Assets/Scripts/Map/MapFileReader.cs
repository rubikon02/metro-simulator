using System.IO;
using Map.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Map {
    public static class MapFileReader {
        private static readonly string mapPath = Path.Combine(Application.dataPath.Replace("/", "\\"), "data", "vienna.json");

        public static OsmData Load(string path = null) {
            path ??= mapPath;

            Debug.Log($"Loading file {path}");

            if (!File.Exists(path)) {
                Debug.LogError($"File not found");
                return null;
            }

            string jsonContent = File.ReadAllText(path);
            Debug.Log($"File loaded");

            var osmData = JsonConvert.DeserializeObject<OsmData>(jsonContent);
            Debug.Log($"File parsed as json");

            return osmData;
        }
    }
}
