using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackKnight : Boss
{
    private Transform targetTr = null;

    private int hitTurn = 0;

    private float originDef = 0;

    private float originAtk = 0;

    private bool passPhase1 = false;

    private bool passPhase2 = false;

    private int attackNum = 0;

    private System.Random random = new System.Random();

    // 특공1, 2
    protected readonly int animAttack1 = Animator.StringToHash("Attack1");
    protected readonly int animAttack2 = Animator.StringToHash("Attack2");

    protected override void Awake()
    {
        base.Awake();

        hitTurn = 0;
        originAtk = atk;
        originDef = def;
        passPhase1 = false;
        passPhase2 = false;
    }

    public override bool Hit(float damage, bool physicalAtk, bool statusDamage = false)
    {
        hitTurn = (hitTurn + 1) % 4;
        atk = originAtk * (1.0f + 0.1f * hitTurn);
        def = originDef * (1.0f - 0.1f * hitTurn);
        base.Hit(damage, physicalAtk, statusDamage);
        return true;
    }

    public override void StartTargeting()
    {
        // 특공1
        if (hp <= maxHp * 0.66f && !passPhase1)
        {
            passPhase1 = true;
            attackNum = 1;
        }
        // 특공2
        else if (hp <= maxHp * 0.33f && passPhase1 && !passPhase2)
        {
            passPhase2 = true;
            attackNum = 2;
        }
        else
        {
            attackNum = 0;
        }

        targetTr = null;
        int rand = random.Next(3);

        if (rand == 0)
        {
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
        }
        else
        {
            float maxDef = -10;
            Pawn maxDefPawn = null;
            for (int i = 0; i < GameManager.instance.battleManager.playerList.Count; i++)
            {
                if (GameManager.instance.battleManager.playerList[i].def * (1.0f + GameManager.instance.battleManager.playerList[i].defUpPercent * 0.01f) >= maxDef)
                {
                    maxDef = GameManager.instance.battleManager.playerList[i].def * (1.0f + GameManager.instance.battleManager.playerList[i].defUpPercent * 0.01f);
                    maxDefPawn = GameManager.instance.battleManager.playerList[i];
                }
            }
            targetTr = maxDefPawn.transform;
        }

        foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
        {
            pawn.ActiveTargetingMark(false);
        }
        foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
        {
            pawn.ActiveTargetingMark(false);
        }
        GameManager.instance.battleManager.targetList.Clear();
        foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
        {
            if (attackNum == 0 || attackNum == 1)
            {
                if (pawn.transform.Equals(targetTr))
                {
                    pawn.ActiveTargetingMark(true);
                    GameManager.instance.battleManager.targetList.Add(pawn);
                }
                else
                {
                    pawn.ActiveTargetingMark(false);
                }
            }
            else
            {
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

        EnemyAttack();
    }

    // 일반 공격 (적 3명 범위 공격)
    public override void Attack()
    {
        base.Attack();

        // 행동 부분
        prevPos = transform.position;
        Vector3 targetPos = new Vector3(targetTr.position.x, targetTr.position.y, targetTr.position.z - 1);
        dist = Vector3.Distance(prevPos, targetPos);
        transform.DOMove(targetPos, dist * 0.1f).OnComplete(() =>
        {
            if (attackNum == 0)
            {
                anim.SetBool(animAttack, true);
            }
            else if (attackNum == 1)
            {
                anim.SetBool(animAttack1, true);
            }
            else if (attackNum == 2)
            {
                anim.SetBool(animAttack2, true);
            }
        });
    }

    public override void AttackEffect()
    {
        Debug.Log(gameObject.name + " 공격");
        // 치명타 확률 반영
        float damage = atk * (1.0f + atkUpPercent * 0.01f);
        if (random.NextDouble() * 100 < criticalChance)
        {
            damage = damage * (1.0f + criticalMultiplier / 100.0f);
        }

        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            pawn.Hit(damage, physicsAtk);
        }
    }

    public override void FinishAttack()
    {
        if (attackNum == 0)
        {
            anim.SetBool(animAttack, false);
        }
        else if (attackNum == 1)
        {
            anim.SetBool(animAttack1, false);
        }
        else if (attackNum == 2)
        {
            anim.SetBool(animAttack2, false);
        }

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
