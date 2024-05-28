using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Pawn
{
    protected override void Awake()
    {
        base.Awake();
        hp += (StatusData.floor / 5) * 100.0f;
        atk += (StatusData.floor / 5) * 20.0f;
        def += (StatusData.floor / 5) * 5.0f;
        criticalChance += (StatusData.floor / 5) * 5.0f;
        criticalMultiplier += (StatusData.floor / 5) * 10.0f;

        maxHp = hp;
    }
}
