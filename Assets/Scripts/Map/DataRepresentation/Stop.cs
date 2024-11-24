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
    }
}
