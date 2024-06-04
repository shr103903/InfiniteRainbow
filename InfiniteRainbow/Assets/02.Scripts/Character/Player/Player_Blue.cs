using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 특성: 버프 효과 보유
// 필살기: 아군 치명타 피해 증가 버프 필살기
// 아군 1명 타겟팅 (랜덤 버프(속도 2턴 증가, 공격력증가 2턴, 방어력증가 2턴, 치명타 피해 증가 1턴 등 중 1개) (처음에는 랜덤 아군 지정)) / 아군 전체 타겟팅 (아군 전체 치명타 피해 증가 3턴)
public class Player_Blue : Player
{
    private System.Random random = new System.Random();

    public override void StartTargeting()
    {
        Transform targetTr = null;
        int rand = random.Next(GameManager.instance.battleManager.playerList.Count);

        // 랜덤 아군
        targetTr = GameManager.instance.battleManager.playerList[rand].transform;

        foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
        {
            pawn.ActiveTargetingMark(false);
        }
        foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
        {
            if (pawn.transform.Equals(targetTr))
            {
                pawn.ActiveTargetingMark(true);
                GameManager.instance.battleManager.targetList.Clear();
                GameManager.instance.battleManager.targetList.Add(pawn);
            }
            else
            {
                pawn.ActiveTargetingMark(false);
            }
        }
    }

    public override void Targetting(Transform targetTr)
    {
        if (targetTr.CompareTag("Player"))
        {
            foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
            {
                pawn.ActiveTargetingMark(false);
            }
            foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
            {
                if (pawn.transform.Equals(targetTr))
                {
                    pawn.ActiveTargetingMark(true);
                    GameManager.instance.battleManager.targetList.Clear();
                    GameManager.instance.battleManager.targetList.Add(pawn);
                }
                else
                {
                    pawn.ActiveTargetingMark(false);
                }
            }
        }
    }

    public override void StartFinisherTargeting()
    {
        foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
        {
            pawn.ActiveTargetingMark(false);
        }
        GameManager.instance.battleManager.targetList.Clear();
        foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
        {
            pawn.ActiveTargetingMark(true);
            GameManager.instance.battleManager.targetList.Add(pawn);
        }
    }

    public override void FinisherTargetting(Transform targetTr)
    {
    }

    // 일반 공격 (아군 1명 랜덤 버프)
    public override void Attack()
    {
        base.Attack();

        // 행동 부분
        prevPos = transform.position;
        transform.DOMove(prevPos, 0.5f).OnComplete(() =>
        {
            anim.SetBool(animAttack, true);
        });
    }

    public override void AttackEffect()
    {
        //Debug.Log(gameObject.name + " 일반공격");

        int rand = random.Next(4);
        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            if (rand == 0)
            {
                pawn.GetBuff(Pawn.Buff.Speed, 2, 20);
            }
            else if (rand == 1)
            {
                pawn.GetBuff(Pawn.Buff.ATKUp, 2, 20);
            }
            else if (rand == 2)
            {
                pawn.GetBuff(Pawn.Buff.DEFUp, 2, 20);
            }
            else if (rand == 3)
            {
                pawn.GetBuff(Pawn.Buff.CriticalChance, 1, 20);
            }
        }
    }

    public override void FinishAttack()
    {
        anim.SetBool(animAttack, false);

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

    // 아군 치명타 피해 증가 버프 필살기
    public override void Finisher()
    {
        base.Finisher();

        // 각 상속된 함수에서 개인적으로 필살기 돌기
        // 행동 부분
        prevPos = transform.position;
        transform.DOMove(prevPos, 0.5f).OnComplete(() =>
        {
            //Debug.Log("파랑 필살기");
            anim.SetBool(animFinisher, true);
        });
    }

    public override void FinisherEffect()
    {
        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            pawn.GetBuff(Pawn.Buff.CriticalMultiplier, 3, 30);
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
