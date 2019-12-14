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
        G.CombatAction[] combatActions;

        public void ShowCombatActions(G.CombatAction[] activeCombatActions)
        {
            combatActions = new G.CombatAction[activeCombatActions.Length];
            Array.Copy(activeCombatActions, combatActions, activeCombatActions.Length);

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

            combatActions = new G.CombatAction[0];
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
                playerScript.LoadCombatAction(currentAction);
            }
        }
    }
}