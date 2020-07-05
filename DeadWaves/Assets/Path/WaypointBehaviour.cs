using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeadWaves
{
    public class WaypointBehaviour : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col) {
            col.transform.root.GetComponentInChildren<IPathAgent>()?.ProcessWaypoint(transform.position);
        }
    }
}