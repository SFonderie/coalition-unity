using System;
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
        Sprite movementHalo;
        [SerializeField]
        Color movementHaloValidColor = Color.white;
        [SerializeField]
        Color movementHaloInvalidColor = new Color(0.625f, 0.625f, 0.625f);
        [SerializeField]
        Sprite targetHalo;
        [SerializeField]
        Color targetHaloAttackColor = Color.red;
        [SerializeField]
        Color targetHaloHealColor = Color.green;
        [SerializeField]
        Color targetHaloInvalidColor = new Color(0.625f, 0.625f, 0.625f);
        [SerializeField]
        GUIStyle displayStyle;
        [SerializeField]
        GameObject[] players;
        [SerializeField]
        Canvas actionCanvas;
        [SerializeField]
        Canvas dialogueCanvas;
        #pragma warning restore CS0649
        GameObject[] enemies;
        ActionCanvas actionScript;
        DialogueCanvas dialogueScript;
        Tilemap[] tilemaps;
        int playerIndex = 0, combatRound = 1, combatants, activeCombatant = 0;
        float combatMoveDistance = 5f;
        float distanceMoved;
        GameObject playerObj;
        CharControlSingle playerScript;
        Rigidbody2D playerHandle;
        GameObject[] initiativeList;
        bool nonPlayerTurn = false;
        float nonPlayerTimer = 1;
        Color mouseHaloBase, mouseHaloInvalid;
        Vector3 mouseLocation;
        Vector2 mouseIso, mouseClose;
        SpriteRenderer mouseHalo;
        SpriteRenderer mouseHaloBehindWalls;
        Collider2D mouseHaloCollider;
        TriggerMouse mouseScript;
        G.MoveMode moveMode = G.MoveMode.free;
        G.CombatState combatState = G.CombatState.none;
        G.Faction playerFaction = G.Faction.neutral;

        public G.MoveMode GetMoveMode()
        {
            return moveMode;
        }
        
        public void SetMoveMode(G.MoveMode mode)
        {
            moveMode = mode;

            if (mode == G.MoveMode.none || mode == G.MoveMode.click || mode == G.MoveMode.free)
            {
                mouseHalo.sprite = movementHalo;
                mouseHaloBehindWalls.sprite = movementHalo;
                mouseHaloBase = movementHaloValidColor;
                mouseHaloInvalid = movementHaloInvalidColor;
            }
            else
            {
                mouseHalo.sprite = targetHalo;
                mouseHaloBehindWalls.sprite = targetHalo;

                if (mode == G.MoveMode.attackTarget || mode == G.MoveMode.attackArea)
                {
                    mouseHaloBase = targetHaloAttackColor;
                    mouseHaloInvalid = targetHaloInvalidColor;
                }
                else
                {
                    mouseHaloBase = targetHaloHealColor;
                    mouseHaloInvalid = targetHaloInvalidColor;
                }
            }

            mouseHalo.enabled = CanAim();
            mouseHaloBehindWalls.enabled = CanAim();
        }

        public void SetMoveMode(int mode)
        {
            if (Enum.IsDefined(typeof(G.MoveMode), mode))
            {
                SetMoveMode((G.MoveMode) mode);
            }
        }

        public bool CanMove()
        {
            return (moveMode != G.MoveMode.none);
        }

        public bool CanAim()
        {
            return (moveMode != G.MoveMode.none && moveMode != G.MoveMode.free);
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
            
            foreach (G.CombatAction action in playerScript.GetCombatActions())
            {
                if (action.GetActionType() == G.CombatActionType.move)
                {
                    combatMoveDistance = action.GetRange();
                }
            }

            distanceMoved = 0;

            Camera.main.GetComponent<CameraControl>().SetTarget(playerObj);
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

        public void StartCombat(ref GameObject[] enemyTeam)
        {
            enemies = new GameObject[enemyTeam.Length];
            for (int i = 0; i < enemyTeam.Length; i++)
            {
                enemies[i] = enemyTeam[i];
            }
            AllIsoSnap();
            SortInitiative();
            SetPlayer(initiativeList[activeCombatant]);
            AlignCombat();
            combatState = G.CombatState.combat;
        }

        public void ClearCombat()
        {
            //Debug.Log("End combat: returning control to " + players[playerIndex].name);
            SetPlayer(players[playerIndex]);
            SetMoveMode(G.MoveMode.free);
            combatRound = 1;
            combatState = G.CombatState.none;

            enemies = new GameObject[0];
        }

        // Use this for initialization
        void Start()
        {
            actionScript = actionCanvas.GetComponent<ActionCanvas>();

            dialogueScript = dialogueCanvas.GetComponent<DialogueCanvas>();
            dialogueScript.HideMessage();

            mouseHalo = GetComponent<SpriteRenderer>();
            mouseHaloCollider = GetComponent<Collider2D>();
            mouseScript = GetComponent<TriggerMouse>();

            mouseHaloBehindWalls = transform.Find("BehindWallSprite").GetComponent<SpriteRenderer>();

            SetMoveMode(G.MoveMode.free);
            SetPlayer(players[playerIndex]);
        }
        
        // Update is called once per frame
        void Update()
        {
            switch (combatState)
            {
                case G.CombatState.none:
                {
                    if (Input.GetButtonDown("SwapCharacter"))  //  swap character out of combat
                    {
                        NextPlayer();
                    }
                    break;
                }
                case G.CombatState.combat:
                {
                    if (Input.GetButtonDown("DummyCombat"))  //  end combat
                    {
                        ClearCombat();
                    }
                    else if ((Input.GetButtonDown("DummyAction") && playerFaction == G.Faction.ally) || nonPlayerTurn)  //  active combatant takes their turn
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

                        AlignCombat();
                    }
                    
                    if (playerFaction == G.Faction.enemy)  //  wait for a second to simulate enemy character's turn; remove this after actually implementing character actions
                    {
                        nonPlayerTimer -= Time.deltaTime;
                        if (nonPlayerTimer <= 0)
                        {
                            nonPlayerTurn = true;
                            nonPlayerTimer = 1;
                        }
                    }
                    else if (Input.GetButtonDown("Fire2"))  //  let player cancel their action
                    {
                        SetMoveMode(G.MoveMode.none);
                    }

                    break;
                }
                default:
                {
                    break;
                }
            }

            if (CanAim())
            {
                //  get the world location that the mouse is pointing to
                mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                G.CartToNearestIso(mouseLocation.x, mouseLocation.y, ref mouseIso);
                G.IsoToCart(mouseIso.x, mouseIso.y, ref mouseClose);
                //  place the movement halo there
                transform.position = (Vector3) mouseClose;
                //  have the active character turn to look at that spot
                playerScript.TurnToward(mouseClose.x, mouseClose.y, false);
                //  hide the halo if there is no floor tile there
                mouseHalo.enabled = mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Floor"));
                mouseHaloBehindWalls.enabled = mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Floor"));
            }


            if (moveMode == G.MoveMode.click)
            {
                //  set the halo brightness and actually allow movement if the spot is valid
                if (mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Floor")) && !mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Scenery")) && !mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Characters")) && Vector2.Distance(playerScript.GetIsoCoords(), mouseIso) <= combatMoveDistance - distanceMoved && distanceMoved < combatMoveDistance)
                {
                    mouseHalo.color = mouseHaloBase;
                    mouseHaloBehindWalls.color = mouseHaloBase;
                    if (Input.GetButtonDown("Fire1"))
                    {
                        distanceMoved += Vector2.Distance(playerScript.GetIsoCoords(), mouseIso);
                        playerScript.Move(mouseClose.x, mouseClose.y, false);
                    }
                }
                else
                {
                    mouseHalo.color = mouseHaloInvalid;
                    mouseHaloBehindWalls.color = mouseHaloInvalid;
                }
            }
            else if (moveMode == G.MoveMode.free)
            {
                if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !IsPlayerNull())
                {
                    playerScript.Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                    playerScript.TurnToward(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                }
            }
            else if (moveMode == G.MoveMode.attackTarget || moveMode == G.MoveMode.attackArea)
            {
                if (mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Floor")) && !mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Scenery")))
                {
                    switch ((int) mouseScript.SearchForAnyFaction() * (int) playerScript.GetFaction())
                    {
                        case 0:  //  neutral
                        {
                            mouseHalo.color = mouseHaloBase;
                            mouseHaloBehindWalls.color = mouseHaloBase;
                            break;
                        }
                        case 1:  //  same faction
                        {
                            mouseHalo.color = mouseHaloInvalid;
                            mouseHaloBehindWalls.color = mouseHaloInvalid;
                            break;
                        }
                        case -1:  //  opposing faction
                        {
                            mouseHalo.color = mouseHaloBase;
                            mouseHaloBehindWalls.color = mouseHaloBase;
                            break;
                        }
                        default:
                        {
                            mouseHalo.color = mouseHaloInvalid;
                            mouseHaloBehindWalls.color = mouseHaloInvalid;
                            break;
                        }
                    }
                }
            }
            else if (moveMode == G.MoveMode.healTarget || moveMode == G.MoveMode.healArea)
            {
                switch ((int) mouseScript.SearchForAnyFaction() * (int) playerScript.GetFaction())
                {
                    case 0:  //  neutral
                    {
                        mouseHalo.color = mouseHaloBase;
                        mouseHaloBehindWalls.color = mouseHaloBase;
                        break;
                    }
                    case 1:  //  same faction
                    {
                        mouseHalo.color = mouseHaloBase;
                        mouseHaloBehindWalls.color = mouseHaloBase;
                        break;
                    }
                    case -1:  //  opposing faction
                    {
                        mouseHalo.color = mouseHaloInvalid;
                        mouseHaloBehindWalls.color = mouseHaloInvalid;
                        break;
                    }
                    default:
                    {
                        mouseHalo.color = mouseHaloInvalid;
                        mouseHaloBehindWalls.color = mouseHaloInvalid;
                        break;
                    }
                }
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

        void AlignCombat()
        {
            if (playerFaction == G.Faction.ally)
            {
                //Debug.Log("Combat round " + combatRound + ": " + initiativeList[activeCombatant].name + "    (space = take turn    enter = end combat)");
                SetMoveMode(G.MoveMode.click);
                actionScript.ShowCombatActions(playerScript.GetCombatActions());
            }
            else
            {
                //Debug.Log("Combat round " + combatRound + ": " + initiativeList[activeCombatant].name + " is taking their turn");
                SetMoveMode(G.MoveMode.none);
                actionScript.HideCombatActions();
            }
        }
    }
}