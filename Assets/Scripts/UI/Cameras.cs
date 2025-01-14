using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class Cameras : MonoBehaviour {
        [SerializeField] private Camera currentCamera;

        [SerializeField] private Camera topCamera;
        [SerializeField] private Camera sideCamera;

        [SerializeField] private Button topCameraButton;
        [SerializeField] private Button sideCameraButton;

        private List<(Camera camera, Button button)> cameras;

        private void Start() {
            cameras = new List<(Camera camera, Button button)>(
                new[] {
                    (topCamera, topCameraButton),
                    (sideCamera, sideCameraButton)
                }
            );
            UpdateCameras();
            topCameraButton.onClick.AddListener(() => UpdateCameras(topCamera));
            sideCameraButton.onClick.AddListener(() => UpdateCameras(sideCamera));
        }

        private void UpdateCameras([CanBeNull] Camera newCamera = null) {
            if (newCamera != null) currentCamera = newCamera;
            foreach (var (camera, button) in cameras) {
                camera.enabled = camera == currentCamera;
                button.interactable = camera != currentCamera;
            }
        }
    }
}