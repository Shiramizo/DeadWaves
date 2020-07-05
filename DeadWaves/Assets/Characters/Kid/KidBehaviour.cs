using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DeadWaves
{
    public class KidBehaviour : CharacterBehaviour
    {
        public MouseButton input;
        bool mouseClick => Input.GetMouseButtonDown((int)input);
        bool spaceHold => Input.GetKey(KeyCode.Space);
        bool goToTarget = false, cueJump = false, isJumping;

        public bool useSpaceToJump = false;

        Vector2 _rbVelocity;

        public JumpManager jumpManager;

        protected override void Awake() {
            if (!jumpManager) jumpManager = GetComponentInChildren<JumpManager>();

            base.Awake();
        }

        protected override void Update() {
            if (mouseClick) {
                SetTarget(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.right + Vector3.up), true);
            }

            if ((spaceHold || jumpManager && jumpManager.JumpAvailable()) && charController.GetGrounded()) {
                cueJump = true;
            }

            if (isJumping) {
                if (!spaceHold && useSpaceToJump) {
                    charController.StopJump();
                    isJumping = false;
                } else isJumping = !charController.GetGrounded();
            }

            base.Update();
        }

        private void FixedUpdate() {
            if (goToTarget) {
                AlignToNextWaypoint();
                charController.Move(charController.Get2D_GroundForwardDirection(), ForceMode2D.Force);
                //print($"Kid moving! {GetComponent<Rigidbody2D>().velocity} at direction {charController.Get2D_GroundForwardDirection()}");
            }

            if (cueJump) {
                charController.Jump(false);
                cueJump = false;
                isJumping = true;
            }
        }

        private void LateUpdate() {
            UpdateAnimatorParameters();
            //print($"[KID INFO] {_rbVelocity.y}");
        }

        void UpdateAnimatorParameters() {
            _rbVelocity = charController.GetVelocity();
            _anim.SetFloat("VelocityX", Mathf.Abs(_rbVelocity.x));
            _anim.SetFloat("VelocityY", _rbVelocity.y);
            _anim.SetBool("Grounded", charController.GetGrounded());
        }

        protected override void OnPathFoundListener(bool isOk) {
            goToTarget = isOk;
            //print($"[KID] Path found {GetPath().vectorPath.Count}");
        }

        protected override void OnWaypointChangeListener() {
            //print($"[KID] Waypoint changed! {GetPath().vectorPath.Count} {GetPathIndex()}");
        }

        protected override void OnPathEndListener() {
            goToTarget = false;
            //print("[KID] Path ended");
        }

        void AlignToNextWaypoint() {
            if (Vector2.Dot(transform.right, (GetPath().vectorPath[GetPathIndex()]) - transform.position) < 0) {
                charController.Turn180_2D();
            }
        }
    }
}