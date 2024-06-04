using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 특성: 치유 효과 보유
// 필살기: 아군 전체 치유 필살기
// 아군 1명 타겟팅 (체력 제일 적은 아군) / 아군 전체 타겟팅 (아군 전체)
public class Player_Green : Player
{
    [SerializeField]
    private ParticleSystem[] healEffectPool = new ParticleSystem[4];

    public override void StartTargeting()
    {
        Transform targetTr = null;

        // 체력이 제일 적은 아군
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

    // 일반 공격 (아군 1명 치유 효과)
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

        int index = 0;
        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            pawn.Heal(atk * (1.0f + atkUpPercent * 0.01f) * 5.0f);
            healEffectPool[index].transform.parent.position = pawn.transform.position;
            healEffectPool[index].Play();
            index++;
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

    // 아군 전체 치유 필살기
    public override void Finisher()
    {
        base.Finisher();

        // 각 상속된 함수에서 개인적으로 필살기 돌기
        // 행동 부분
        prevPos = transform.position;
        transform.DOMove(prevPos, 0.5f).OnComplete(() =>
        {
            //Debug.Log("초록 필살기");
            anim.SetBool(animFinisher, true);
        });
    }

    public override void FinisherEffect()
    {
        int index = 0;
        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            pawn.Heal(atk * (1.0f + atkUpPercent * 0.01f) * 3.0f);
            healEffectPool[index].transform.parent.position = pawn.transform.position;
            healEffectPool[index].Play();
            index++;
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
