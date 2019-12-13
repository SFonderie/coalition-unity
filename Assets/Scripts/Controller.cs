using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public float originOffSet = 0.5f;
    public float maxDisR = 10f;
   

    Rigidbody2D m_Rigidbody;
    public float m_Speed = 3f;

    public float turnFloat = 30f;
    private Quaternion turnAnglePos; 
    private Quaternion turnAngleNeg; 


    private float angle;
   
    public int typeOfEnemy =1; //change to faction
 
    public Vector3 PreviousFramePosition = Vector3.zero;
    private float speedForFrames = 0f;
    private float mPF;



    // Use this for initialization
    void Start () {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        //transform.rotation = Quaternion.Euler(0, 0, startFacing);
       
        
        //turnAnglePos = new Quaternion(0, 0, turnFloat/2, 0);
        //turnAngleNeg = new Quaternion(0, 0, -1*turnFloat / 2, 0);
    }
	void Update()
    {
        mPF = Vector3.Distance(PreviousFramePosition, transform.position);
        speedForFrames = mPF / Time.deltaTime;
        PreviousFramePosition = transform.position;
        if (typeOfEnemy == 1 )
        {

            if (mPF == 0)
            {
                rotationQ();//add patrol to enemy if enemy 1
            }
            
        }
        else if (typeOfEnemy == 2)
        {
            rotationQ(); //add something to show light for camera
        }
        else if(typeOfEnemy == 3)
        {
            rotationQ();//no patrol for this script (sniper
        }
               
      
        
    }
	
    public void rotationQ()
    {

        angle = Mathf.PingPong(m_Speed * Time.time, turnFloat) - turnFloat / 2;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private RaycastHit2D CheckRaycast(Vector2 direction)
    {
        //float dirOffSet = originOffSet * (dir.x > 0 ? 1 : -1);
        Vector2 startingPos = new Vector2(transform.position.x + 1.5f, transform.position.y);
        
        return Physics2D.Raycast(startingPos, direction, maxDisR);
    }

    void FixedUpdate()
    {
        RaycastCheckUpdate();

    }

    void RaycastCheckUpdate()
    {

        //Vector2 direction = new Vector2(facingFloat, 0);
        Vector2 direction = transform.right; 
        RaycastHit2D hit = CheckRaycast(direction);
        if (hit.collider)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Hit" + hit.collider.name + " at location " + hit.transform.position); //3.5 -3.3
                Debug.DrawRay(transform.position, hit.point, Color.red, 3f);
            }
        }
        //return true;
    }
}
