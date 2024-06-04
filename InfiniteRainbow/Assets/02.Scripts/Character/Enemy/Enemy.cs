using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Pawn
{
    protected override void Awake()
    {
        base.Awake();
        if (StatusData.floor > 5)
        {
            hp += ((StatusData.floor - 1) / 5) * 100.0f;
            atk += ((StatusData.floor - 1) / 5) * 20.0f;
            def += ((StatusData.floor - 1) / 5) * 5.0f;
            criticalChance += ((StatusData.floor - 1) / 5) * 5.0f;
            criticalMultiplier += ((StatusData.floor - 1) / 5) * 10.0f;
        }

        maxHp = hp;
    }
}
