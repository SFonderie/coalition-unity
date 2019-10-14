using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlSingle : MonoBehaviour
{
    [SerializeField]
    int faction = 0;

    [SerializeField]
    float moveSpeed = 2f;

    [SerializeField]
    int startFacingAngle = 0;

#pragma warning disable CS0649
    [SerializeField]
    Sprite[] images;
#pragma warning restore CS0649

    [SerializeField]
    Vector2 tileHalfSize = new Vector2(0.5f, 0.25f);
    
    bool canTurn = true;
    SpriteRenderer playerSprite, haloSprite;
    Vector2 moveHere, isoCoords, closeCoords;
    Color[] colorNeutral = new Color[] { new Color(0.5f, 0.5f, 0.5f), new Color(1, 1, 1) }, colorAlly = new Color[] { new Color(0, 0.5f, 0), new Color(0, 1, 0) }, colorEnemy = new Color[] { new Color(0.5f, 0, 0), new Color(1, 0, 0) };

    public int GetFaction()
    {
        return faction;
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

    public void TurnToward(float x, float y)
    {
        if (canTurn)
        {
            playerSprite.sprite = images[GetSpriteIndex(AngleToOther(x, y))];
        }
    }

    public void IsoSnap()
    {
        UpdateSpecialCoords(ref isoCoords, ref closeCoords);

        moveHere = new Vector2(closeCoords.x, closeCoords.y);
        transform.position = (Vector3) moveHere;
    }

    public void Move(float x, float y)
    {
        UpdateSpecialCoords(ref isoCoords, ref closeCoords);

        moveHere = new Vector2(x, y * 0.5f);
        moveHere = moveHere.normalized * moveSpeed * Time.deltaTime;
        transform.position += (Vector3) moveHere;

        TurnToward(x, y);
    }

    public void SetHaloActive(bool active)
    {
        switch (faction)
        {
            case 0:
                haloSprite.color = colorNeutral[System.Convert.ToInt32(active)];
                break;
            case 1:
                haloSprite.color = colorAlly[System.Convert.ToInt32(active)];
                break;
            case 2:
                haloSprite.color = colorEnemy[System.Convert.ToInt32(active)];
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        if (transform.Find("Halo") != null)
        {
            haloSprite = transform.Find("Halo").GetComponent<SpriteRenderer>();
        }

        playerSprite.sprite = images[GetSpriteIndex(startFacingAngle)];

        UpdateSpecialCoords(ref isoCoords, ref closeCoords);
    }

    // Update is called once per frame
    void Update()
    {

    }

    float AngleToOther(float x, float y)
    {
        /*x = x - transform.position.x*/;
        /*y = y - transform.position.y*/;

        if (x < 0)
        {
            if (y < 0) //quadrant 3
            {
                return 180 + ((Mathf.Atan2(-1 * x, -1 * y)) * Mathf.Rad2Deg);
            }
            else if (y == 0) //left
            {
                return 180;
            }
            else if (y > 0) //quadrant 2
            {
                return 180 - ((Mathf.Atan2(-1 * x, y)) * Mathf.Rad2Deg);
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
                return 360 - ((Mathf.Atan2(x, -1 * y)) * Mathf.Rad2Deg);
            }
            else if (y == 0) //right
            {
                return 0;
            }
            else if (y > 0) //quadrant 1
            {
                return (Mathf.Atan2(x, y)) * Mathf.Rad2Deg;
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

    void UpdateSpecialCoords(ref Vector2 iso, ref Vector2 close)
    {
        iso = new Vector2(Mathf.RoundToInt((transform.position.y / tileHalfSize.y - transform.position.x / tileHalfSize.x) * 0.5f), Mathf.RoundToInt((transform.position.x / tileHalfSize.x + transform.position.y / tileHalfSize.y) * 0.5f));
        close = new Vector2(tileHalfSize.x * ((-1 * isoCoords.x) + isoCoords.y), tileHalfSize.y * (isoCoords.x + isoCoords.y));
    }
}