using System.Collections.Generic;
using System.Linq;
using Simulation;
using TMPro;
using UnityEngine;

namespace Map.DataRepresentation {
    public class StopGroup : MonoBehaviour {
        [SerializeField] private List<Stop> stops;
        public List<Passenger> passengers;

        [Header("Components")]
        [SerializeField] private GameObject passengersContainer;
        [SerializeField] private TextMeshProUGUI nameText;

        public void SetName(string stopName) {
            name = stopName;
            nameText.text = stopName;

            foreach (var stop in stops) {
                stop.name = stopName;
            }
        }

        public string GetName() {
            return nameText.text;
        }

        public void AddStop(Stop stop) {
            stops.Add(stop);

            transform.position = new Vector3(
                stops.Average(el => el.transform.position.x),
                0,
                stops.Average(el => el.transform.position.z)
            );

            stop.SetGroup(this);
        }

        /// <summary>
        /// Adds new passenger standing at the stop in the square shape
        /// </summary>
        /// <param name="passenger"></param>
        public void AddPassenger(Passenger passenger) {
            passengers.Add(passenger);
            var cellSize = passenger.gameObject.GetComponentInChildren<Renderer>().bounds.size;
            passenger.transform.parent = passengersContainer.transform;

            int n = passengers.Count;
            int k = Mathf.CeilToInt((Mathf.Sqrt(n) - 1) / 2);
            int m = 2 * k + 1;
            int p = n - (m - 2) * (m - 2);

            int x = 0, z = 0;
            if (p <= m - 1) {
                x = k;
                z = -k + p;
            } else if (p <= 2 * (m - 1)) {
                x = k - (p - (m - 1));
                z = k;
            } else if (p <= 3 * (m - 1)) {
                x = -k;
                z = k - (p - 2 * (m - 1));
            } else {
                x = -k + (p - 3 * (m - 1));
                z = -k;
            }

            passenger.transform.localPosition = new Vector3(x * cellSize.x, passenger.transform.localPosition.y, z * cellSize.z);
        }

        public List<SubwayLine> GetSubwayLines() {
            return stops.Select(stop => stop.GetSubwayLine()).Distinct().ToList();
        }
    }
}
