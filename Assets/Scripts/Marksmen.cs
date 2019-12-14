using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marksmen : BaseEnemyClass
{
    public float damage;
    public float health;
    public float moveDistance;
    // Use this for initialization
    void Start()
    {

        damage = Dmg + 2;
        health = HP - 5;
        moveDistance = MoveRate - MoveRate;

    }
}
