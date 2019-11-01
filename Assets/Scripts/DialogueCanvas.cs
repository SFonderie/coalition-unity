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
        CharControlOverlord playerScript;
        int playerMoveBackup = -1;
        CameraControl cameraScript;
        GameObject cameraTargetBackup;

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
        
        public void DisplayMessage(string message, Sprite portrait, bool stop, GameObject cameraTarget = (GameObject) null, bool backupState = false)
        {
            //  set the given text and character image to appear
            dialogueText.text = message;
            dialoguePortrait.sprite = portrait;

            if (stop)
            {
                if (backupState)
                {
                    playerMoveBackup = (int) playerScript.GetMoveMode();
                }
                playerScript.SetMoveMode(Globals.MoveMode.none);
            }

            if (cameraTarget != null)
                {
                    if (backupState)
                    {
                        cameraTargetBackup = cameraScript.GetTarget();
                    }
                    cameraScript.SetTarget(cameraTarget);
                }
            
            //  make the components visible
            dialogueText.enabled = true;
            dialoguePortrait.enabled = true;
            dialogueBackground.enabled = true;
            dialogueButton.gameObject.SetActive(true);  //  different because buttons are weird

            //  internally keep track of enabled status
            isEnabled = true;
        }

        public void DisplayMessage(DialogueData dialogue)
        {
            DisplayMessage(dialogue.GetText(), dialogue.GetPortrait(), dialogue.GetStop(), dialogue.GetTarget(), currentStage == 0);
        }

        public void DisplayMessage()
        {
            if (dialogueStages.Length > 0)
            {
                DisplayMessage(dialogueStages[currentStage]);
            }
        }

        public void HideMessage()
        {
            dialogueText.text = "";
            dialoguePortrait.sprite = (Sprite) null;

            if (playerMoveBackup != -1)
            {
                playerScript.SetMoveMode(playerMoveBackup);
                playerMoveBackup = -1;
            }

            if (cameraTargetBackup != null)
            {
                cameraScript.SetTarget(cameraTargetBackup);
                cameraTargetBackup = (GameObject) null;
            }
            
            dialogueText.enabled = false;
            dialoguePortrait.enabled = false;
            dialogueBackground.enabled = false;
            dialogueButton.gameObject.SetActive(false);

            isEnabled = false;
        }

        public bool IsEnabled()
        {
            return isEnabled;
        }

        void Awake()
        {
            playerScript = GameObject.Find("PlayerController").GetComponent<CharControlOverlord>();

            dialogueBackground = transform.Find("Background").GetComponent<SpriteRenderer>();
            dialoguePortrait = transform.Find("Portrait").GetComponent<SpriteRenderer>();
            dialogueText = transform.Find("Text").GetComponent<Text>();
            dialogueButton = transform.Find("Button").GetComponent<Button>();
            dialogueButton.onClick.AddListener(Next);
        }

        // Start is called before the first frame update
        void Start()
        {
            cameraScript = Camera.main.GetComponent<CameraControl>();
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