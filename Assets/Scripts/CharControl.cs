using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControl : MonoBehaviour {

    [SerializeField]
    GameObject[] players;

    [SerializeField]
    float moveSpeed = 2f;

    [SerializeField]
    Vector2 tileHalfSize = new Vector2(0.5f, 0.25f);

    int playerIndex = 0;
    GameObject playerObj;
    SpriteControl playerSprite;
    Rigidbody2D playerHandle;
    bool canMove = true;
    Vector2 moveHere, isoCoords, closeCoords;

    public void SetPlayer(GameObject player)
    {
        //Debug.Log("Selecting player at index " + playerIndex);

        if (player != null)
        {
            if (!IsPlayerNull())
            {
                //  snap old player to nearest iso
                IsoSnap(false);
                //  make old player immovable
                playerHandle.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            // get new player
            playerObj = player;
            playerSprite = playerObj.GetComponent(typeof(SpriteControl)) as SpriteControl;
            playerHandle = playerObj.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            //  make new player movable
            playerHandle.constraints = RigidbodyConstraints2D.FreezeRotation;

            isoCoords = CartToIso(playerObj.transform.position.x, playerObj.transform.position.y);
            closeCoords = IsoToCart(isoCoords.x, isoCoords.y);

            (Camera.main.GetComponent(typeof(CameraFollow)) as CameraFollow).SetTarget(playerObj.transform);
        }
    }

    public void AllowMove(bool allow)
    {
        canMove = allow;
    }

    public bool CanMove()
    {
        return canMove;
    }

    public bool IsPlayerNull()
    {
        return (playerObj == null || playerSprite == null || playerHandle == null);
    }

	// Use this for initialization
	void Start()
    {
        SetPlayer(players[playerIndex]);
    }
	
	// Update is called once per frame
	void Update()
    {
        if (Input.GetButtonDown("SwapCharacter"))
        {
            NextPlayer();
        }

        if (Input.GetButtonDown("SnapToIso"))
        {
            IsoSnap();
        }
    }

    // Like Update but for physics-related stuff
    void FixedUpdate()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    void OnGUI()
    {
        //Debug.Log("(x, y) = " + closeCoords.ToString("f2") + "    (isoX, isoY) = " + isoCoords.ToString("f0"));
    }

    void NextPlayer()
    {
        int startIndex = playerIndex;
        ++playerIndex;

        //  go through the array once looking for valid players
        while (playerIndex != startIndex)
        {
            //  look at the next player in the array
            if (playerIndex >= players.Length)
            {
                playerIndex = 0;
            }
            else
            {
                ++playerIndex;
            }

            //  if they're not null, switch to them
            if (players[playerIndex] != null)
            {
                SetPlayer(players[playerIndex]);
                break;
            }
        }
    }

    void IsoSnap(bool freeze = true)
    {
        isoCoords = CartToIso(playerObj.transform.position.x, playerObj.transform.position.y);
        closeCoords = IsoToCart(isoCoords.x, isoCoords.y);

        moveHere = new Vector2(closeCoords.x, closeCoords.y);
        playerObj.transform.position = (Vector3) moveHere;

        if (freeze)
        {
            AllowMove(!canMove);
        }
    }

    void Move(float x, float y)
    {
        isoCoords = CartToIso(playerObj.transform.position.x, playerObj.transform.position.y);
        closeCoords = IsoToCart(isoCoords.x, isoCoords.y);

        if (canMove)
        {
            moveHere = new Vector2(x, y * 0.5f);
            moveHere = moveHere.normalized * moveSpeed * Time.fixedDeltaTime;  //  fixedDeltaTime is like deltaTime but for FixedUpdate()
            playerHandle.MovePosition(playerHandle.position + moveHere);
        }

        playerSprite.TurnToward(x, y);
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