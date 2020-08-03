using System;
using System.Collections;
using UnityEngine;

namespace DeadWaves
{
    [RequireComponent(typeof(CharacterInfo_Container), typeof(Rigidbody2D))]
    public class CharacterControl : MonoBehaviour
    {
        Rigidbody2D rb2D; //In case it ever changes to 3D movement someday
        CharacterInfo_Container character;

        //public LayerMask groundLayers;

        Vector2 groundPoint_2D, groundNormal_2D, normalAngle_2D;
        Vector2 jumpDir;

        float targetRotationY_2D;
        float accelTime;
        float jumpTime;

        public bool isTurning { get; protected set; }
        bool isMoving;
        bool isGrounded;
        bool moveCommandGiven;
        bool cueStopJump;

        public Action<RaycastHit2D> OnGroundEvent;

        private void Awake() {
            rb2D = GetComponentInChildren<Rigidbody2D>();
            character = GetComponent<CharacterInfo_Container>();

            targetRotationY_2D = transform.localEulerAngles.y;
        }

        private void OnEnable() {
            OnGroundEvent += OnGroundCollision;
        }

        private void OnDisable() {
            OnGroundEvent -= OnGroundCollision;
        }

        public float GetSpeed() => character.info.speed;
        public float GetJumpForce() => character.info.jumpForce;
        public float GetJumpMinTime() => character.info.minimumJumpTime;
        public float GetAcceleration() => character.info.accelerationCurve.Evaluate(Time.time - accelTime);

        public Vector2 GetVelocity() {
            Vector2 value = rb2D ? rb2D.velocity : Vector2.zero;
            return value;
        }

        public bool GetGrounded() => isGrounded;

        public void SetGrounded(bool value) {
            isGrounded = value;
            cueStopJump = false;
        }

        //Move command to, well, move the character to a direction with some force
        public void Move(Vector2 direction, ForceMode2D mode) {
            moveCommandGiven = true;
            //print("Move command issued!");
            if (isTurning) return;

            if (!isMoving) {
                isMoving = true;
                accelTime = Time.time;
            }

            rb2D.AddForce(direction * GetAcceleration() * GetSpeed(), mode);
        }

        public void Jump(bool useInertia = true, ForceMode2D mode = ForceMode2D.Impulse, bool overrideJumpDir = true) {
            if (overrideJumpDir) jumpDir = Vector2.up;
            if (!useInertia) rb2D.velocity = new Vector2(rb2D.velocity.x, 0f);

            rb2D.AddForce(jumpDir * GetJumpForce(), mode);
            jumpTime = Time.time;
        }

        public void Jump(Vector2 direction, bool useInertia = true, ForceMode2D mode = ForceMode2D.Impulse) {
            jumpDir = direction;
            Jump(useInertia, mode, false);
        }

        //Use this to make your character to stop the Jump (i.e. fall immediately when you want)
        public void StopJump() {
            if (rb2D.velocity.y <= 0) return;

            if (Time.time - jumpTime < GetJumpMinTime()) { 
                //This is called to make a minimum height of jump, 
                //Since we are dealing with forces and things done automatically by Unity, we have to use the Time
                
                cueStopJump = true;
                float timeToWait = GetJumpMinTime() - (Time.time - jumpTime);
                //Debug.Log($"[{gameObject.name} Char Control] Need to wait {timeToWait} sec to stop jump");
                Invoke("StopJump_Cued", timeToWait);
                return;
            }

            cueStopJump = false;
            rb2D.velocity = new Vector2(rb2D.velocity.x, 0f);
        }

        void StopJump_Cued() {
            if (!cueStopJump) return;
            StopJump();
        }

        //Called when the a collision is detected with the ground
        void SolveGround(GameObject groundObj, RaycastHit2D info) {
            //if (groundObj.layer != (groundObj.layer | (1 << groundLayers))) return;

            //groundNormal_2D = col.GetContact(0).normal;
            //groundPoint_2D = col.GetContact(0).point;
            groundNormal_2D = info.normal;
            groundPoint_2D = info.point;
        }

        //Calculates the forward direction based on the ground normal. Makes walking on slopes better.
        public Vector2 Get2D_GroundForwardDirection() {
            normalAngle_2D = Vector2.Perpendicular(groundNormal_2D);
            return normalAngle_2D * Math.Sign(Vector2.Dot(transform.right, normalAngle_2D));
        }

        void OnGroundCollision(RaycastHit2D collisionInfo) {
            //isGrounded = true;
            SolveGround(collisionInfo.collider.gameObject, collisionInfo);
        }

        public void RotateChar(Vector3 euler) {
            transform.localEulerAngles = euler;
        }

        public void Turn180_2D(float timeToTurn = 0f) {
            if (isTurning) return;

            if (timeToTurn == 0f) timeToTurn = character.info.turnSpeed;
            float currentY = SetTargetRotationY() == 0f ? 180f : 0f; //Sets the Y target rotation and also gets the current Y rotation. Nice
            StartCoroutine(Turn180Coroutine(timeToTurn, currentY, targetRotationY_2D));
        }

        float SetTargetRotationY() {
            targetRotationY_2D = targetRotationY_2D == 0f ? 180f : 0f;
            return targetRotationY_2D;
        }

        IEnumerator Turn180Coroutine(float timer, float start, float end) {
            float t = 0;
            Vector3 eulers = transform.localEulerAngles;

            isTurning = true;
            while (t <= timer) {
                eulers.y = character.info.turnWithoutAnimationSprite == true? Mathf.Lerp(start, end, t / timer) : end;
                t += Time.deltaTime;
                RotateChar(eulers);
                yield return null;
            }

            eulers.y = end;
            RotateChar(eulers); //Guarantees the Y rotation to be the exact number after it rotates. Avoids imprecision later
            isTurning = false;
        }

        public void StopCompletely() {
            rb2D.velocity = Vector2.zero;
        }

        private void FixedUpdate() {
            //Migue that worked. If the Move() isnt called, the moveCommandGiven will be false, then it means the char will be stopping moving, thus isMoving = false! This was how I managed to detect isMoving = false without making the char behaviour call something from there
            if (!moveCommandGiven) {
                isMoving = false;
            }
            moveCommandGiven = false;
        }

        private void Update() {
            //Debug
            DrawStuff();

            if (isTurning) print($"{gameObject.name} is turning");
        }

        //Debug
        void DrawStuff() {
            Debug.DrawLine(groundPoint_2D, groundPoint_2D + groundNormal_2D, Color.red);
            Debug.DrawLine(groundPoint_2D, groundPoint_2D + (Get2D_GroundForwardDirection() * 2), Color.blue);
            Debug.DrawLine(groundPoint_2D, groundPoint_2D + normalAngle_2D * 2, Color.green);
            if(contactDebug) contactDebug.position = groundPoint_2D;
        }

        public Transform contactDebug;
    }
}