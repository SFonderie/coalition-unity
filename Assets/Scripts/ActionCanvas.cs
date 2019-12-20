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
        Text tooltip;
        Transform buttonGroup;
        SpriteRenderer tooltipBackground, buttonsBackground;
        List<Button> buttons = new List<Button>();
        #pragma warning restore CS0649
        G.CombatAction combatAction1, combatAction2;
        bool listenTemp = true;
        bool[] listenPerm = {true, true, true};

        public void ShowCombatActions(G.CombatAction action1, G.CombatAction action2)
        {
            buttonsBackground.enabled = true;

            combatAction1 = action1;
            combatAction2 = action2;

            SetButtonText(1, combatAction1.GetName());
            SetButtonText(2, combatAction2.GetName());

            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(true);
            }

            SetListeningTemp(true, false);
            SetListeningPerm(0, true, false);
            SetListeningPerm(1, true, false);
            SetListeningPerm(2, true);
        }

        public void HideCombatActions()
        {
            buttonsBackground.enabled = false;

            combatAction1 = null;
            combatAction2 = null;

            SetButtonText(1);
            SetButtonText(2);

            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(false);
            }
        }

        public void SetListeningTemp(bool listen = true, bool refresh = true)
        {
            listenTemp = listen;

            if (refresh)
            {
                RefreshButtons();
            }
        }

        public void SetListeningPerm(int i, bool listen = true, bool refresh = true)
        {
            if (i >= 0 && i < listenPerm.Length)
            {
                listenPerm[i] = listen;
            }

            if (refresh)
            {
                RefreshButtons();
            }
        }

        public void RefreshButtons()
        {
            buttons[0].interactable = listenTemp && listenPerm[0];
            buttons[1].interactable = listenTemp && listenPerm[1];
            buttons[2].interactable = listenTemp && listenPerm[2];
            buttons[3].interactable = listenTemp;
        }

        public void SetToolTip(string message, bool visible = true)
        {
            tooltip.text = message;
            tooltip.enabled = visible;
            tooltipBackground.enabled = visible;
        }

        void Awake()
        {
            playerScript = GameObject.Find("PlayerController").GetComponent<CharControlOverlord>();
            tooltip = transform.Find("Tooltip").GetComponent<Text>();
            tooltipBackground = tooltip.gameObject.transform.Find("Background").GetComponent<SpriteRenderer>();
            SetToolTip("", false);
            buttonGroup = transform.Find("CombatActionButtons");
            buttonsBackground = buttonGroup.Find("Background").GetComponent<SpriteRenderer>();

            buttons.Add(buttonGroup.Find("Button 1").GetComponent<Button>());
            buttons.Add(buttonGroup.Find("Button 2").GetComponent<Button>());
            buttons.Add(buttonGroup.Find("Button 3").GetComponent<Button>());
            buttons.Add(buttonGroup.Find("Button 4").GetComponent<Button>());

            buttons[0].onClick.AddListener(() => ProcessMoveAction());
            buttons[1].onClick.AddListener(() => ProcessCombatAction(combatAction1));
            buttons[2].onClick.AddListener(() => ProcessCombatAction(combatAction2));
            buttons[3].onClick.AddListener(() => ProcessEndTurn());
        }

        // Start is called before the first frame update
        void Start()
        {
            SetButtonText(0, "Move");
            SetButtonText(3, "End Turn");

            HideCombatActions();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void SetButtonText(int i, string buttonText = "CombatAction")
        {
            if (i >= 0 && i < buttons.Count)
            {
                buttons[i].transform.Find("Text").GetComponent<Text>().text = buttonText;
            }
        }

        string GetButtonText(Button button)
        {
            return button.transform.Find("Text").GetComponent<Text>().text;
        }

        void ProcessMoveAction()
        {
            playerScript.SetMoveMode(G.MoveMode.click);
            SetToolTip("RightClick to cancel action");
            SetListeningTemp(false);
        }

        void ProcessCombatAction(G.CombatAction action)
        {
            playerScript.LoadCombatAction(action);

            if (action.GetActionType() == G.CombatActionType.attackTarget)
            {
                playerScript.SetMoveMode(G.MoveMode.attack);
                SetToolTip("RightClick to cancel action");
                SetListeningTemp(false);
            }
            else if (action.GetActionType() == G.CombatActionType.healTarget)
            {
                playerScript.SetMoveMode(G.MoveMode.heal);
                SetToolTip("RightClick to cancel action");
                SetListeningTemp(false);
            }
        }

        void ProcessEndTurn()
        {
            playerScript.NextTurn();
        }
    }
}