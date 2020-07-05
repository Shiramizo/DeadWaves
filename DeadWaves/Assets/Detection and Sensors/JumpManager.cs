using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeadWaves
{
    public class JumpManager : MonoBehaviour
    {
        RaycastSensor[] allSensors;

        bool jumpAvailable;
        int count;

        public bool JumpAvailable() => jumpAvailable;

        private void Awake() {
            allSensors = GetComponentsInChildren<RaycastSensor>();
        }

        private void OnEnable() {
            SubscribeToSensors();
        }

        private void OnDisable() {
            UnsubscribeToSensors();
        }

        void SubscribeToSensors() {
            foreach (var s in allSensors) s.Subscribe(OnSensorDetection);
        }

        void UnsubscribeToSensors() {
            foreach (var s in allSensors) s.Unsubscribe(OnSensorDetection);
        }

        void OnSensorDetection(RaycastHit2D rayHit, RaycastSensor sensor) {
            count++;
        }

        private void Update() {
            jumpAvailable = count >= allSensors.Length;
        }

        private void LateUpdate() {
            count = 0;
        }
    }
}