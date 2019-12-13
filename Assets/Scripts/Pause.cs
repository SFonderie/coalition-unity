using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    //[SerializeField] private GameObject pausePanel;
    bool onOff = false; //false being off
    [SerializeField]
    GameObject[] menus;
    void Start()
    {
        //pausePanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            onOff = !onOff;

            if (onOff)
            {
                PauseGame();
            }
            if (!onOff)
            {
                ContinueGame();
            }

            /*if (!pausePanel.activeInHierarchy)
            {
                PauseGame();
            }
            if (pausePanel.activeInHierarchy)
            {
                ContinueGame();
            }
            */

        }
        
    }
    private void PauseGame()
    {
        Time.timeScale = 0;
        //pausePanel.SetActive(true);
        Debug.Log("pause");
        for(int i = 0; i < menus.Length; i++)
        {
            menus[i].gameObject.SetActive(false);

        }
        //Disable scripts that still work while timescale is set to 0

    }
    private void ContinueGame()
    {
        Time.timeScale = 1;
        //pausePanel.SetActive(false);
        Debug.Log("Unpause");
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].gameObject.SetActive(true);

        }
        //enable the scripts again
    }
}
