using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControl : MonoBehaviour {

    [SerializeField]
    float moveSpeed = 2.5f;

    [SerializeField]
    Sprite[] images;

    [SerializeField]
    Vector2 tileHalfSize = new Vector2(0.5f, 0.25f);

    bool canMove = true, canTurn = true, inputSnap = false;
    SpriteRenderer playerSprite;
    Rigidbody2D playerHandle;
    Vector2 moveHere, isoCoords, closeCoords;
    float inputX = 0, inputY = 0,dX, dY;

    public void AllowMove(bool allow)
    {
        canMove = allow;
    }

    public bool CanMove()
    {
        return canMove;
    }

    public void AllowTurn(bool allow)
    {
        canTurn = allow;
    }

    public bool CanTurn()
    {
        return canTurn;
    }

	// Use this for initialization
	void Start()
    {
        playerSprite = GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        playerHandle = GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
	}
	
	// Update is called once per frame
	void Update()
    {
		
	}

    // Like Update but for physics-related stuff
    void FixedUpdate()
    {
        if (Input.anyKey)
        {
            Move();
        }
    }

    void OnGUI()
    {
        Debug.Log("(x, y) = " + closeCoords.ToString("f2") + "    (isoX, isoY) = " + isoCoords.ToString("f0"));
    }

    void Move()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        inputSnap = Input.GetButtonDown("Jump");

        isoCoords = CartToIso(transform.position.x, transform.position.y);
        closeCoords = IsoToCart(isoCoords.x, isoCoords.y);

        if (inputSnap)
        {
            if (CanMove())
            {
                moveHere = new Vector2(closeCoords.x, closeCoords.y);
                transform.position = (Vector3) moveHere;
                AllowMove(false);
            }
            else
            {
                AllowMove(true);
            }
        }

        if (inputX != 0 || inputY != 0)
        {
            if (canMove)
            {
                moveHere = new Vector2(inputX, inputY * 0.5f);
                moveHere = moveHere.normalized * moveSpeed * Time.fixedDeltaTime;  //  fixedDeltaTime is like deltaTime but for FixedUpdate()
                playerHandle.MovePosition(playerHandle.position + moveHere);
            }

            if (canTurn)
            {
                playerSprite.sprite = images[GetSpriteIndex(AngleToOther(inputX, inputY))];
            }
        }
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

    Vector2 CartToIso(float x, float y)
    {
        return new Vector2(Mathf.RoundToInt((y / tileHalfSize.y - x / tileHalfSize.x) * 0.5f), Mathf.RoundToInt((x / tileHalfSize.x + y / tileHalfSize.y) * 0.5f));
    }

    Vector2 IsoToCart(float x, float y)
    {
        return new Vector2(tileHalfSize.x * ((-1 * x) + y), tileHalfSize.y * (x + y));
    }
}