using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyClass : MonoBehaviour {
    protected float dmg = 10f;
    protected float hP = 10f;
    protected float moveRate = 5f;


    public float Dmg
    {
        get { return dmg; }
    }
    public float HP
    {
        get { return hP; }
    }
    public float MoveRate
    {
        get { return moveRate; }
    }
}


