using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DeadWaves
{
    [RequireComponent(typeof(CharacterControl))]
    public class CharacterBehaviour : MonoBehaviour, IPathAgent
    {
        protected CharacterControl charController;

        //General info --------------------------------------------------------------------------
        #region General Info
        
        public CharacterInfo TryGetCharacterInfo () {
            //This is a miguezao to make a character description available in the behaviour script, just to be a little more organized, because fuck me
            CharacterInfo_Container container = GetComponent<CharacterInfo_Container>();
            if (!container) return null;

            CharacterInfo info = container.info;

            if (!info) return null;
            else return info;
        }

        #endregion

        //PathFinding --------------------------------------------------------------------------
        #region PathFinding Variables and Methods

        Vector3 target, currentWaypoint;
        Seeker seeker;
        Path path;
        int pathIndex = 0;

        protected delegate void PathDoneDelegate(bool success);
        protected delegate void WaypointChangeDelegate();
        protected delegate void PathEndDelegate();

        protected event PathDoneDelegate OnPathDone;
        protected event WaypointChangeDelegate OnWaypointChange;
        protected event PathEndDelegate OnPathEnd;

        protected virtual void OnPathFoundListener(bool isOk) { }//Listener for children

        protected virtual void OnWaypointChangeListener() { }//Listener for children

        protected virtual void OnPathEndListener() { } //Listener for children

        protected Path GetPath() => path;
        protected int GetPathIndex() => pathIndex;

        protected void SetTarget(Vector3 value, bool startPath = false) {
            target = value;
            if (startPath) StartPath();
        }

        protected void StartPath() {
            if (seeker.IsDone()) {
                seeker.StartPath(transform.position, target, OnPathFoundCallback);
            }
        }
        protected void OnPathFoundCallback(Path p) {
            if (!p.error) {
                path = p;
                pathIndex = 0;
                IncreaseWaypoint();
            }
            if (OnPathDone != null) OnPathDone.Invoke(!p.error);
        }

        protected void IncreaseWaypoint() {
            pathIndex++;

            if (pathIndex >= path.vectorPath.Count) {
                path = null;
                OnPathEnd.Invoke();
                return;
            }

            currentWaypoint = path.vectorPath[pathIndex];
            OnWaypointChange.Invoke();
        }

        public void ProcessWaypoint(Vector3 waypointPosition) {
            if (path == null) return;

            if ((path.vectorPath[pathIndex] - waypointPosition).sqrMagnitude < 0.001f) IncreaseWaypoint();
        }

        [ContextMenu("Go to Next Waypoint")]
        public void Debug_IncreasePathIndex() {
            IncreaseWaypoint();
        }

        public Transform pathDebug;

        #endregion

        //Animation --------------------------------------------------------------------------
        #region Animation
        public Animator _anim;

        #endregion

        private void OnEnable() {
            //Pathfinding ------------------------------------
            OnPathDone += OnPathFoundListener;
            OnWaypointChange += OnWaypointChangeListener;
            OnPathEnd += OnPathEndListener;
        }

        private void OnDisable() {
            //Pathfinding -------------------------------------
            OnPathDone -= OnPathFoundListener;
            OnWaypointChange -= OnWaypointChangeListener;
            OnPathEnd -= OnPathEndListener;
        }

        protected virtual void Awake() {
            charController = GetComponent<CharacterControl>();

            //Pathfinding -------------------------------------
            seeker = GetComponent<Seeker>();

            //Animation ------------------------------------
            if (!_anim) _anim = GetComponentInChildren<Animator>();
        }

        protected virtual void Update() {
            //Pathfinding -------------------------------------
            if (path != null) {
                currentWaypoint.y = transform.position.y;
            }

            if(pathDebug) pathDebug.position = currentWaypoint;
        }
    }

    //Pathfinding -------------------------------------
    public interface IPathAgent
    {
        void ProcessWaypoint(Vector3 waypointPosition);
    }
}