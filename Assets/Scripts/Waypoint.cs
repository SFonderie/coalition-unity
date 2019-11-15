using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public class Waypoint : MonoBehaviour
    {
        #pragma warning disable CS0649
        [SerializeField]
        G.WaypointLookMode mode = G.WaypointLookMode.none;
        [SerializeField]
        float waitTimeBefore = 0;
        [SerializeField]
        float waitTimeAfter = 0;
        [SerializeField]
        float lookDirection = 0;
        [SerializeField]
        float lookRange = 0;
        [SerializeField]
        float lookSpeed = 10f;
        [SerializeField]
        int lookCycles = 1;
        #pragma warning restore CS0649

        void Awake()
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        public G.WaypointLookMode GetLookMode()
        {
            return mode;
        }

        public float GetWaitBefore()
        {
            return waitTimeBefore;
        }

        public float GetWaitAfter()
        {
            return waitTimeAfter;
        }
        
        public float GetLookDirection()
        {
            return lookDirection;
        }

        public float GetLookRange()
        {
            return lookRange;
        }

        public float GetLookSpeed()
        {
            return lookSpeed;
        }

        public int GetLookCycles()
        {
            return lookCycles;
        }
    }
}