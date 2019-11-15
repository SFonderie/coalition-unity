using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public class Patrol : MonoBehaviour
    {
        #pragma warning disable CS0649
        [SerializeField]
        float waypointProximity = 0.1f;
        [SerializeField]
        Transform[] waypoints;
        #pragma warning restore CS0649
        CharControlSingle control;
        float moveSpeed = 0;
        int doMode = 0;
        Waypoint waypoint;
        bool shouldRotate = false;
        float angle;
        int turnState = 0, i = 0;
        G.WaypointLookMode wpLookMode;
        float wpTimeBefore, wpTimeAfter, wpLookDirection, wpLookRange, wpLookSpeed, currentWait = 0;
        int wpLookCycles, currentCycle;

        void Start()
        {
            control = GetComponent<CharControlSingle>();
            moveSpeed = control.GetMoveSpeed();
        }

        void Update()
        {
            if (waypoints.Length > 0)
            {
                if (Vector2.Distance(transform.position, waypoints[i].position) < waypointProximity)
                {
                    switch (doMode)
                    {
                        case 0:  //  hit waypoint
                        {
                            control.IsoSnap();
                            waypoint = waypoints[i].GetComponent<Waypoint>();

                            wpLookMode = waypoint.GetLookMode();
                            wpTimeBefore = waypoint.GetWaitBefore();
                            wpTimeAfter = waypoint.GetWaitAfter();
                            wpLookDirection = waypoint.GetLookDirection();
                            wpLookRange = waypoint.GetLookRange();
                            wpLookSpeed = waypoint.GetLookSpeed();
                            wpLookCycles = waypoint.GetLookCycles();

                            doMode = 1;
                            break;
                        }
                        case 1:  //  pre-wait
                        {
                            if (currentWait >= wpTimeBefore)
                            {
                                if (wpLookMode != G.WaypointLookMode.none)
                                {
                                    angle = wpLookDirection;
                                    control.TurnToAngle(angle);
                                }

                                currentWait = 0;
                                currentCycle = 1;
                                doMode = 2;
                            }
                            currentWait += Time.deltaTime;
                            break;
                        }
                        case 2:  //  look
                        {
                            if (wpLookMode == G.WaypointLookMode.sweep)
                            {
                                if (currentCycle <= wpLookCycles)
                                {
                                    if (turnState == 0)
                                    {
                                        angle += wpLookSpeed * Time.deltaTime;
                                        if (angle >= wpLookDirection + (wpLookRange / 2))
                                        {
                                            turnState = 1;
                                        }
                                    }
                                    else if (turnState == 1)
                                    {
                                        angle -= wpLookSpeed * Time.deltaTime;
                                        if (angle <=  wpLookDirection - (wpLookRange / 2))
                                        {
                                            turnState = 2;
                                        }
                                    }
                                    else if (turnState == 2)
                                    {
                                        angle += wpLookSpeed * Time.deltaTime;
                                        if (angle >= wpLookDirection)
                                        {
                                            currentCycle++;
                                            turnState = 0;
                                        }
                                    }
                                    control.TurnToAngle(angle);
                                }
                                else
                                {
                                    currentCycle = 1;
                                    doMode = 3;
                                }
                            }
                            else
                            {
                                doMode = 3;
                            }
                            break;
                        }
                        case 3:  //  post-wait
                        {
                            if (currentWait >= wpTimeAfter)
                            {
                                currentWait = 0;
                                doMode = 4;
                            }
                            currentWait += Time.deltaTime;
                            break;
                        }
                        case 4:  //  next
                        {
                            if (i < waypoints.Length - 1)
                            {
                                i++;

                            }
                            else
                            {
                                i = 0;
                            }

                            doMode = 0;
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                }
                else if (!shouldRotate)
                {
                    control.Move(2 * Convert.ToInt32(transform.position.x < waypoints[i].transform.position.x) - 1, 2 * Convert.ToInt32(transform.position.y < waypoints[i].transform.position.y) - 1);
                }
            }
        }
    }
}