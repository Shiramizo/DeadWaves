using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DeadWaves
{
    [ExecuteAlways]
    public class RaycastSensor : MonoBehaviour
    {
        public Action<RaycastHit2D, RaycastSensor> OnCollision;

        int priority = 0;
        bool isSubscribed;

        public Color color = Color.magenta;

        public LayerMask sensorLayer;
        RaycastHit2D _rayHit;

        public float sensorLength;
        public float sensorAngle;
        float _sensorLength { get { return sensorLength / 100f; } set { sensorLength = value; } } //I only created this property to make the numbers on the inspector be less polluted with values so small

        [Header("THIS WILL ADD A UpdateFrameSkipper COMPONENT!")]
        [Space()]
        public bool performanceOptions;
        UpdateFrameSkipper _frameSkipper;

        private void Awake() {
            TryGetFrameSkipper();
        }

        private void Update() {
            if (!Application.isPlaying) {

                if (performanceOptions && !TryGetFrameSkipper()) {
                    ConfigureFrameSkipper();
                    return;
                }

                if (!performanceOptions && TryGetFrameSkipper()) {
                    DestroyImmediate(_frameSkipper);
                    return;
                }

                return;
            }

            if (performanceOptions && !_frameSkipper.proceed) return;

            _rayHit = Physics2D.Raycast(transform.position, Quaternion.AngleAxis(sensorAngle, transform.forward) * -transform.up, _sensorLength, sensorLayer);

            if (_rayHit.collider != null) {
                OnCollision?.Invoke(_rayHit, this);
                //Debug.Log($"{_rayHit.collider.gameObject.name}", _rayHit.collider.gameObject);
            }
        }

        bool TryGetFrameSkipper() {
            if (_frameSkipper) return true;
            else {
                _frameSkipper = GetComponent<UpdateFrameSkipper>();
                return _frameSkipper;
            }
        }

        void ConfigureFrameSkipper() {
            _frameSkipper = gameObject.AddComponent<UpdateFrameSkipper>();
            _frameSkipper.Config(UpdateFrameSkipper.UpdateType.Normal);
        }

        public void Subscribe(Action<RaycastHit2D, RaycastSensor> method, int priorityValue = 0) {
            if (isSubscribed) return;
            isSubscribed = true;
            OnCollision += method;
            priority = priorityValue;
        }

        public void Unsubscribe(Action<RaycastHit2D, RaycastSensor> method) {
            if (!isSubscribed) return;
            isSubscribed = false;
            OnCollision -= method;
        }

        public int GetPriority() => priority;

        private void OnDrawGizmos() {
            //Vector3 dir = -transform.up * sensorLength; //Gets direction
            //dir = Quaternion.AngleAxis(sensorAngle, transform.forward) * dir; //Rotates vector by a quaternion (The "*" does that)
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(sensorAngle, transform.forward) * -transform.up * _sensorLength, color);
        }
    }
}