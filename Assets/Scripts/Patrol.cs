using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public float speedWP;
    public float waitTime;
    public float sWaitTime;
    public Transform[] moveSpots;
    private int i = 0;
    private int r = 0;
    private Controller m_Controller; 
    

    // Start is called before the first frame update
    void Start()
    {        
        m_Controller = GetComponent<Controller>();
        waitTime = sWaitTime = ((m_Controller.turnFloat * 2 )) / m_Controller.m_Speed;
    }

    internal ContactFilter2D GetRaycastFilter()
    {
        throw new NotImplementedException();
    }


    // Update is called once per frame
    void Update()
    {
        
        if (r == 0){
            //change this to make it turned base, use maybe for first if statment
            
            transform.position = Vector2.MoveTowards(transform.position, moveSpots[i].position, speedWP * Time.deltaTime);
            if (Vector2.Distance(transform.position, moveSpots[i].position) < 0.2f)
            {
                
                //m_Controller.m_Speed = 0 ;
                if (waitTime <= 0) //do && is turn
                {
                    if (i < moveSpots.Length - 1)
                    {
                        Debug.Log("Next");
                        
                        i++;

                    }
                    else
                    {
                        Debug.Log("Reset");
                        i = 0;
                    }

                                       

                    waitTime =  (m_Controller.turnFloat * 2) / m_Controller.m_Speed; 


                }
                else
                {
                    waitTime -= Time.deltaTime;
                }

            }
        }
    }
}
