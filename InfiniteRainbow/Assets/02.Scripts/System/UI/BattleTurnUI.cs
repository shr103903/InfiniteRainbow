using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTurnUI : MonoBehaviour
{
    [SerializeField]
    private BattleTurnUIBlock blockPrefab = null;

    private Dictionary<int, Pawn> turnDict = new Dictionary<int, Pawn>();

    [SerializeField]
    private List<BattleTurnUIBlock> blockList = new List<BattleTurnUIBlock>();

    private int curIndex = 0;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.H))
    //    {
    //        FInishTurn(GameManager.instance.battleManager.currentTurnList[0]);
    //    }
    //}

    public void setTurn()
    {
        Init();
    }

    // 폰 사망
    public void RemoveTurn(Pawn pawn)
    {
        int index = 0;
        for(int i = 0; i < turnDict.Count; i++)
        {
            if (turnDict[i].Equals(pawn))
            {
                index = i;
                break;
            }
        }

        blockList[index].gameObject.SetActive(false);
        blockList.RemoveAt(index);

        if (index <= turnDict.Count - 2)
        {
            for (int i = index; i < turnDict.Count - 1; i++)
            {
                turnDict[i] = turnDict[i + 1];
            }
        }
        turnDict.Remove(turnDict.Count - 1);
    }

    // 속도 변화
    public void ChangeTurn(ref List<Pawn> prevTurnList, ref List<Pawn> changeTurnList)
    {
        for (int i = 0; i < turnDict.Count; i++)
        {
            if (!prevTurnList[i].Equals(changeTurnList[i]))
            {
                turnDict[i] = changeTurnList[i];
                blockList[i].SetImage(turnDict[i].pawnSprite);
                Debug.Log(prevTurnList[i].gameObject.name + " 위치로 " + changeTurnList[i].gameObject.name + "의 순서 변경");
            }
        }
    }

    public void CheckTurn()
    {
        List<Pawn> prevTurnList = new List<Pawn>();
        if (GameManager.instance.battleManager.currentTurnList.Count > 0)
        {
            for (int i = 0; i < GameManager.instance.battleManager.currentTurnList.Count; i++)
            {
                prevTurnList.Add(GameManager.instance.battleManager.currentTurnList[i]);
            }
        }
        for (int i = 0; i < GameManager.instance.battleManager.playerList.Count + GameManager.instance.battleManager.enemyList.Count
            - GameManager.instance.battleManager.currentTurnList.Count; i++)
        {
            prevTurnList.Add(GameManager.instance.battleManager.nextTurnList[i]);
        }

        for (int i = 0; i < turnDict.Count; i++)
        {
            if (!turnDict[i].Equals(prevTurnList[i]))
            {
                turnDict[i] = prevTurnList[i];
                blockList[i].SetImage(turnDict[i].pawnSprite);
                Debug.Log(prevTurnList[i].gameObject.name + " 위치로 " + prevTurnList[i].gameObject.name + "의 순서 변경");
            }
        }
    }

    // 처음 블럭 사라지고 마지막에 나옴
    public void FInishTurn(Pawn pawn)
    {
        blockList[0].gameObject.transform.SetAsLastSibling();
        BattleTurnUIBlock temp = blockList[0];
        for (int i = 0; i < turnDict.Count - 1; i++)
        {
            turnDict[i] = turnDict[i + 1];
            blockList[i] = blockList[i + 1];
        }

        //GameObject block = GameObject.Instantiate(blockPrefab.gameObject, transform);
        //blockList.Add(block.GetComponent<BattleTurnUIBlock>());
        turnDict[turnDict.Count - 1] = pawn;
        blockList[turnDict.Count - 1] = temp;

        CheckTurn();
    }

    private void Init()
    {
        for(int i = 0; i < 9; i++)
        {
            AddBlockPrefab();
            if (i < GameManager.instance.battleManager.currentTurnList.Count)
            {
                turnDict.Add(i, GameManager.instance.battleManager.currentTurnList[i]);
                blockList[i].SetImage(turnDict[i].pawnSprite);
                blockList[i].gameObject.SetActive(true);
            }
        }
    }

    private void AddBlockPrefab()
    {
        GameObject block = GameObject.Instantiate(blockPrefab.gameObject, transform);
        blockList.Add(block.GetComponent<BattleTurnUIBlock>());
        

        block.SetActive(false);
    }
}
