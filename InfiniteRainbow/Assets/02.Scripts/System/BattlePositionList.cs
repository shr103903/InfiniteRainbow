using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePositionList : MonoBehaviour
{
    public Transform enemyMiddlePos = null;

    public Transform playerMiddlePos = null;

    private void Awake()
    {
        GameManager.instance.battleManager.positionParent = this;
    }
}
