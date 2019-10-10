using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteControl : MonoBehaviour
{
    [SerializeField]
    Sprite[] images;

    bool canTurn = true;
    SpriteRenderer playerSprite;

    public void AllowTurn(bool allow)
    {
        canTurn = allow;
    }

    public bool CanTurn()
    {
        return canTurn;
    }

    public void TurnToward(float x, float y)
    {
        if (canTurn)
        {
            playerSprite.sprite = images[GetSpriteIndex(AngleToOther(x, y))];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerSprite = GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
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
}
