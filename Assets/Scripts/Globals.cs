using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public static class G
    {
        static bool isPaused = false;

        public static void Pause()
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                isPaused = true;
            }
            else
            {
                Time.timeScale = 1;
                isPaused = false;
            }
        }

        public static bool IsPaused()
        {
            return isPaused;
        }

        public static Vector2 tileSize = new Vector2(1, 0.5f);
        public static Vector2 tileHalfSize = new Vector2(0.5f, 0.25f);
        public static Vector2 tileFactor = new Vector2(2, 4);

        public enum CombatState { none, combat };

        public enum MoveMode { none, click, free, attack, heal };

        public enum Faction { none = 42, neutral = 0, ally = 1, enemy = -1 };

        public enum WaypointLookMode { none, turn, sweep };

        public enum CombatActionType { none, attackTarget, healTarget, attackArea, healArea };

        [Serializable]
        public class DialogueData
        {
            #pragma warning disable CS0649
            [SerializeField]
            Sprite characterPortraitOverride;
            [SerializeField]
            string message;
            [SerializeField]
            bool stopMovement;
            [SerializeField]
            GameObject cameraTarget;
            #pragma warning restore CS0649

            public string GetText()
            {
                return message;
            }

            public Sprite GetPortrait()
            {
                if (cameraTarget != null)
                {
                    return cameraTarget.GetComponent<CharControlSingle>().GetPortrait();
                }
                else
                {
                    return null;
                }
            }

            public Sprite GetPortraitOverride()
            {
                return characterPortraitOverride;
            }

            public bool GetStop()
            {
                return stopMovement;
            }

            public GameObject GetTarget()
            {
                return cameraTarget;
            }
        }

        [Serializable]
        public class CombatAction
        {
            #pragma warning disable CS0649
            [SerializeField]
            CombatActionType type;
            [SerializeField]
            string name = "CombatAction";
            [SerializeField]
            float range = 5f;
            [SerializeField]
            int accuracy = 67;
            [SerializeField]
            int magnitudeMin = 0;
            [SerializeField]
            int magnitudeMax = 0;
            #pragma warning restore CS0649

            public CombatActionType GetActionType()
            {
                return type;
            }

            public string GetName()
            {
                return name;
            }

            public float GetRange()
            {
                return range;
            }

            public int GetAccuracy()
            {
                return accuracy;
            }

            public int GetMagnitudeMin()
            {
                return magnitudeMin;
            }

            public int GetMagnitudeMax()
            {
                return magnitudeMax;
            }
        }

        [Serializable]
        public class MoveAction
        {
            #pragma warning disable CS0649
            [SerializeField]
            float range = 5f;
            #pragma warning restore CS0649

            public float GetRange()
            {
                return range;
            }
        }

        public static int RandomInt(int start, int end)
        {
            return UnityEngine.Random.Range(start, end);
        }

        public static float RandomFloat(float start, float end)
        {
            return UnityEngine.Random.Range(start, end);
        }

        public static void CartToNearestIso(float cartX, float cartY, ref Vector2 vector)
        {
            vector = new Vector2(Mathf.RoundToInt((cartY / tileHalfSize.y - cartX / tileHalfSize.x) * 0.5f), Mathf.RoundToInt((cartX / tileHalfSize.x + cartY / tileHalfSize.y) * 0.5f));
        }

        public static void IsoToCart(float isoX, float isoY, ref Vector2 vector)
        {
            vector = new Vector2(tileHalfSize.x * ((-1 * isoX) + isoY), tileHalfSize.y * (isoX + isoY));
        }

        public static float EuclidToIso(float degrees)
        {
            return degrees - 15 * Mathf.Sin(Mathf.Deg2Rad * 2 * degrees);
        }

        public static float IsoAngleScale(float degrees, float coef)
        {
            return 1 + coef * Mathf.Pow(Mathf.Sin(degrees * Mathf.Deg2Rad), 2);
        }

        public static float IsoAngleScaleX(float degrees)
        {
            return IsoAngleScale(degrees, -0.5f);
        }

        public static float IsoAngleScaleY(float degrees)
        {
            return IsoAngleScale(degrees, 1f);
        }

        public static bool LineOfSight(Vector2 start, Vector2 end, ContactFilter2D raycastFilter)
        {
            return !Physics2D.Raycast(start, end - start, Vector2.Distance(start, end), raycastFilter.layerMask);
            
            /*RaycastHit2D[] hits = new RaycastHit2D[1];

            if (Physics2D.Raycast(start, end - start, raycastFilter, hits, Vector2.Distance(start, end)) == 0)
            {
                Debug.Log("line of sight is good");
                return true;
            }
            else
            {
                Debug.Log("line of sight is blocked by " + hits[0].collider.gameObject.name);
                return false;
            }*/
        }
        
        public static void UseCombatAction(CharControlSingle userScript, CharControlSingle targetScript, CombatAction action)
        {
            int magnitude;

            if (action.GetActionType() == CombatActionType.attackTarget)
            {
                if (RandomInt(1, 100) <= action.GetAccuracy())
                {
                    magnitude = RandomInt(action.GetMagnitudeMin(), action.GetMagnitudeMax());
                    targetScript.Damage(magnitude);
                    Debug.Log(userScript.gameObject.name + " hit " + targetScript.gameObject.name + " with their attack for " + magnitude + " damage");
                }
                else
                {
                    Debug.Log(userScript.gameObject.name + " missed " + targetScript.gameObject.name + " with their attack");
                }
            }
            else if (action.GetActionType() == CombatActionType.healTarget)
            {
                magnitude = RandomInt(action.GetMagnitudeMin(), action.GetMagnitudeMax());
                targetScript.Heal(magnitude);
                Debug.Log(userScript.gameObject.name + " healed " + targetScript.gameObject.name + " for " + magnitude + " health");
            }
        }
    }
}
