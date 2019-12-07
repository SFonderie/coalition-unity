using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public class CharControlSingle : MonoBehaviour
    {
        #pragma warning disable CS0649
        [SerializeField]
        G.Faction faction = G.Faction.neutral;
        [SerializeField]
        float moveSpeed = 2f;
        [SerializeField]
        int initiativeMod = 0;
        [SerializeField]
        int attackMod = 0;
        [SerializeField]
        int damageMod = 0;
        [SerializeField]
        int defenseMod = 0;
        [SerializeField]
        int armor = 0;
        [SerializeField]
        G.CombatAction[] combatActions;
        [SerializeField]
        int startFacingAngle = 0;
        [SerializeField]
        Sprite[] images;
        [SerializeField]
        Sprite portrait;
        #pragma warning restore CS0649
        
        bool canTurn = true;
        float facingAngle = 0;
        int initiative = 0;
        int attack = 0;
        int defense = 0;
        SpriteRenderer playerSprite, haloSprite, behindSprite, behindHalo;
        Vector2 moveHere, isoCoords, closeCoords;
        Color[] colorNeutral = new Color[] { new Color(0.5f, 0.5f, 0.5f), new Color(1, 1, 1) }, colorAlly = new Color[] { new Color(0, 0.5f, 0), new Color(0, 1, 0) }, colorEnemy = new Color[] { new Color(0.5f, 0, 0), new Color(1, 0, 0) };

        public float GetMoveSpeed()
        {
            return moveSpeed;
        }

        public G.Faction GetFaction()
        {
            return faction;
        }

        public G.CombatAction[] GetCombatActions()
        {
            return combatActions;
        }

        public Sprite GetPortrait()
        {
            return portrait;
        }

        public void SetInitiative(int i)
        {
            initiative = i;
            
            if (initiative < 1)
            {
                initiative = 1;
            }

            initiative += initiativeMod;
        }

        public int RollInitiative()
        {
            initiative = G.RandomInt(1, 20) + initiativeMod;
            return initiative;
        }

        public int GetInitiative()
        {
            return initiative;
        }

        public void SetAttack(int i)
        {
            attack = i;

            if (attack < 1)
            {
                attack = 1;
            }

            attack += attackMod;
        }

        public int RollAttack()
        {
            attack = G.RandomInt(1, 20) + attackMod;
            return attack;
        }

        public int GetAttack()
        {
            return attack;
        }

        public int GetDamageMod()
        {
            return damageMod;
        }

        public void SetDefense(int i)
        {
            defense = i;

            if (defense < 0)
            {
                defense = 0;
            }

            defense += defenseMod;
        }

        public int RollDefense()
        {
            defense = G.RandomInt(1, 20) + defenseMod;
            return defense;
        }

        public int GetDefense()
        {
            return defense;
        }

        public int GetArmor()
        {
            return armor;
        }

        public void AllowTurn(bool allow)
        {
            canTurn = allow;
        }

        public bool CanTurn()
        {
            return canTurn;
        }

        public float GetFacingAngle()
        {
            return facingAngle;
        }

        public Vector2 GetIsoCoords()
        {
            return isoCoords;
        }

        public Vector2 GetCloseCoords()
        {
            return closeCoords;
        }

        public void TurnToward(float x, float y, bool relative = true)
        {
            if (canTurn)
            {
                facingAngle = AngleToOther(x, y, relative);
                playerSprite.sprite = images[GetSpriteIndex(facingAngle)];
                behindSprite.sprite = images[GetSpriteIndex(facingAngle)];
            }
        }

        public void TurnToAngle(float degrees)
        {
            if (canTurn)
            {
                facingAngle = degrees;
                playerSprite.sprite = images[GetSpriteIndex(facingAngle)];
                behindSprite.sprite = images[GetSpriteIndex(facingAngle)];
            }
        }

        public void IsoSnap()
        {
            G.CartToNearestIso(transform.position.x, transform.position.y, ref isoCoords);
            G.IsoToCart(isoCoords.x, isoCoords.y, ref closeCoords);

            moveHere = new Vector2(closeCoords.x, closeCoords.y);
            transform.position = (Vector3) moveHere;
        }

        public void Move(float x, float y, bool smooth = true)
        {
            if (smooth)
            {
                moveHere = new Vector2(x, y * 0.5f);
                moveHere = moveHere.normalized * moveSpeed * Time.deltaTime;
                transform.position += (Vector3) moveHere;
            }
            else
            {
                moveHere = new Vector2(x, y);
                transform.position = (Vector3) moveHere;
            }

            G.CartToNearestIso(transform.position.x, transform.position.y, ref isoCoords);
            G.IsoToCart(isoCoords.x, isoCoords.y, ref closeCoords);

            TurnToward(x, y);
        }

        public void SetHaloActive(bool active)
        {
            switch (faction)
            {
                case G.Faction.neutral:
                    haloSprite.color = colorNeutral[Convert.ToInt32(active)];
                    break;
                case G.Faction.ally:
                    haloSprite.color = colorAlly[Convert.ToInt32(active)];
                    break;
                case G.Faction.enemy:
                    haloSprite.color = colorEnemy[Convert.ToInt32(active)];
                    break;
                default:
                    break;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            RollInitiative();

            playerSprite = GetComponent<SpriteRenderer>();
            if (transform.Find("Halo") != null)
            {
                haloSprite = transform.Find("Halo").GetComponent<SpriteRenderer>();
            }

            if (transform.Find("BehindWallSprite") != null)
            {
                behindSprite = transform.Find("BehindWallSprite").GetComponent<SpriteRenderer>();
            }

            if (transform.Find("BehindWallHalo") != null)
            {
                behindHalo = transform.Find("BehindWallHalo").GetComponent<SpriteRenderer>();
            }

            facingAngle = startFacingAngle;
            playerSprite.sprite = images[GetSpriteIndex(facingAngle)];
            behindSprite.sprite = images[GetSpriteIndex(facingAngle)];

            G.CartToNearestIso(transform.position.x, transform.position.y, ref isoCoords);
            G.IsoToCart(isoCoords.x, isoCoords.y, ref closeCoords);
        }

        // Update is called once per frame
        void Update()
        {

        }

        float AngleToOther(float x, float y, bool relative = true)
        {
            if (!relative)
            {
                x = (x - transform.position.x) * G.tileFactor.x;
                y = (y - transform.position.y) * G.tileFactor.y;
            }
            float angleFactor = Mathf.Abs(x / y);

            if (x == 0)  //  straight up or down
            {
                if (y < 0)  //  straight down
                {
                    return 270;
                }
                else if (y > 0)  //  straight up
                {
                    return 90;
                }
            }
            else if (y == 0)  //  straight left or right
            {
                if (x < 0)  //  straight left
                {
                    return 180;
                }
                else if (x > 0)  //  straight right
                {
                    return 0;
                }
            }
            else if (x < 0)  //  somewhere that's not straight up, down, left, or right
            {
                if (y < 0)  //  lower left quadrant
                {
                    if (0 < angleFactor && angleFactor <= 0.5)
                    {
                        return 270;
                    }
                    else if (0.5 < angleFactor && angleFactor <= 2)
                    {
                        return 225;
                    }
                    else
                    {
                        return 180;
                    }
                }
                else if (y > 0)  //  upper left quadrant
                {
                    if (0 < angleFactor && angleFactor <= 0.5)
                    {
                        return 90;
                    }
                    else if (0.5 < angleFactor && angleFactor <= 2)
                    {
                        return 135;
                    }
                    else
                    {
                        return 180;
                    }
                }
            }
            else if (x > 0)  //  right half
            {
                if (y < 0)  //  lower right quadrant
                {
                    if (0 < angleFactor && angleFactor <= 0.5)
                    {
                        return 270;
                    }
                    else if (0.5 < angleFactor && angleFactor <= 2)
                    {
                        return 315;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else if (y > 0)  //  upper right quadrant 
                {
                    if (0 < angleFactor && angleFactor <= 0.5)
                    {
                        return 90;
                    }
                    else if (0.5 < angleFactor && angleFactor <= 2)
                    {
                        return 45;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            return 0;
        }

        int GetSpriteIndex(float degrees)
        {
            int result = Mathf.RoundToInt(degrees / 45f);

            while (result < 0)
            {
                result += 8;
            }

            while (result > 7)
            {
                result -= 8;
            }

            return result;
        }
    }
}