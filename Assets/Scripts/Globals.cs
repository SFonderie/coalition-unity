using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public static class Globals
    {
        public static Vector2 tileSize = new Vector2(1, 0.5f);
        public static Vector2 tileHalfSize = new Vector2(0.5f, 0.25f);
        public static Vector2 tileFactor = new Vector2(2, 4);

        public enum CombatState { none, combat };

        public enum MoveMode { none, click, free };

        public enum Faction { neutral, ally, enemy };

        public static void CartToNearestIso(float cartX, float cartY, ref Vector2 vector)
        {
            vector = new Vector2(Mathf.RoundToInt((cartY / tileHalfSize.y - cartX / tileHalfSize.x) * 0.5f), Mathf.RoundToInt((cartX / tileHalfSize.x + cartY / tileHalfSize.y) * 0.5f));
        }

        public static void IsoToCart(float isoX, float isoY, ref Vector2 vector)
        {
            vector = new Vector2(tileHalfSize.x * ((-1 * isoX) + isoY), tileHalfSize.y * (isoX + isoY));
        }
    }
}
