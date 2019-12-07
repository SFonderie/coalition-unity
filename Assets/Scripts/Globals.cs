using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public static class G
    {
        public static Vector2 tileSize = new Vector2(1, 0.5f);
        public static Vector2 tileHalfSize = new Vector2(0.5f, 0.25f);
        public static Vector2 tileFactor = new Vector2(2, 4);

        public enum CombatState { none, combat };

        public enum MoveMode { none, click, free, attackTarget, attackArea, healTarget, healArea };

        public enum Faction { none = 42, neutral = 0, ally = 1, enemy = -1 };

        public enum WaypointLookMode { none, turn, sweep };

        public enum CombatActionType { empty, move, attackTarget, attackArea, healTarget, healArea };

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
            CombatActionType type = CombatActionType.empty;
            [SerializeField]
            string name = "CombatAction";
            [SerializeField]
            int range = 5;
            [SerializeField]
            int targets = 1;
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

            public int GetRange()
            {
                return range;
            }

            public int GetMaxTargets()
            {
                return targets;
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
        
        public static void Attack(GameObject attacker, GameObject[] targets, int damageMin, int damageMax)
        {
            CharControlSingle attackerScript = attacker.GetComponent<CharControlSingle>();

            foreach (GameObject target in targets)
            {
                CharControlSingle targetScript = target.GetComponent<CharControlSingle>();

                if (attackerScript.RollAttack() >= targetScript.RollDefense())
                {
                    Debug.Log(attacker.name + " hit " + target.name + " with an attack");

                    int damageDealt = RandomInt(damageMin, damageMax) - targetScript.GetArmor();

                    if (damageDealt < 0)
                    {
                        damageDealt = 0;
                    }

                    Debug.Log(attacker.name + " dealt " + damageDealt + " to " + target.name);
                }
                else
                {
                    Debug.Log(attacker.name + " missed " + target.name + " with an attack");
                }
            }
        }

        public static void Heal(GameObject caster, GameObject[] subjects, int healMin, int healMax)
        {
            foreach (GameObject subject in subjects)
            {
                CharControlSingle subjectScript = subject.GetComponent<CharControlSingle>();

                int healingDealt = RandomInt(healMin, healMax);

                Debug.Log(caster.name + " gave " + subject.name + " " + healingDealt + " points of healing");
            }
        }
    }
}
