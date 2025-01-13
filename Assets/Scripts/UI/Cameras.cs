using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class Cameras : MonoBehaviour {
        [SerializeField] private Camera currentCamera;

        [SerializeField] private Camera topCamera;
        [SerializeField] private Camera sideCamera;

        [SerializeField] private Button topCameraButton;
        [SerializeField] private Button sideCameraButton;

        private void Start() {
            topCamera.enabled = false;
            sideCamera.enabled = false;
            SetCamera(currentCamera);
            topCameraButton.onClick.AddListener(() => SetCamera(topCamera));
            sideCameraButton.onClick.AddListener(() => SetCamera(sideCamera));
        }

        private void SetCamera(Camera camera) {
            currentCamera.enabled = false;
            currentCamera = camera;
            currentCamera.enabled = true;
            topCameraButton.interactable = currentCamera != topCamera;
            sideCameraButton.interactable = currentCamera != sideCamera;
        }
    }
}