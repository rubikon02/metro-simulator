using System;
using System.IO;
using Map.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Map {
    public class MapReader : MonoBehaviour {
        public OsmData OsmData { get; private set; }

        private readonly string mapPath = Path.Combine(Application.dataPath.Replace("/", "\\"), "data", "vienna.json");

        private void Start() {
            Load();
        }

        private void Load(string path = null) {
            path ??= mapPath;

            Debug.Log($"Loading file {path}");

            if (!File.Exists(path)) {
                Debug.LogError($"File not found");
                return;
            }

            string jsonContent = File.ReadAllText(path);
            Debug.Log($"File loaded");

            OsmData = JsonConvert.DeserializeObject<OsmData>(jsonContent);
            Debug.Log($"File parsed as json");
        }
    }
}
