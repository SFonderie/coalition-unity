using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coalition
{
    public class ActionCanvas : MonoBehaviour
    {
        #pragma warning disable CS0649
        CharControlOverlord playerScript;
        Transform buttonGroup;
        List<Button> buttons = new List<Button>();
        #pragma warning restore CS0649
        G.CombatAction[] combatActions = new G.CombatAction[0];

        public void ShowCombatActions(G.CombatAction[] activeCombatActions)
        {
            combatActions = activeCombatActions;

            for (int i = 0; i < combatActions.Length && i < buttons.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);
                SetButtonText(i, combatActions[i].GetName());
            }
        }

        public void HideCombatActions()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].gameObject.SetActive(false);
                SetButtonText(i, "Button");
            }

            Array.Clear(combatActions, 0, combatActions.Length);
        }

        string GetButtonText(Button button)
        {
            return button.transform.Find("Text").GetComponent<Text>().text;
        }

        void Awake()
        {
            playerScript = GameObject.Find("PlayerController").GetComponent<CharControlOverlord>();
            buttonGroup = transform.Find("CombatActionButtons");
            
            for (int i = 0; i < buttonGroup.childCount; i++)
            {
                buttons.Add(buttonGroup.GetChild(i).GetComponent<Button>());
            }

            foreach (Button button in buttons)
            {
                button.onClick.AddListener(() => ProcessCombatAction(GetButtonText(button)));
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            HideCombatActions();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void SetButtonText(int i, string buttonText)
        {
            if (i >= 0 && i < buttons.Count)
            {
                buttons[i].transform.Find("Text").GetComponent<Text>().text = buttonText;
            }
        }

        void ProcessCombatAction(string actionText)
        {
            G.CombatAction currentAction = null;

            foreach (G.CombatAction action in combatActions)
            {
                if (action.GetName() == actionText)
                {
                    currentAction = action;
                    break;
                }
            }

            if (currentAction != null)
            {
                switch (currentAction.GetActionType())
                {
                    case (G.CombatActionType.empty):
                    {
                        playerScript.SetMoveMode(G.MoveMode.free);
                        break;
                    }
                    case (G.CombatActionType.move):
                    {
                        playerScript.SetMoveMode(G.MoveMode.click);
                        break;
                    }
                    case (G.CombatActionType.attackTarget):
                    {
                        playerScript.SetMoveMode(G.MoveMode.attackTarget);
                        break;
                    }
                    case (G.CombatActionType.attackArea):
                    {
                        playerScript.SetMoveMode(G.MoveMode.attackArea);
                        break;
                    }
                    case (G.CombatActionType.healTarget):
                    {
                        playerScript.SetMoveMode(G.MoveMode.healTarget);
                        break;
                    }
                    case (G.CombatActionType.healArea):
                    {
                        playerScript.SetMoveMode(G.MoveMode.healArea);
                        break;
                    }
                    default:
                    {
                        playerScript.SetMoveMode(G.MoveMode.free);
                        break;
                    }
                }
            }
        }
    }
}