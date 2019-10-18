using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public class CharControlSingle : MonoBehaviour
    {
        [SerializeField]
        Globals.Faction faction = Globals.Faction.neutral;

        [SerializeField]
        float moveSpeed = 2f;

        [SerializeField]
        int initiativeMod = 0;

        [SerializeField]
        int startFacingAngle = 0;

        #pragma warning disable CS0649
        [SerializeField]
        Sprite[] images;
        #pragma warning restore CS0649
        
        bool canTurn = true;
        int initiative = 0;
        SpriteRenderer playerSprite, haloSprite;
        Vector2 moveHere, isoCoords, closeCoords;
        Color[] colorNeutral = new Color[] { new Color(0.5f, 0.5f, 0.5f), new Color(1, 1, 1) }, colorAlly = new Color[] { new Color(0, 0.5f, 0), new Color(0, 1, 0) }, colorEnemy = new Color[] { new Color(0.5f, 0, 0), new Color(1, 0, 0) };

        public Globals.Faction GetFaction()
        {
            return faction;
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
            initiative = Random.Range(1, 20) + initiativeMod;
            return initiative;
        }

        public int GetInitiative()
        {
            return initiative;
        }

        public void AllowTurn(bool allow)
        {
            canTurn = allow;
        }

        public bool CanTurn()
        {
            return canTurn;
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
                playerSprite.sprite = images[GetSpriteIndex(AngleToOther(x, y, relative))];
            }
        }

        public void IsoSnap()
        {
            Globals.CartToNearestIso(transform.position.x, transform.position.y, ref isoCoords);
            Globals.IsoToCart(isoCoords.x, isoCoords.y, ref closeCoords);

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

            Globals.CartToNearestIso(transform.position.x, transform.position.y, ref isoCoords);
            Globals.IsoToCart(isoCoords.x, isoCoords.y, ref closeCoords);

            TurnToward(x, y);
        }

        public void SetHaloActive(bool active)
        {
            switch (faction)
            {
                case Globals.Faction.neutral:
                    haloSprite.color = colorNeutral[System.Convert.ToInt32(active)];
                    break;
                case Globals.Faction.ally:
                    haloSprite.color = colorAlly[System.Convert.ToInt32(active)];
                    break;
                case Globals.Faction.enemy:
                    haloSprite.color = colorEnemy[System.Convert.ToInt32(active)];
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

            playerSprite.sprite = images[GetSpriteIndex(startFacingAngle)];

            Globals.CartToNearestIso(transform.position.x, transform.position.y, ref isoCoords);
            Globals.IsoToCart(isoCoords.x, isoCoords.y, ref closeCoords);
        }

        // Update is called once per frame
        void Update()
        {

        }

        float AngleToOther(float x, float y, bool relative = true)
        {
            if (!relative)
            {
                x = (x - transform.position.x) * Globals.tileFactor.x;
                y = (y - transform.position.y) * Globals.tileFactor.y;
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

            /*if (x < 0)
            {
                if (y < 0) //quadrant 3
                {
                    if (0 < newThing && newThing <= 0.5)
                    {
                        return 90;
                    }
                    else if (0.5 < newThing && newThing <= 2)
                    {
                        return 45;
                    }
                    else
                    {
                        return 0;
                    }
                    //return 180 + ((Mathf.Atan2(-1 * x, -1 * y)) * Mathf.Rad2Deg);
                }
                else if (y == 0) //left
                {
                    return 180;
                }
                else if (y > 0) //quadrant 2
                {
                    if (0 < newThing && newThing <= 0.5)
                    {
                        return 90;
                    }
                    else if (0.5 < newThing && newThing <= 2)
                    {
                        return 45;
                    }
                    else
                    {
                        return 0;
                    }
                    //return 180 - ((Mathf.Atan2(-1 * x, y)) * Mathf.Rad2Deg);
                }
            }
            else if (x == 0)
            {
                if (y < 0) //down
                {
                    return 270;
                }
                else if (y > 0) //up
                {
                    return 90;
                }
            }
            else if (x > 0)
            {
                if (y < 0) //quadrant 4
                {
                    if (0 < newThing && newThing <= 0.5)
                    {
                        return 90;
                    }
                    else if (0.5 < newThing && newThing <= 2)
                    {
                        return 45;
                    }
                    else
                    {
                        return 0;
                    }
                    //return 360 - ((Mathf.Atan2(x, -1 * y)) * Mathf.Rad2Deg);
                }
                else if (y == 0) //right
                {
                    return 0;
                }
                else if (y > 0) //quadrant 1
                {
                    if (0 < newThing && newThing <= 0.5)
                    {
                        return 90;
                    }
                    else if (0.5 < newThing && newThing <= 2)
                    {
                        return 45;
                    }
                    else
                    {
                        return 0;
                    }
                    //return (Mathf.Atan2(x, y)) * Mathf.Rad2Deg;
                }
            }*/
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