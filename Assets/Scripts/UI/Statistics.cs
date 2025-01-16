using System;
using System.Collections;
using Simulation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI {
    public class Statistics : MonoSingleton<Statistics> {
        public int ExistingCount => existingCount;
        public int DespawnedCount => despawnedCount;
        public float TrafficIntensit => trafficIntensity;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI existingText;
        [SerializeField] private TextMeshProUGUI despawnedText;
        [SerializeField] private TextMeshProUGUI trafficText;

        private int despawnedCount => PassengerSpawner.I.GetDespawnedCount();
        private int existingCount => PassengerSpawner.I.GetExistingCount();
        private float trafficIntensity => PassengerSpawner.I.GetTrafficIntensity();
        private string dayOfWeek => TimeIndicator.I.GetDayOfWeek();


        private void Start() {
            UpdateExistingText();
            UpdateDespawnedText();
            UpdateTrafficText();
        }

        private void Update() {
            UpdateExistingText();
            UpdateDespawnedText();  
            UpdateTrafficText();      
        }

        private void UpdateExistingText() {
            existingText.text = $"Passengers in move: {existingCount}";
        }

        private void UpdateDespawnedText() {
            despawnedText.text = $"Passengers transported: {despawnedCount}";
        }

        private void UpdateTrafficText() {
            trafficText.text = $"Traffic intensity: {trafficIntensity * 100:0}%";
        }
    }
}