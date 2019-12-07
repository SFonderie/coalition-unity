using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public class TriggerDialogue : MonoBehaviour
    {
        #pragma warning disable CS0649
        [SerializeField]
        bool isActive = true;
        [SerializeField]
        int doTimes = 1;
        [SerializeField]
        G.DialogueData[] dialogueStages;
        [SerializeField]
        TriggerCombat goToCombat;
        #pragma warning restore CS0649
        CharControlOverlord playerScript;
        DialogueCanvas dialogueScript;
        int timesDone = 0;

        public void Activate()
        {
            TriggerDialogue self = gameObject.GetComponent<TriggerDialogue>();
            timesDone++;
            //  pass data to the dialogue manager
            dialogueScript.LoadDialogueData(ref dialogueStages, ref self);
            //  activate dialogue manager
            dialogueScript.DisplayMessage();
        }

        public void Finish()
        {
            if (goToCombat != null)
            {
                goToCombat.Activate();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            playerScript = GameObject.Find("PlayerController").GetComponent<CharControlOverlord>();
            dialogueScript = GameObject.Find("DialogueCanvas").GetComponent<DialogueCanvas>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (isActive && timesDone < doTimes && IsValidCollider(col))
            {
                Activate();
            }
        }

        bool IsValidCollider(Collider2D col)
        {
            return col.gameObject == playerScript.GetPlayer();
        }
    }
}