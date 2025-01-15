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

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI existingText;
        [SerializeField] private TextMeshProUGUI despawnedText;

        private int despawnedCount => PassengerSpawner.I.GetDespawnedCount();
        private int existingCount => PassengerSpawner.I.GetExistingCount();

        private void Start() {
            UpdateExistingText();
            UpdateDespawnedText();
        }

        private void Update() {
            UpdateExistingText();
            UpdateDespawnedText();        
        }

        private void UpdateExistingText() {
            existingText.text = $"Passengers in move: {existingCount}";
        }

        private void UpdateDespawnedText() {
            despawnedText.text = $"Passengers before: {despawnedCount}";
        }
    }
}