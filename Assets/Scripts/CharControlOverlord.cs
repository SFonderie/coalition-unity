using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Coalition
{
    public class CharControlOverlord : MonoBehaviour
    {
        #pragma warning disable CS0649
        [SerializeField]
        GameObject tilemapGrid;

        [SerializeField]
        GUIStyle displayStyle;
        [SerializeField]
        float combatMoveDistance = 5f;

        [SerializeField]
        GameObject[] players;

        [SerializeField]
        GameObject[] enemies;
        #pragma warning restore CS0649
        
        Rect displayArea = new Rect(0, 0, Screen.width, Screen.height);
        string displayText;
        Tilemap[] tilemaps;
        int playerIndex = 0, combatRound = 1, combatants, activeCombatant = 0;
        GameObject playerObj;
        CharControlSingle playerScript;
        Rigidbody2D playerHandle;
        GameObject[] initiativeList;
        bool nonPlayerTurn = false;
        float nonPlayerTimer = 1;
        Vector3 mouseLocation;
        Vector2 mouseIso, mouseClose;
        SpriteRenderer mouseHalo;
        Color haloValidMove = Color.white, haloInvalidMove = new Color(0.75f, 0.75f, 0.75f);
        Globals.MoveMode moveMode = Globals.MoveMode./*free*/click;
        Globals.CombatState combatState = Globals.CombatState.none;
        Globals.Faction playerFaction = Globals.Faction.neutral;
        
        public void SetMoveMode(Globals.MoveMode mode)
        {
            if ((int) mode >= 0 && (int) mode <= 2)
            {
                mouseHalo.enabled = ((int) mode == 1);
                moveMode = mode;
            }
        }

        public void SetMoveMode(int mode)
        {
            if (mode >= 0 && mode <= 2)
            {
                mouseHalo.enabled = mode == 1;
                moveMode = (Globals.MoveMode) mode;
            }
        }

        public bool CanMove()
        {
            return (moveMode != Globals.MoveMode.none);
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
            playerFaction = playerScript.GetFaction();  //  get the new player's faction
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
            DebugLog("backspace = swap character    enter = start combat");

            tilemaps = new Tilemap[3];
            tilemaps[0] = tilemapGrid.transform.Find("Floor").GetComponent<Tilemap>();
            tilemaps[1] = tilemapGrid.transform.Find("Scenery").GetComponent<Tilemap>();
            tilemaps[2] = tilemapGrid.transform.Find("Walls").GetComponent<Tilemap>();

            mouseHalo = GetComponent<SpriteRenderer>();

            SetMoveMode(Globals.MoveMode./*free*/click);
            SetPlayer(players[playerIndex]);
        }
        
        // Update is called once per frame
        void Update()
        {
            switch (combatState)
            {
                case Globals.CombatState.none:
                    if (Input.GetButtonDown("DummyCombat"))  //  start combat round
                    {
                        AllIsoSnap();
                        SortInitiative();
                        DebugLog("Combat round " + combatRound + ": " + initiativeList[activeCombatant].name + "    (space = take turn    enter = end combat)");
                        SetPlayer(initiativeList[activeCombatant]);
                        if (playerFaction == Globals.Faction.ally)
                        {
                            SetMoveMode(Globals.MoveMode.click);
                        }
                        else
                        {
                            SetMoveMode(Globals.MoveMode.none);
                        }
                        combatState = Globals.CombatState.combat;
                    }
                    else if (Input.GetButtonDown("SwapCharacter"))  //  swap character out of combat
                    {
                        NextPlayer();
                    }
                    break;
                case Globals.CombatState.combat:
                    if (Input.GetButtonDown("DummyCombat"))  //  end combat
                    {
                        DebugLog("End combat: returning control to " + players[playerIndex].name);
                        SetPlayer(players[playerIndex]);
                        SetMoveMode(Globals.MoveMode./*free*/click);
                        combatRound = 1;
                        combatState = Globals.CombatState.none;
                    }
                    else if ((Input.GetButtonDown("DummyAction") && playerFaction == Globals.Faction.ally) || nonPlayerTurn)  //  active combatant takes their turn
                    {
                        nonPlayerTurn = false;

                        if (activeCombatant < initiativeList.Length - 1)  //  next combatant
                        {
                            activeCombatant++;
                        }
                        else  //  next round
                        {
                            activeCombatant = 0;
                            combatRound++;
                        }

                        SetPlayer(initiativeList[activeCombatant]);

                        if (playerFaction == Globals.Faction.ally)
                        {
                            DebugLog("Combat round " + combatRound + ": " + initiativeList[activeCombatant].name + "    (space = take turn    enter = end combat)");
                            SetMoveMode(Globals.MoveMode.click);
                        }
                        else
                        {
                            DebugLog("Combat round " + combatRound + ": " + initiativeList[activeCombatant].name + " is taking their turn");
                            SetMoveMode(Globals.MoveMode.none);
                        }
                    }
                    
                    if (playerFaction == Globals.Faction.enemy)  //  wait for a second to simulate enemy character's turn; remove this after actually implementing character actions
                    {
                        nonPlayerTimer -= Time.deltaTime;
                        if (nonPlayerTimer <= 0)
                        {
                            nonPlayerTurn = true;
                            nonPlayerTimer = 1;
                        }
                    }
                    break;
                default:
                    break;
            }

            switch (moveMode)
            {
                case Globals.MoveMode.none:
                    break;
                case Globals.MoveMode.free:
                    if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !IsPlayerNull())
                    {
                        playerScript.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                        playerScript.TurnToward(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                    }
                    break;
                case Globals.MoveMode.click:
                    mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Globals.CartToNearestIso(mouseLocation.x, mouseLocation.y, ref mouseIso);
                    Globals.IsoToCart(mouseIso.x, mouseIso.y, ref mouseClose);
                    transform.position = (Vector3) mouseClose;
                    playerScript.TurnToward(mouseClose.x, mouseClose.y, false);
                    if (mouseIso != playerScript.GetIsoCoords() && Vector3.Distance(playerScript.GetIsoCoords(), mouseIso) <= combatMoveDistance && tilemaps[0].GetTile((Vector3Int) tilemaps[0].WorldToCell(mouseClose)) != null && tilemaps[1].GetTile((Vector3Int) tilemaps[1].WorldToCell(mouseClose)) == null && tilemaps[2].GetTile((Vector3Int) tilemaps[2].WorldToCell(mouseClose)) == null)
                    {
                        mouseHalo.color = haloValidMove;
                        if (Input.GetButtonDown("Fire1"))
                        {
                            playerScript.Move(mouseClose.x, mouseClose.y, false);
                        }
                    }
                    else
                    {
                        mouseHalo.color = haloInvalidMove;
                    }
                    break;
                default:
                    break;
            }
        }

        void OnGUI()
        {
            GUI.Label(displayArea, displayText, displayStyle);
        }

        void DebugLog(string text)
        {
            displayText = text;
            Debug.Log(displayText);
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

            initiativeList = initiativeList.OrderByDescending(x => x.GetComponent<CharControlSingle>().RollInitiative()).ToArray();
        }
    }
}