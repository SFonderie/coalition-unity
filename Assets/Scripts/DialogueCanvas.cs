using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coalition
{
    public class DialogueCanvas : MonoBehaviour
    {
        Text dialogueText;
        SpriteRenderer dialoguePortrait, dialogueBackground;
        Button dialogueButton;
        bool isEnabled = true;
        DialogueData[] dialogueStages;
        int currentStage = 0;

        public void LoadDialogueData(ref DialogueData[] data)
        {
            dialogueStages = new DialogueData[data.Length];
            System.Array.Copy(data, dialogueStages, data.Length);
            currentStage = 0;
        }

        public void ClearDialogueData()
        {
            dialogueStages = new DialogueData[0];
            currentStage = 0;
        }
        
        public void DisplayMessage(string message, Sprite portrait)
        {
            //  set the given text and character image to appear
            dialoguePortrait.sprite = portrait;
            dialogueText.text = message;
            
            //  make the components visible
            dialogueBackground.enabled = true;
            dialoguePortrait.enabled = true;
            dialogueText.enabled = true;
            dialogueButton.gameObject.SetActive(true);  //  different because buttons are weird

            //  internally keep track of enabled status
            isEnabled = true;
        }

        public void DisplayMessage()
        {
            if (dialogueStages.Length > 0)
            {
                dialoguePortrait.sprite = dialogueStages[currentStage].GetPortrait();
                dialogueText.text = dialogueStages[currentStage].GetText();

                dialogueBackground.enabled = true;
                dialoguePortrait.enabled = true;
                dialogueText.enabled = true;
                dialogueButton.gameObject.SetActive(true);

                isEnabled = true;
            }
        }

        public void HideMessage()
        {
            dialoguePortrait.sprite = (Sprite) null;
            dialogueText.text = "";
            
            dialogueBackground.enabled = false;
            dialoguePortrait.enabled = false;
            dialogueText.enabled = false;
            dialogueButton.gameObject.SetActive(false);

            isEnabled = false;
        }

        public bool IsEnabled()
        {
            return isEnabled;
        }

        // Start is called before the first frame update
        void Start()
        {
            dialogueBackground = transform.Find("Background").GetComponent<SpriteRenderer>();
            dialoguePortrait = transform.Find("Portrait").GetComponent<SpriteRenderer>();
            dialogueText = transform.Find("Text").GetComponent<Text>();
            dialogueButton = transform.Find("Button").GetComponent<Button>();
            dialogueButton.onClick.AddListener(Next);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void Next()
        {
            if (isEnabled)
            {
                if (currentStage < dialogueStages.Length - 1)
                {
                    ++currentStage;
                    DisplayMessage();
                }
                else
                {
                    ClearDialogueData();
                    HideMessage();
                }
            }
        }
    }
}