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

        public enum MoveMode { none, click, free };

        public enum Faction { neutral, ally, enemy };

        public enum WaypointLookMode { none, turn, sweep };

        public enum CombatAction { empty, basicAttack };

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
    }
}
