using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Pawn
{
    protected override void Awake()
    {
        base.Awake();
        GetStatus();
        maxHp = hp;
    }

    private void GetStatus()
    {
        for (int i = 0; i < 3; i++)
        {
            hp += (50.0f + 20.0f * i) * StatusData.hpUpgrade[i];
        }
        for (int i = 0; i < 3; i++)
        {
            atk += (10.0f + 2.0f * i) * StatusData.atkUpgrade[i];
        }
        for (int i = 0; i < 3; i++)
        {
            def += (3.0f + 1.0f * i) * StatusData.defUpgrade[i];
        }
        for (int i = 0; i < 3; i++)
        {
            dodge += (3.0f + 1.0f * i) * StatusData.dodgeUpgrade[i];
        }
        for (int i = 0; i < 3; i++)
        {
            criticalChance += (4.0f + 1.0f * i) * StatusData.criChanceUpgrade[i];
        }
        for (int i = 0; i < 3; i++)
        {
            criticalMultiplier += (8.0f + 2.0f * i) * StatusData.criMultiUpgrade[i];
        }
        for (int i = 0; i < 3; i++)
        {
            speed += (2 + 1 * i) * StatusData.speedUpgrade[i];
        }
    }
}
