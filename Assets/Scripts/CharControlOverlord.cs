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
        Sprite targetHaloValid;
        [SerializeField]
        Sprite targetHaloInvalid;
        [SerializeField]
        Color targetHaloAttackColor = Color.red;
        [SerializeField]
        Color targetHaloHealColor = Color.green;
        [SerializeField]
        Color targetHaloInvalidColor = new Color(0.625f, 0.625f, 0.625f);
        [SerializeField]
        GUIStyle displayStyle;
        [SerializeField]
        //GameObject[] players;
        List<GameObject> players;
        [SerializeField]
        Canvas actionCanvas;
        [SerializeField]
        Canvas dialogueCanvas;
        [SerializeField]
        ContactFilter2D raycastFilter;
        #pragma warning restore CS0649
        //GameObject[] enemies;
        List<GameObject> enemies;
        ActionCanvas actionScript;
        DialogueCanvas dialogueScript;
        Tilemap[] tilemaps;
        int playerIndex = 0, combatRound = 1, combatants, activeCombatant = 0, shotsLeft = 0;
        float distanceMoved, moveDistanceMin = 1f;
        GameObject playerObj;
        CharControlSingle playerScript;
        Rigidbody2D playerHandle;
        List<GameObject> initiativeList;
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
        G.CombatAction currentCombatAction;
        float currentMoveRange;

        public G.MoveMode GetMoveMode()
        {
            return moveMode;
        }
        
        public void SetMoveMode(G.MoveMode mode)
        {
            moveMode = mode;

            if (mode == G.MoveMode.none || mode == G.MoveMode.click || mode == G.MoveMode.free)
            {
                mouseHaloBase = movementHaloValidColor;
                mouseHaloInvalid = movementHaloInvalidColor;
            }
            else
            {
                if (mode == G.MoveMode.attack)
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

            distanceMoved = 0;
            currentMoveRange = playerScript.GetMoveActionRange();

            Camera.main.GetComponent<CameraControl>().SetTarget(playerObj);
        }

        public void LoadCombatAction(G.CombatAction action)
        {
            currentCombatAction = action;
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

        //public void StartCombat(ref GameObject[] enemyTeam)
        public void StartCombat(List<GameObject> enemyTeam)
        {
            /*enemies = new GameObject[enemyTeam.Length];
            for (int i = 0; i < enemyTeam.Length; i++)
            {
                enemies[i] = enemyTeam[i];
            }*/
            enemies = enemyTeam;
            AllIsoSnap();
            SortInitiative();
            SetPlayer(initiativeList[activeCombatant]);
            AlignCombat();
            combatState = G.CombatState.combat;

            foreach (GameObject p in players)
            {
                p.transform.Find("HealthCanvas").GetComponent<Canvas>().enabled = true;
            }

            foreach (GameObject e in enemies)
            {
                e.transform.Find("HealthCanvas").GetComponent<Canvas>().enabled = true;
            }
        }

        public void NextTurn()
        {
            nonPlayerTurn = false;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null)
                {
                    enemies.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < initiativeList.Count; i++)
            {
                if (initiativeList[i] == null)
                {
                    initiativeList.RemoveAt(i);
                    i--;
                }
            }

            if (players.Count <= 0 || enemies.Count <= 0)  //  end combat
            {
                ClearCombat();
            }
            else if (activeCombatant < initiativeList.Count - 1)  //  next combatant
            {
                activeCombatant++;
                SetPlayer(initiativeList[activeCombatant]);
                AlignCombat();
            }
            else  //  next round
            {
                activeCombatant = 0;
                combatRound++;
                SetPlayer(initiativeList[activeCombatant]);
                AlignCombat();
            }
        }

        public void ClearCombat()
        {
            //Debug.Log("End combat: returning control to " + players[playerIndex].name);
            SetPlayer(players[playerIndex]);
            SetMoveMode(G.MoveMode.free);
            combatRound = 1;
            combatState = G.CombatState.none;
            actionScript.HideCombatActions();

            //enemies = new GameObject[0];
            enemies = null;
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
            if (Input.GetButtonDown("Cancel"))
            {
                G.Pause();
            }

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
                    /*if (Input.GetButtonDown("DummyCombat"))  //  end combat
                    {
                        ClearCombat();
                    }
                    else if ((Input.GetButtonDown("DummyAction") && playerFaction == G.Faction.ally) || nonPlayerTurn)  //  active combatant takes their turn
                    {
                        NextTurn();
                    }*/

                    if (nonPlayerTurn)  //  active combatant takes their turn
                    {
                        NextTurn();
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
                    else if (playerFaction == G.Faction.ally && Input.GetButtonDown("Fire2"))  //  let player cancel their action
                    {
                        CancelAction();
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
                mouseHalo.sprite = movementHalo;
                mouseHaloBehindWalls.sprite = movementHalo;

                //  set the halo brightness and actually allow movement if the spot is valid
                if (mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Floor")) && !mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Scenery")) && !mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Characters")) && Vector2.Distance(playerScript.GetIsoCoords(), mouseIso) <= currentMoveRange - distanceMoved && distanceMoved < currentMoveRange)
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

                if (currentMoveRange - distanceMoved < moveDistanceMin)  //  movement for this turn is used up
                {
                    CancelAction();
                    actionScript.SetListeningPerm(0, false, true);
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
            else if (moveMode != G.MoveMode.none)
            {
                //Debug.Log("mode switched to either attack or heal");
                mouseHalo.sprite = targetHaloInvalid;
                mouseHaloBehindWalls.sprite = targetHaloInvalid;

                if (shotsLeft == 0)
                {
                    shotsLeft = currentCombatAction.GetUses();
                }

                if (mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Floor")) && !mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Scenery")))
                {
                    //Debug.Log("found floor but not scenery");
                    if (mouseHaloCollider.IsTouchingLayers(LayerMask.GetMask("Characters")))
                    {
                        //Debug.Log("found character");
                        mouseHalo.sprite = targetHaloValid;
                        mouseHaloBehindWalls.sprite = targetHaloValid;
                    }

                    if (Vector2.Distance(playerScript.GetIsoCoords(), mouseIso) > currentCombatAction.GetRange() || !G.LineOfSight(playerScript.GetCloseCoords(), mouseClose, raycastFilter))  //  aiming at something out of range or blocked by an obstacle
                    {
                        //Debug.Log("aiming out of range");
                        mouseHalo.color = mouseHaloInvalid;
                        mouseHaloBehindWalls.color = mouseHaloInvalid;
                    }
                    else  //  aiming at something in range
                    {
                        //Debug.Log("aiming in range");
                        switch ((int) mouseScript.SearchForAnyFaction() * (int) playerScript.GetFaction() * (moveMode == G.MoveMode.attack ? 1 : -1))
                        {
                            case 0:  //  aiming at a neutral character (valid)
                            {
                                //Debug.Log("aiming at neutral character");
                                mouseHalo.color = mouseHaloBase;
                                mouseHaloBehindWalls.color = mouseHaloBase;

                                if (Input.GetButtonDown("Fire1"))
                                {
                                    actionScript.SetListeningPerm(1, false, false);
                                    actionScript.SetListeningPerm(2, false);
                                    G.UseCombatAction(playerScript, mouseScript.SearchForCharOfFaction(G.Faction.neutral).GetComponent<CharControlSingle>(), currentCombatAction);

                                    shotsLeft--;

                                    if (shotsLeft == 0)
                                    {
                                        CancelAction();
                                    }
                                }

                                break;
                            }
                            case 1:  //  targeting an enemy with a heal or a friend with an attack (invalid)
                            {
                                //Debug.Log("aiming at invalid target");
                                mouseHalo.color = mouseHaloInvalid;
                                mouseHaloBehindWalls.color = mouseHaloInvalid;
                                break;
                            }
                            case -1:  //  targeting an enemy with an attack or a friend with a heal (valid)
                            {
                                //Debug.Log("aiming at valid target");
                                mouseHalo.color = mouseHaloBase;
                                mouseHaloBehindWalls.color = mouseHaloBase;

                                if (Input.GetButtonDown("Fire1"))
                                {
                                    actionScript.SetListeningPerm(1, false, false);
                                    actionScript.SetListeningPerm(2, false);
                                    G.UseCombatAction(playerScript, mouseScript.SearchForCharOfFaction((G.Faction) (((int) playerFaction) * -1)).GetComponent<CharControlSingle>(), currentCombatAction);

                                    shotsLeft--;

                                    if (shotsLeft == 0)
                                    {
                                        CancelAction();
                                    }
                                }

                                break;
                            }
                            default:  //  aiming at empty space (invalid)
                            {
                                //Debug.Log("aiming at empty space");
                                mouseHalo.color = mouseHaloInvalid;
                                mouseHaloBehindWalls.color = mouseHaloInvalid;
                                break;
                            }
                        }
                    }
                }
            }
        }

        void CancelAction()
        {
            SetMoveMode(G.MoveMode.none);
            actionScript.SetToolTip("", false);
            actionScript.SetListeningTemp(true);
            shotsLeft = 0;
        }

        void NextPlayer()
        {
            int startIndex = playerIndex;

            //  go through the array once looking for valid players
            do
            {
                ++playerIndex;

                //  look at the next player in the array
                if (playerIndex >= players.Count)
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
            initiativeList = new List<GameObject>();

            activeCombatant = 0;

            //  fill initiative list
            foreach (GameObject p in players)
            {
                if (p != null)
                {
                    initiativeList.Add(p);
                }
            }

            foreach (GameObject e in enemies)
            {
                if (e != null)
                {
                    initiativeList.Add(e);
                }
            }

            activeCombatant = 0;

            initiativeList = initiativeList.OrderByDescending(x => x.GetComponent<CharControlSingle>().RollInitiative()).ToList()/*.ToArray()*/;
        }

        void AlignCombat()
        {
            SetMoveMode(G.MoveMode.none);
            actionScript.HideCombatActions();
                
            if (playerFaction == G.Faction.ally)
            {
                //Debug.Log("Combat round " + combatRound + ": " + initiativeList[activeCombatant].name + "    (space = take turn    enter = end combat)");
                actionScript.ShowCombatActions(playerScript.GetCombatAction1(), playerScript.GetCombatAction2());
            }
            /*else
            {
                Debug.Log("Combat round " + combatRound + ": " + initiativeList[activeCombatant].name + " is taking their turn");
            }*/
        }
    }
}