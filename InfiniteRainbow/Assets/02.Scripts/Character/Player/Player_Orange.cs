using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 특성: 높은 방어력
// 필살기: 아군 보호막 부여 필살기
// 단일 적 타겟팅 (기본) / 아군 3명 범위 공격 필살기 (체력이 제일 적은 아군)
public class Player_Orange : Player
{
    private System.Random random = new System.Random();

    //// 일반 공격 (기본 공격)
    //public override void Attack()
    //{
    //}

    public override void StartFinisherTargeting()
    {
        Transform targetTr = null;

        // 공격 중심에서 맞을 적부터 타겟팅
        // 체력이 제일 적은 적
        float minHp = 10000;
        Pawn minHpPawn = null;
        for (int i = 0; i < GameManager.instance.battleManager.playerList.Count; i++)
        {
            if (GameManager.instance.battleManager.playerList[i].hp <= minHp)
            {
                minHp = GameManager.instance.battleManager.playerList[i].hp;
                minHpPawn = GameManager.instance.battleManager.playerList[i];
            }
        }
        targetTr = minHpPawn.transform;
        middleTarget = targetTr;

        foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
        {
            pawn.ActiveTargetingMark(false);
        }
        foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
        {
            pawn.ActiveTargetingMark(false);
        }

        int middleIndex = 0;
        foreach (int index in GameManager.instance.battleManager.playerPositionDict.Keys)
        {
            if (GameManager.instance.battleManager.playerPositionDict[index] != null)
            {
                if (GameManager.instance.battleManager.playerPositionDict[index].transform.Equals(targetTr))
                {
                    // 중심 인덱스 반환
                    middleIndex = index;
                    break;
                }
            }
        }
        GameManager.instance.battleManager.targetList.Clear();
        for (int i = middleIndex - 1; i <= middleIndex + 1; i++)
        {
            if (i >= 0 && i < 4)
            {
                if (GameManager.instance.battleManager.playerPositionDict[i] != null)
                {
                    GameManager.instance.battleManager.playerPositionDict[i].ActiveTargetingMark(true);
                    GameManager.instance.battleManager.targetList.Add(GameManager.instance.battleManager.playerPositionDict[i]);
                }
            }
        }
    }

    public override void FinisherTargetting(Transform targetTr)
    {
        if (targetTr.CompareTag("Player"))
        {
            foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
            {
                pawn.ActiveTargetingMark(false);
            }
            foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
            {
                pawn.ActiveTargetingMark(false);
            }

            middleTarget = targetTr;
            int middleIndex = 0;
            foreach (int index in GameManager.instance.battleManager.playerPositionDict.Keys)
            {
                if (GameManager.instance.battleManager.playerPositionDict[index] != null)
                {
                    if (GameManager.instance.battleManager.playerPositionDict[index].transform.Equals(targetTr))
                    {
                        // 중심 인덱스 반환
                        middleIndex = index;
                        break;
                    }
                }
            }
            GameManager.instance.battleManager.targetList.Clear();
            for (int i = middleIndex - 1; i <= middleIndex + 1; i++)
            {
                if (i >= 0 && i < 4)
                {
                    if (GameManager.instance.battleManager.playerPositionDict[i] != null)
                    {
                        GameManager.instance.battleManager.playerPositionDict[i].ActiveTargetingMark(true);
                        GameManager.instance.battleManager.targetList.Add(GameManager.instance.battleManager.playerPositionDict[i]);
                    }
                }
            }
        }
    }

    // 아군 보호막 부여
    public override void Finisher()
    {
        base.Finisher();

        // 각 상속된 함수에서 개인적으로 필살기 돌기
        // 행동 부분
        prevPos = transform.position;
        transform.DOMove(prevPos, 0.5f).OnComplete(() =>
        {
            //Debug.Log("주황 필살기");
            anim.SetBool(animFinisher, true);
        });
    }

    public override void FinisherEffect()
    {
        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            pawn.Shield();
        }
    }

    public override void FinishFinisher()
    {
        anim.SetBool(animFinisher, false);

        transform.DOMove(prevPos, 1.0f).OnComplete(() =>
        {
            if (actionCor != null)
            {
                StopCoroutine(actionCor);
            }
            actionCor = CorAction();
            StartCoroutine(actionCor);
        });
    }
}
