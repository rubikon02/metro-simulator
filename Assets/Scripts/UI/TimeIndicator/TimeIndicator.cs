using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.TimeIndicator {
    public class TimeIndicator : MonoSingleton<TimeIndicator> {
        public DateTime CurrentTime => currentTime;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private Button slowerButton;
        [SerializeField] private Button fasterButton;

        private readonly DateTime startTime = DateTime.Today.AddHours(8);
        private readonly float[] speeds = { 0.1f, 0.25f, 0.5f, 1f, 1.5f, 2f, 4f, 8f, 16f, 32f, 64f };
        private int currentSpeedIndex = 3;
        private DateTime currentTime;

        private void Start() {
            slowerButton.onClick.AddListener(() => ChangeSpeed(-1));
            fasterButton.onClick.AddListener(() => ChangeSpeed(1));
            UpdateSpeedText();
            SetSimulationSpeed();
            UpdateButtonStates();
            InitializeTime();
        }

        private void Update() {
            currentTime = currentTime.AddSeconds(Time.deltaTime * speeds[currentSpeedIndex]);
            UpdateTimeText();
        }

        private void ChangeSpeed(int direction) {
            currentSpeedIndex = Mathf.Clamp(currentSpeedIndex + direction, 0, speeds.Length - 1);
            UpdateSpeedText();
            SetSimulationSpeed();
            UpdateButtonStates();
        }

        private void UpdateSpeedText() {
            speedText.text = $"{speeds[currentSpeedIndex]:0.##}x";
        }

        private void UpdateTimeText() {
            timeText.text = $"<mspace=0.6em>{currentTime:hh\\:mm\\:ss}</mspace>";
        }

        private void SetSimulationSpeed() {
            Time.timeScale = speeds[currentSpeedIndex];
        }

        private void UpdateButtonStates() {
            slowerButton.interactable = currentSpeedIndex > 0;
            fasterButton.interactable = currentSpeedIndex < speeds.Length - 1;
        }

        private void InitializeTime() {
            currentTime = startTime;
        }
    }
}