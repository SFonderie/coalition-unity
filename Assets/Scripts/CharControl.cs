using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControl : MonoBehaviour {

    [SerializeField]
    float moveSpeed = 2.5f;
    
    [SerializeField]
    GameObject playerObj;

    [SerializeField]
    Sprite[] images;

    SpriteRenderer playerSprite;
    Vector3 moveHere;
    Vector2 isoCoords;
    float inputX, inputY, dX, dY;
    
    Vector2 tileExtents = new Vector2(0.5f, 0.25f);

	// Use this for initialization
	void Start () {
        playerSprite = playerObj.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        inputX = 0;
        inputY = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey)
        {
            Move();
        }
	}

    void OnGUI ()
    {
        Debug.Log("(isoX, isoY) = " + isoCoords);
    }

    float AngleToOther(float x, float y)
    {
        dX = x/* - transform.position.x*/;
        dY = y/* - transform.position.y*/;

        if (dX < 0)
        {
            if (dY < 0) //quadrant 3
            {
                return 180 + ((Mathf.Atan2(-1 * dX, -1 * dY)) * Mathf.Rad2Deg);
            }
            else if (dY == 0) //left
            {
                return 180;
            }
            else if (dY > 0) //quadrant 2
            {
                return 180 - ((Mathf.Atan2(-1 * dX, dY)) * Mathf.Rad2Deg);
            }
        }
        else if (dX == 0)
        {
            if (dY < 0) //down
            {
                return 270;
            }
            else if (dY > 0) //up
            {
                return 90;
            }
        }
        else if (dX > 0)
        {
            if (dY < 0) //quadrant 4
            {
                return 360 - ((Mathf.Atan2(dX, -1 * dY)) * Mathf.Rad2Deg);
            }
            else if (dY == 0) //right
            {
                return 0;
            }
            else if (dY > 0) //quadrant 1
            {
                return (Mathf.Atan2(dX, dY)) * Mathf.Rad2Deg;
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

    void Move()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        moveHere = new Vector3(inputX, inputY * 0.5f, 0);
        moveHere = moveHere.normalized * moveSpeed * Time.deltaTime;

        playerSprite.sprite = images[GetSpriteIndex(AngleToOther(inputX, inputY))];

        isoCoords = SpaceToIso(transform.position.x, transform.position.y);

        transform.position += moveHere;
    }

    Vector2 SpaceToIso(float x, float y)
    {
        Vector2 result = new Vector2(Mathf.RoundToInt((y / tileExtents.y - x / tileExtents.x) * 0.5f), Mathf.RoundToInt((x / tileExtents.x + y / tileExtents.y) * 0.5f));
        return result;
    }
}