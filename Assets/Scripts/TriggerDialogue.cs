using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    [System.Serializable]
    public class DialogueData
    {
        #pragma warning disable CS0649
        [SerializeField]
        Sprite characterPortrait;
        [SerializeField]
        string message;
        #pragma warning restore CS0649

        public string GetText()
        {
            return message;
        }

        public Sprite GetPortrait()
        {
            return characterPortrait;
        }
    }

    public class TriggerDialogue : MonoBehaviour
    {
        #pragma warning disable CS0649
        [SerializeField]
        DialogueData[] dialogueStages;
        #pragma warning restore CS0649

        CharControlOverlord playerScript;
        DialogueCanvas dialogueScript;

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
            if (col.gameObject == playerScript.GetPlayer())  //  only trigger if activated by the main character
            {
                //  pass data to the dialogue manager
                dialogueScript.LoadDialogueData(ref dialogueStages);
                //  activate dialogue manager
                dialogueScript.DisplayMessage();
            }
        }
    }
}