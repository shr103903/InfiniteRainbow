using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDemon : Boss
{
    private Transform targetTr = null;

    private float originAtk = 0;

    private bool passPhase1 = false;

    private bool passPhase2 = false;

    private bool firstAttack = false;

    private bool buffAttack = false;

    private int hitCount = 0;

    private int attackNum = 0;

    private System.Random random = new System.Random();

    // Ư��1, 2
    protected readonly int animAttack1 = Animator.StringToHash("Attack1");
    protected readonly int animAttack2 = Animator.StringToHash("Attack2");
    protected readonly int animAttack3 = Animator.StringToHash("Attack3");

    protected override void Awake()
    {
        base.Awake();

        hitCount = 0;
        originAtk = atk;
        passPhase1 = false;
        passPhase2 = false;
        firstAttack = false;
        buffAttack = false;
    }

    public override bool Hit(float damage, bool physicalAtk, bool statusDamage = false)
    {
        base.Hit(damage, physicalAtk, statusDamage);
        hitCount += 1;
        atk = originAtk * (1.0f + 0.05f * hitCount);
        return true;
    }

    public override void StartTargeting()
    {
        // ù �������� ù �Ͽ� �ٴڿ� �� ������ ��Ƽ� 3�ϰ� ȭ�� �������� ���������� ��
        if (!firstAttack)
        {
            firstAttack = true;
            attackNum = 3;
        }
        // Ư��1 ü���� 2/3, 1/3������ �� �� ���� �� �Կ��� �߻��ϴ� ���� ����(�Ʊ� ��ü �ǰ�)
        else if (hp <= maxHp * 0.66f && !passPhase1)
        {
            passPhase1 = true;
            attackNum = 1;
        }
        else if (hp <= maxHp * 0.33f && passPhase1 && !passPhase2)
        {
            passPhase2 = true;
            attackNum = 1;
        }
        // Ư��2 ü���� 20% ���ϰ� �Ǹ� ȸ������ 20% �����ϴ� ���� ����
        else if (hp <= maxHp * 0.2f && !buffAttack)
        {
            buffAttack = true;
            attackNum = 2;
        }
        // �Ϲ�
        else
        {
            attackNum = 0;
        }

        targetTr = null;

        if (attackNum == 1)
        {
            targetTr = GameManager.instance.battleManager.positionParent.playerMiddlePos;
        }
        else if (attackNum == 2 || attackNum == 3)
        {
            targetTr = transform.transform;
        }
        else
        {
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
        }

        GameManager.instance.battleManager.targetList.Clear();
        foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
        {
            if (attackNum == 2 && pawn.Equals((Pawn)this))
            {
                pawn.ActiveTargetingMark(true);
                GameManager.instance.battleManager.targetList.Add(pawn);
            }
            else
            {
                pawn.ActiveTargetingMark(false);
            }
        }
        foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
        {
            if (attackNum == 3 || attackNum == 1)
            {
                pawn.ActiveTargetingMark(true);
                GameManager.instance.battleManager.targetList.Add(pawn);
            }
            else if (attackNum == 0 && pawn.transform.Equals(targetTr))
            {
                pawn.ActiveTargetingMark(true);
                GameManager.instance.battleManager.targetList.Add(pawn);
            }
            else
            {
                pawn.ActiveTargetingMark(false);
            }
        }

        EnemyAttack();
    }

    // �Ϲ� ���� (�� 3�� ���� ����)
    public override void Attack()
    {
        base.Attack();

        // �ൿ �κ�
        prevPos = transform.position;
        Vector3 targetPos = Vector3.zero;
        if (attackNum == 0)
        {
            targetPos = new Vector3(targetTr.position.x, targetTr.position.y, targetTr.position.z - 1);
            dist = 5.0f;
        }
        else if (attackNum == 1)
        {
            targetPos = new Vector3(targetTr.position.x, targetTr.position.y, targetTr.position.z - 1);
            dist = Vector3.Distance(prevPos, targetPos);
        }
        else if (attackNum == 2 || attackNum == 3)
        {
            targetPos = transform.position;
            dist = 5.0f;
        }
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
            else if (attackNum == 3)
            {
                anim.SetBool(animAttack3, true);
            }
        });
    }

    public override void AttackEffect()
    {
        if (attackNum == 0 || attackNum == 1)
        {
            Debug.Log(gameObject.name + " ����");
            // ġ��Ÿ Ȯ�� �ݿ�
            float damage = atk * (1.0f + atkUpPercent * 0.01f);
            if (random.NextDouble() * 100 < criticalChance)
            {
                damage = damage * (1.0f + criticalMultiplier / 100.0f);
            }

            int rand = random.Next(100);
            foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
            {
                if (pawn.Hit(damage, physicsAtk))
                {
                    if (rand < 50)
                    {
                        pawn.Burn();
                    }
                }
            }
        }
        else if (attackNum == 2)
        {
            this.dodge *= 1.2f;
        }
        else if (attackNum == 3)
        {
            Debug.Log(gameObject.name + " ����");
            // ġ��Ÿ Ȯ�� �ݿ�
            float damage = atk * (1.0f + atkUpPercent * 0.01f);
            if (random.NextDouble() * 100 < criticalChance)
            {
                damage = damage * (1.0f + criticalMultiplier / 100.0f);
            }

            foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
            {
                if (pawn.Hit(damage * 0.25f, physicsAtk))
                {
                    pawn.Burn();
                }
            }
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
        else if (attackNum == 3)
        {
            anim.SetBool(animAttack3, false);
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
