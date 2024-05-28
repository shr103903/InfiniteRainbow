using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 특성: 높은 마법 공격력 및 높은 마법 내성
// 필살기: 적 속도 감소 및 방어력 감소 디버프 부여 필살기)
// 적 3명 범위 공격 (기본) / 적 3명 범위 타겟팅 (타겟팅 완료 후 속도 20 감소 1턴, 방어력 감소 30% 3턴) (체력이 제일 많은 적)
public class Player_Purple : Player
{
    private Transform targetTr = null;

    private System.Random random = new System.Random();

    public override void StartTargeting()
    {
        int rand = random.Next(3);

        // 공격 중심에서 맞을 적부터 타겟팅
        // 체력이 제일 적은 적
        if (rand == 0)
        {
            float minHp = 10000;
            Pawn minHpPawn = null;
            for (int i = 0; i < GameManager.instance.battleManager.enemyList.Count; i++)
            {
                if (GameManager.instance.battleManager.enemyList[i].hp <= minHp)
                {
                    minHp = GameManager.instance.battleManager.enemyList[i].hp;
                    minHpPawn = GameManager.instance.battleManager.enemyList[i];
                }
            }
            targetTr = minHpPawn.transform;
        }
        // 방어력이 제일 높은 적
        else
        {
            float maxDef = -10;
            Pawn maxDefPawn = null;
            for (int i = 0; i < GameManager.instance.battleManager.enemyList.Count; i++)
            {
                if (GameManager.instance.battleManager.enemyList[i].def * (1.0f + GameManager.instance.battleManager.enemyList[i].defUpPercent * 0.01f) >= maxDef)
                {
                    maxDef = GameManager.instance.battleManager.enemyList[i].def * (1.0f + GameManager.instance.battleManager.enemyList[i].defUpPercent * 0.01f);
                    maxDefPawn = GameManager.instance.battleManager.enemyList[i];
                }
            }
            targetTr = maxDefPawn.transform;
        }


        foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
        {
            pawn.ActiveTargetingMark(false);
        }
        foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
        {
            pawn.ActiveTargetingMark(false);
        }

        int middleIndex = 0;
        foreach (int index in GameManager.instance.battleManager.enemyPositionDict.Keys)
        {
            if (GameManager.instance.battleManager.enemyPositionDict[index] != null)
            {
                if (GameManager.instance.battleManager.enemyPositionDict[index].transform.Equals(targetTr))
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
            if (i >= 0 && i < 5)
            {
                if (GameManager.instance.battleManager.enemyPositionDict[i] != null)
                {
                    GameManager.instance.battleManager.enemyPositionDict[i].ActiveTargetingMark(true);
                    GameManager.instance.battleManager.targetList.Add(GameManager.instance.battleManager.enemyPositionDict[i]);
                }
            }
        }
    }

    public override void Targetting(Transform targetTr)
    {
        if (!targetTr.CompareTag("Enemy"))
        {
            return;
        }

        this.targetTr = targetTr;
        foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
        {
            pawn.ActiveTargetingMark(false);
        }
        foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
        {
            pawn.ActiveTargetingMark(false);
        }

        int middleIndex = 0;
        foreach (int index in GameManager.instance.battleManager.enemyPositionDict.Keys)
        {
            if (GameManager.instance.battleManager.enemyPositionDict[index] != null)
            {
                if (GameManager.instance.battleManager.enemyPositionDict[index].transform.Equals(targetTr))
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
            if (i >= 0 && i < 5)
            {
                if (GameManager.instance.battleManager.enemyPositionDict[i] != null)
                {
                    GameManager.instance.battleManager.enemyPositionDict[i].ActiveTargetingMark(true);
                    GameManager.instance.battleManager.targetList.Add(GameManager.instance.battleManager.enemyPositionDict[i]);
                }
            }
        }
    }

    public override void StartFinisherTargeting()
    {
        Transform targetTr = null;

        // 공격 중심에서 맞을 적부터 타겟팅
        // 체력이 제일 많은 적
        float maxHp = 0;
        Pawn maxHpPawn = null;
        for (int i = 0; i < GameManager.instance.battleManager.enemyList.Count; i++)
        {
            if (GameManager.instance.battleManager.enemyList[i].hp >= maxHp)
            {
                maxHp = GameManager.instance.battleManager.enemyList[i].hp;
                maxHpPawn = GameManager.instance.battleManager.enemyList[i];
            }
        }
        targetTr = maxHpPawn.transform;
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
        foreach (int index in GameManager.instance.battleManager.enemyPositionDict.Keys)
        {
            if (GameManager.instance.battleManager.enemyPositionDict[index] != null)
            {
                if (GameManager.instance.battleManager.enemyPositionDict[index].transform.Equals(targetTr))
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
            if (i >= 0 && i < 5)
            {
                if (GameManager.instance.battleManager.enemyPositionDict[i] != null)
                {
                    GameManager.instance.battleManager.enemyPositionDict[i].ActiveTargetingMark(true);
                    GameManager.instance.battleManager.targetList.Add(GameManager.instance.battleManager.enemyPositionDict[i]);
                }
            }
        }
    }

    public override void FinisherTargetting(Transform targetTr)
    {
        if (targetTr.CompareTag("Enemy"))
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
            foreach (int index in GameManager.instance.battleManager.enemyPositionDict.Keys)
            {
                if (GameManager.instance.battleManager.enemyPositionDict[index] != null)
                {
                    if (GameManager.instance.battleManager.enemyPositionDict[index].transform.Equals(targetTr))
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
                if (i >= 0 && i < 5)
                {
                    if (GameManager.instance.battleManager.enemyPositionDict[i] != null)
                    {
                        GameManager.instance.battleManager.enemyPositionDict[i].ActiveTargetingMark(true);
                        GameManager.instance.battleManager.targetList.Add(GameManager.instance.battleManager.enemyPositionDict[i]);
                    }
                }
            }
        }
    }

    // 일반 공격 (적 3명 범위 공격)
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
        Debug.Log(gameObject.name + " 일반공격");
        // 치명타 확률 반영
        float damage = atk * (1.0f + atkUpPercent * 0.01f);
        if (random.NextDouble() * 100 < criticalChance)
        {
            damage = damage * (1.0f + criticalMultiplier / 100.0f);
        }

        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            pawn.Hit(damage * 0.5f, physicsAtk);
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

    // 적 속도 감소 및 방어력 감소 디버프 부여
    public override void Finisher()
    {
        base.Finisher();

        // 각 상속된 함수에서 개인적으로 필살기 돌기
        // 행동 부분
        prevPos = transform.position;
        transform.DOMove(prevPos, 0.5f).OnComplete(() =>
        {
            Debug.Log("보라 필살기");
            anim.SetBool(animFinisher, true);
        });
    }

    public override void FinisherEffect()
    {
        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            pawn.GetDebuff(Pawn.Debuff.Speed, 1, 20);
            pawn.GetDebuff(Pawn.Debuff.DEFUp, 3, 30);
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
