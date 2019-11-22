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
        ContactFilter2D raycastFilter;
        [SerializeField]
        float waypointProximity = 0.1f;
        [SerializeField]
        Transform[] waypoints;
        #pragma warning restore CS0649
        CharControlSingle control;
        PolygonCollider2D visionCone;
        float moveSpeed = 0;
        int doMode = 0;
        Waypoint waypoint;
        //bool shouldRotate = false;
        float angle;
        float isoAngle;
        Vector2 facingDirection;
        int turnState = -1, i = 0;
        G.WaypointLookMode wpLookMode;
        float wpTimeBefore, wpTimeAfter, wpLookDirection, wpLookRange, wpLookSpeed, currentWait = 0;
        int wpLookCycles, currentCycle;
        RaycastHit2D[] raycastHits;
        [SerializeField]
        float sightDistance = 5f;
        [SerializeField]
        float sightAngle = 30f;
        Vector2[] sightPoints;

        public ContactFilter2D GetRaycastFilter()
        {
            return raycastFilter;
        }

        void Start()
        {
            control = GetComponent<CharControlSingle>();
            moveSpeed = control.GetMoveSpeed();
            visionCone = transform.Find("VisionCone").GetComponent<PolygonCollider2D>();
        }

        void Update()
        {
            facingDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

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
                            currentWait += Time.deltaTime;
                            if (currentWait >= wpTimeBefore)
                            {
                                if (wpLookMode != G.WaypointLookMode.none)
                                {
                                    angle = wpLookDirection;
                                    TurnCharacter();
                                    TurnVisionCone();
                                }

                                turnState = 0;
                                currentWait = 0;
                                currentCycle = 1;
                                doMode = 2;
                            }
                            break;
                        }
                        case 2:  //  look
                        {
                            if (wpLookMode == G.WaypointLookMode.sweep)
                            {
                                if (currentCycle <= wpLookCycles)
                                {
                                    switch (turnState)
                                    {
                                        case 0:
                                        {
                                            angle += wpLookSpeed * Time.deltaTime;
                                            if (angle >= wpLookDirection + (wpLookRange / 2))
                                            {
                                                turnState = 1;
                                            }
                                            break;
                                        }
                                        case 1:
                                        {
                                            angle -= wpLookSpeed * Time.deltaTime;
                                            if (angle <=  wpLookDirection - (wpLookRange / 2))
                                            {
                                                turnState = 2;
                                            }
                                            break;
                                        }
                                        case 2:
                                        {
                                            angle += wpLookSpeed * Time.deltaTime;
                                            if (angle >= wpLookDirection)
                                            {
                                                currentCycle++;
                                                turnState = 0;
                                            }
                                            break;
                                        }
                                        default:
                                        {
                                            turnState = 0;
                                            break;
                                        }
                                    }

                                    TurnCharacter();
                                    TurnVisionCone();
                                    /*if (turnState == 0)
                                    {

                                    }
                                    else if (turnState == 1)
                                    {

                                    }
                                    else if (turnState == 2)
                                    {

                                    }*/
                                }
                                else
                                {
                                    angle = wpLookDirection;
                                    currentCycle = 1;
                                    doMode = 3;
                                }
                            }
                            else
                            {
                                angle = wpLookDirection;
                                doMode = 3;
                            }
                            break;
                        }
                        case 3:  //  post-wait
                        {
                            currentWait += Time.deltaTime;
                            if (currentWait >= wpTimeAfter)
                            {
                                angle = wpLookDirection;
                                currentWait = 0;
                                doMode = 4;
                            }
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
                else/* if (!shouldRotate)*/
                {
                    control.Move(2 * Convert.ToInt32(transform.position.x < waypoints[i].transform.position.x) - 1, 2 * Convert.ToInt32(transform.position.y < waypoints[i].transform.position.y) - 1);
                    angle = control.GetFacingAngle();
                    TurnCharacter();
                    TurnVisionCone();
                }
            }
        }

        void FixedUpdate()
        {
            //TurnCharacter();
            //TurnVisionCone();
        }

        void TurnCharacter()
        {
            control.TurnToAngle(angle);
        }

        void TurnVisionCone()
        {
            sightPoints = visionCone.points;
            sightPoints[0].x = 0;
            sightPoints[0].y = 0;
            sightPoints[1].x = sightDistance * Mathf.Cos((angle + sightAngle / 2) * Mathf.Deg2Rad);
            sightPoints[1].y = sightDistance * Mathf.Sin((angle + sightAngle / 2) * Mathf.Deg2Rad) / 2;
            sightPoints[2].x = sightDistance * Mathf.Cos((angle - sightAngle / 2) * Mathf.Deg2Rad);
            sightPoints[2].y = sightDistance * Mathf.Sin((angle - sightAngle / 2) * Mathf.Deg2Rad) / 2;
            visionCone.points = sightPoints;
        }
    }
}