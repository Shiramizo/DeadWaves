using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeadWaves
{
    public class GroundManager : MonoBehaviour
    {
        CharacterControl charControl;
        RaycastSensor[] allSensors;
        RaycastSensor currentValidSensor;
        RaycastHit2D currentValidGroundInfo;

        bool dirty = false;

        private void Awake() {
            LoadCharControl();
            LoadSensors();
        }

        private void OnEnable() {
            SubscribeGroundSensorEvents();
        }

        private void OnDisable() {
            UnsubscribeGroundSensorEvents();
        }

        void LoadCharControl() {
            charControl = GetComponentInParent<CharacterControl>();
        }

        void CallCharControlEvent() {
            //print($"Calling ground from {currentValidSensor.gameObject.name}");
            //print($"Calling ground on frame {Time.frameCount}");
            charControl.OnGroundEvent.Invoke(currentValidGroundInfo);
            dirty = false;
        }

        void LoadSensors() {
            allSensors = GetComponentsInChildren<RaycastSensor>();
        }

        public RaycastSensor[] GetAllSensors() => allSensors;
        public int GetAllSensorsLength() {
            if (allSensors == null) return 0;
            return allSensors.Length;
        }

        void SubscribeGroundSensorEvents() {
            int priority = 0;
            foreach (var sensor in allSensors) {
                sensor.Subscribe(OnGroundSensor, priority);
                priority++;
            }
        }

        void UnsubscribeGroundSensorEvents() {
            foreach (var sensor in allSensors) sensor.Unsubscribe(OnGroundSensor);
        }

        void OnGroundSensor(RaycastHit2D info, RaycastSensor groundDetectorSensor) {
            if (!dirty) {
                currentValidSensor = groundDetectorSensor;
                currentValidGroundInfo = info;
            } else {
                SolvePriority(info, groundDetectorSensor);
            }

            dirty = true;
        }

        void SolvePriority(RaycastHit2D newInfo, RaycastSensor newSensor) {
            bool isNewBetter = newSensor.GetPriority() < currentValidSensor.GetPriority();
            currentValidGroundInfo = isNewBetter ? newInfo : currentValidGroundInfo;
            currentValidSensor = isNewBetter ? newSensor : currentValidSensor;
        }

        void SetCharControlGrounded(bool value) => charControl.SetGrounded(value);

        private void Update() {
            SetCharControlGrounded(dirty);
            if (dirty) CallCharControlEvent();
        }
    }
}