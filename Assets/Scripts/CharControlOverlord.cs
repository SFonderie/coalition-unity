using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharControlOverlord : MonoBehaviour {

#pragma warning disable CS0649
    [SerializeField]
    GameObject[] players;

    [SerializeField]
    GameObject[] enemies;
#pragma warning restore CS0649

    int playerIndex = 0, combatRound = 1, combatants, activeCombatant = 0;
    GameObject playerObj;
    CharControlSingle playerScript;
    Rigidbody2D playerHandle;
    bool canMove = true;
    GameObject[] initiativeList;

    enum combatState { none, combat };
    combatState stage = combatState.none;

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);
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
    public void SetPlayer(GameObject player)
    {
        if (!IsPlayerNull())
        {
            //  snap old player to nearest iso
            playerScript.IsoSnap();
            //  toggle off pld player's halo
            playerScript.SetHaloActive(false);
            //  make old player immovable
            playerHandle.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // get new player
        playerObj = player;
        playerScript = playerObj.GetComponent<CharControlSingle>();
        playerScript.SetHaloActive(true);  //  toggle on new player's halo
        playerHandle = playerObj.GetComponent<Rigidbody2D>();
        playerHandle.constraints = RigidbodyConstraints2D.FreezeRotation;  //  make new player movable

        Camera.main.GetComponent<CameraFollow>().SetTarget(playerObj.transform);
    }

    public void AllIsoSnap()
    {
        foreach (GameObject p in players)
        {
            if (p != null)
            {
                p.GetComponent<CharControlSingle>().IsoSnap();
            }
        }

        foreach (GameObject e in enemies)
        {
            if (e != null)
            {
                e.GetComponent<CharControlSingle>().IsoSnap();
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        SetPlayer(players[playerIndex]);

        Debug.Log("backspace = swap character    enter = start combat");
    }
	
	// Update is called once per frame
	void Update()
    {
        switch (stage)
        {
            case combatState.none:
                if (Input.GetButtonDown("DummyCombat"))  //  start combat round
                {
                    AllIsoSnap();
                    AllowMove(false);
                    SortInitiative();
                    Debug.Log("Combat round " + combatRound + ": " + initiativeList[activeCombatant].name + "    (space = take turn    enter = end combat)");
                    SetPlayer(initiativeList[activeCombatant]);
                    stage = combatState.combat;
                }
                else if (Input.GetButtonDown("SwapCharacter"))  //  swap character out of combat
                {
                    NextPlayer();
                }

                    break;
            case combatState.combat:
                if (Input.GetButtonDown("DummyCombat"))  //  end combat
                {
                    Debug.Log("End combat: returning control to " + players[playerIndex].name);
                    SetPlayer(players[playerIndex]);
                    AllowMove(true);
                    combatRound = 1;
                    stage = combatState.none;
                }
                else if (Input.GetButtonDown("DummyAction"))  //  active combatant takes their turn
                {
                    if (activeCombatant < initiativeList.Length - 1)  //  next combatant
                    {
                        activeCombatant++;
                    }
                    else  //  next round
                    {
                        activeCombatant = 0;
                        combatRound++;
                    }

                    Debug.Log("Combat round " + combatRound + ": " + initiativeList[activeCombatant].name + "    (space = take turn    enter = end combat)");
                    SetPlayer(initiativeList[activeCombatant]);
                }

                break;
            default:
                break;
        }

        /*if (Input.GetButtonDown("IsoSnap"))
        {
            if (!IsPlayerNull())
            {
                playerScript.IsoSnap();
                playerScript.AllowTurn(!playerScript.CanTurn());
                AllowMove(!canMove);
            }
        }*/

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
        /*if (!IsPlayerNull())
        {
            Debug.Log("(x, y) = " + playerScript.GetCloseCoords().ToString("f2") + "    (isoX, isoY) = " + playerScript.GetIsoCoords().ToString("f0"));
        }*/
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

    void SortInitiative()
    {
        //  just a dummy function for now, no actual initiative is calculated
        combatants = players.Count(p => p != null) + enemies.Count(e => e != null);
        initiativeList = new GameObject[combatants];

        activeCombatant = 0;

        //  fill initiative list
        foreach (GameObject p in players)
        {
            if (p != null)
            {
                initiativeList[activeCombatant] = p;
                activeCombatant++;
            }
        }

        foreach (GameObject e in enemies)
        {
            if (e != null)
            {
                initiativeList[activeCombatant] = e;
                activeCombatant++;
            }
        }

        activeCombatant = 0;
    }
}