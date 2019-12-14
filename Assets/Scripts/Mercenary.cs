using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mercenary : BaseEnemyClass
{
    public float damage;
    public float health;
    public float moveDistance;
    // Use this for initialization
    void Start()
    {

        damage = Dmg;
        health = HP ;
        moveDistance = MoveRate;

    }

}
