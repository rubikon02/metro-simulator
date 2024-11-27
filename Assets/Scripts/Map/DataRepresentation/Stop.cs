using Map.Data;
using TMPro;
using UnityEngine;

namespace Map.DataRepresentation {
    public class Stop : MapElement {
        public Coordinates coordinates;
        [SerializeField] private TextMeshProUGUI nameText;

        public void SetName(string stopName) {
            nameText.text = stopName;
        }

        public void disableName() {
            nameText.enabled = false;
        }

        public void enableName() {
            nameText.enabled = true;
        }
    }
}
