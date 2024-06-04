using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 특성: 높은 공격력
// 필살기: 화상 상태이상 부여
// 단일 적 타겟팅 (기본: 체력이 제일 적은 적(실드 포함X)) / 적 3명 범위 공격 필살기 (기본)
public class Player_Red : Player
{
    private System.Random random = new System.Random();

    //// 일반 공격 (기본 공격)
    //public override void Attack()
    //{
    //}

    // 화상 상태이상 부여
    public override void Finisher()
    {
        base.Finisher();

        // 각 상속된 함수에서 개인적으로 필살기 돌기
        // 행동 부분
        prevPos = transform.position;
        Vector3 targetPos = targetPos = new Vector3(middleTarget.position.x, middleTarget.position.y, middleTarget.transform.position.z + 1);
        dist = Vector3.Distance(prevPos, targetPos);
        transform.DOMove(targetPos, dist * 0.1f).OnComplete(() =>
        {
            //Debug.Log("빨강 필살기");
            anim.SetBool(animFinisher, true);
        });
    }

    public override void FinisherEffect()
    {
        // 치명타 확률 반영
        float damage = atk * (1.0f + atkUpPercent * 0.01f);
        if (random.NextDouble() * 100 < criticalChance + 20)
        {
            damage = damage * (1.0f + criticalMultiplier / 100.0f);
        }

        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            if (pawn.Hit(damage * 0.8f, physicsAtk))
            {
                pawn.Burn();
            }
        }
    }

    public override void FinishFinisher()
    {
        anim.SetBool(animFinisher, false);

        transform.DOMove(prevPos, dist * 0.2f).OnComplete(() =>
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
