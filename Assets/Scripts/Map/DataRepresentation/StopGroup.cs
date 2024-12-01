using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Map.DataRepresentation {
    public class StopGroup : MonoBehaviour {
        [SerializeField] private List<Stop> stops;
        [SerializeField] private TextMeshProUGUI nameText;

        public void SetName(string stopName) {
            name = stopName;
            nameText.text = stopName;
        }

        public void AddStop(Stop stop) {
            stops.Add(stop);

            transform.position = new Vector3(
                stops.Average(el => el.transform.position.x),
                0,
                stops.Average(el => el.transform.position.z)
            );
        }
    }
}
