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

        public void ShowCombatActions(G.CombatAction[] activeCombatActions)
        {
            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(true);
            }
        }

        public void HideCombatActions()
        {
            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(false);
            }
        }

        void Awake()
        {
            playerScript = GameObject.Find("PlayerController").GetComponent<CharControlOverlord>();
            buttonGroup = this.gameObject.transform.Find("CombatActionButtons");
            
            for (int i = 0; i < buttonGroup.childCount; i++)
            {
                buttons.Add(buttonGroup.GetChild(i).GetComponent<Button>());
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
    }
}