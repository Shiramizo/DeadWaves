using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DeadWaves
{
    public class UpdateFrameSkipper : MonoBehaviour
    {
        public enum UpdateType { Normal, Fixed, Late };
        public UpdateType updateType = UpdateType.Normal;

        public int framesToSkip = 0;
        public int _framesCounted;
        public bool proceed => _framesCounted >= framesToSkip;

        public void Config(UpdateType type, int targetFrame = 0) {
            updateType = type;
            framesToSkip = targetFrame;
        }

        private void FixedUpdate() {
            if (updateType != UpdateType.Fixed) return;
            CountFrames();
        }

        void Update() {
            if (updateType != UpdateType.Normal) return;
            CountFrames();
        }

        private void LateUpdate() {
            if (updateType != UpdateType.Late) return;
            CountFrames();
        }

        void CountFrames() {
            _framesCounted = (_framesCounted + 1) % (framesToSkip + 1);
        }
    }
}