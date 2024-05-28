using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDeck", menuName = "EnemyDeck")]
public class EnemyDeckData : ScriptableObject
{
    public GameObject[] enemyArr = new GameObject[0];
}
