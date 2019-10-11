using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlOverlord : MonoBehaviour {

    [SerializeField]
    GameObject[] players;

    int playerIndex = 0;
    GameObject playerObj;
    CharControlSingle playerScript;
    Rigidbody2D playerHandle;
    bool canMove = true;

    public void SetPlayer(GameObject player)
    {
        //Debug.Log("Selecting player at index " + playerIndex);

        if (!IsPlayerNull())
        {
            //  snap old player to nearest iso
            playerScript.IsoSnap();
            //  make old player immovable
            playerHandle.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // get new player
        playerObj = player;
        playerScript = playerObj.GetComponent(typeof(CharControlSingle)) as CharControlSingle;
        playerHandle = playerObj.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        //  make new player movable
        playerHandle.constraints = RigidbodyConstraints2D.FreezeRotation;

        (Camera.main.GetComponent(typeof(CameraFollow)) as CameraFollow).SetTarget(playerObj.transform);
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
        return (playerObj == null || playerScript == null || playerHandle == null);
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
            if (!IsPlayerNull())
            {
                playerScript.IsoSnap();
                playerScript.AllowTurn(!playerScript.CanTurn());
                AllowMove(!canMove);
            }
        }

        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !IsPlayerNull())
        {
            if (canMove)
            {
                playerScript.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }

            playerScript.TurnToward(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    void OnGUI()
    {
        if (!IsPlayerNull())
        {
            Debug.Log("(x, y) = " + playerScript.GetCloseCoords().ToString("f2") + "    (isoX, isoY) = " + playerScript.GetIsoCoords().ToString("f0"));
        }
    }

    void NextPlayer()
    {
        int startIndex = playerIndex;

        //  go through the array once looking for valid players
        do
        {
            ++playerIndex;

            //  look at the next player in the array
            if (playerIndex >= players.Length)
            {
                playerIndex = 0;
            }

            //  if they're not null, switch to them
            if (players[playerIndex] != null)
            {
                SetPlayer(players[playerIndex]);
                break;
            }
        } while (playerIndex != startIndex);
    }
}