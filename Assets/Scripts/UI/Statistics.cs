using System;
using System.Collections;
using Simulation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI {
    public class Statistics : MonoSingleton<Statistics> {

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI existingText;
        [SerializeField] private TextMeshProUGUI despawnedText;
        [SerializeField] private TextMeshProUGUI trafficText;

        private int despawnedCount => PassengerSpawner.I.GetDespawnedCount();
        private int existingCount => PassengerSpawner.I.GetExistingCount();
        private float trafficIntensity => PassengerSpawner.I.GetTrafficIntensity();
        private string dayOfWeek => TimeIndicator.I.CurrentTime.DayOfWeek.ToString();
        private string lastDayOfWeek = null;
        private int despawnedPast = 0;

        private void Start() {
            UpdateExistingText();
            UpdateDespawnedText();
            UpdateTrafficText();
            if(lastDayOfWeek == null) {
                lastDayOfWeek = dayOfWeek;
                Debug.Log("nadano");
            }
        }

        private void Update() {
            UpdateExistingText();
            UpdateDespawnedText();  
            UpdateTrafficText();     
            if(lastDayOfWeek == null) {
                lastDayOfWeek = dayOfWeek;
                Debug.Log("nadano");

            } else if(lastDayOfWeek != dayOfWeek) {
                Debug.Log("różne");
                lastDayOfWeek = dayOfWeek;
                despawnedPast = despawnedCount;
            }
        }

        private void UpdateExistingText() {
            existingText.text = $"Passengers in move: {existingCount}";
        }

        private void UpdateDespawnedText() {
            despawnedText.text = $"Passengers transported: {despawnedCount - despawnedPast}";
        }

        private void UpdateTrafficText() {
            trafficText.text = $"Traffic intensity: {trafficIntensity * 100:0}%";
        }
    }
}