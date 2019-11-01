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
        Color movementHaloValidColor = Color.white;
        [SerializeField]
        Color movementHaloInvalidColor = new Color(0.625f, 0.625f, 0.625f);
        [SerializeField]
        GUIStyle displayStyle;
        [SerializeField]
        float combatMoveDistance = 5f;
        [SerializeField]
        GameObject[] players;
        [SerializeField]
        GameObject[] enemies;
        [SerializeField]
        Canvas dialogueCanvas;
        //[SerializeField]
        //Sprite dialogueBackgroundTexture;
        #pragma warning restore CS0649
        DialogueCanvas dialogueScript;
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
        Collider2D mouseHaloCollider;
        Globals.MoveMode moveMode = Globals.MoveMode.free;
        Globals.CombatState combatState = Globals.CombatState.none;
        Globals.Faction playerFaction = Globals.Faction.neutral;
        
        public void SetMoveMode(Globals.MoveMode mode)
        {
            mouseHalo.enabled = (mode == Globals.MoveMode.click);
            moveMode = mode;
        }

        public void SetMoveMode(int mode)
        {
            if (System.Enum.IsDefined(typeof(Globals.MoveMode), mode))
            {
                SetMoveMode((Globals.MoveMode) mode);
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
                //  toggle old player's halo to dim mode
                playerScript.SetHaloActive(false);
                //  make old player immovable
                playerHandle.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            // get new player
            playerObj = player;
            playerScript = playerObj.GetComponent<CharControlSingle>();
            playerScript.SetHaloActive(true);  //  toggle new player's halo to bright mode
            playerFaction = playerScript.GetFaction();  //  get the new player's faction
            playerHandle = playerObj.GetComponent<Rigidbody2D>();
            playerHandle.constraints = RigidbodyConstraints2D.FreezeRotation;  //  make new player movable

            Camera.main.GetComponent<CameraControl>().SetTarget(playerObj.transform);
        }

        public GameObject GetPlayer()
        {
            return playerObj;
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
            dialogueScript = dialogueCanvas.GetComponent<DialogueCanvas>();
            dialogueScript.HideMessage();

            DebugLog("backspace = swap character    enter = start combat");

            mouseHalo = GetComponent<SpriteRenderer>();
            mouseHaloCollider = GetComponent<Collider2D>();

            SetMoveMode(Globals.MoveMode.free);
            SetPlayer(players[playerIndex]);
        }

        void FixedUpdate()
        {
            
        }
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Fire3"))
            {
                if (dialogueScript.IsEnabled())
                {
                    dialogueScript.HideMessage();
                }
                else
                {
                    dialogueScript.DisplayMessage(playerObj.name + " says hello", playerScript.GetPortrait());
                }
            }

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
                        SetMoveMode(Globals.MoveMode.free);
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
                    //  get the world location that the mouse is pointing to
                    mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Globals.CartToNearestIso(mouseLocation.x, mouseLocation.y, ref mouseIso);
                    Globals.IsoToCart(mouseIso.x, mouseIso.y, ref mouseClose);
                    //  place the movement halo there
                    transform.position = (Vector3) mouseClose;
                    //  have the active character turn to look at that spot
                    playerScript.TurnToward(mouseClose.x, mouseClose.y, false);
                    //  hide the halo if there is no floor tile there
                    mouseHalo.enabled = mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Floor"));
                    //  set the halo brightness and actually allow movement if the spot is valid
                    if (mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Floor")) && !mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Scenery")) && Vector2.Distance(playerScript.GetIsoCoords(), mouseIso) <= combatMoveDistance)
                    {
                        mouseHalo.color = movementHaloValidColor;
                        if (Input.GetButtonDown("Fire1"))
                        {
                            playerScript.Move(mouseClose.x, mouseClose.y, false);
                        }
                    }
                    else
                    {
                        mouseHalo.color = movementHaloInvalidColor;
                    }
                    break;
                default:
                    break;
            }
        }

        void OnGUI()
        {
            
        }

        void DebugLog(string text)
        {
            //displayText = text;
            Debug.Log(text);
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