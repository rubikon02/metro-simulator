using System.Collections.Generic;
using System.Linq;
using Simulation;
using TMPro;
using UnityEngine;
using Utils;

namespace Map.DataRepresentation {
    public class StopGroup : MonoBehaviour {
        [SerializeField] private List<Stop> stops;
        public List<Passenger> passengers;

        [Header("Components")]
        [SerializeField] private GameObject passengersContainer;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI countText;

        public void SetName(string stopName) {
            name = stopName;
            nameText.text = stopName;

            foreach (var stop in stops) {
                stop.name = stopName;
            }
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

        public void AddPassenger(Passenger passenger) {
            passengers.Add(passenger);
            UpdateCountText();
            if (Config.I.physicalPassengers) {
                AddPhysicalPassenger(passenger);
            }
        }

        public void AddPassengers(List<Passenger> passengersToAdd) {
            passengers.AddRange(passengersToAdd);
            if (Config.I.physicalPassengers) {
                foreach (Passenger passenger in passengersToAdd) {
                    AddPhysicalPassenger(passenger);
                }
            }
            UpdateCountText();
        }

        public void RemovePassengers(IEnumerable<Passenger> passengersToRemove) {
            passengers = passengers.Except(passengersToRemove).ToList();
            UpdateCountText();
        }

        /// <summary>
        /// Adds new passenger standing at the stop in the square shape
        /// </summary>
        /// <param name="passenger"></param>
        private void AddPhysicalPassenger(Passenger passenger) {
            passenger.transform.parent = passengersContainer.transform;

            var cellSize = passenger.Size;

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

            passenger.transform.localPosition = new Vector3(x * cellSize.x, 0, z * cellSize.z);
        }

        public List<SubwayLine> GetSubwayLines() {
            return stops.Select(stop => stop.line).Distinct().ToList();
        }

        private void UpdateCountText() {
            countText.text = passengers.Count.ToString();
        }
    }
}
