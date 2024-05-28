using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ư��: ġ�� ȿ�� ����
// �ʻ��: �Ʊ� ��ü ġ�� �ʻ��
// �Ʊ� 1�� Ÿ���� (ü�� ���� ���� �Ʊ�) / �Ʊ� ��ü Ÿ���� (�Ʊ� ��ü)
public class Player_Green : Player
{
    public override void StartTargeting()
    {
        Transform targetTr = null;

        // ü���� ���� ���� �Ʊ�
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

    // �Ϲ� ���� (�Ʊ� 1�� ġ�� ȿ��)
    public override void Attack()
    {
        base.Attack();

        // �ൿ �κ�
        prevPos = transform.position;
        transform.DOMove(prevPos, 0.5f).OnComplete(() =>
        {
            anim.SetBool(animAttack, true);
        });
    }

    public override void AttackEffect()
    {
        Debug.Log(gameObject.name + " �Ϲݰ���");

        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            pawn.Heal(atk * (1.0f + atkUpPercent * 0.01f) * 5.0f);
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

    // �Ʊ� ��ü ġ�� �ʻ��
    public override void Finisher()
    {
        base.Finisher();

        // �� ��ӵ� �Լ����� ���������� �ʻ�� ����
        // �ൿ �κ�
        prevPos = transform.position;
        transform.DOMove(prevPos, 0.5f).OnComplete(() =>
        {
            Debug.Log("�ʷ� �ʻ��");
            anim.SetBool(animFinisher, true);
        });
    }

    public override void FinisherEffect()
    {
        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            pawn.Heal(atk * (1.0f + atkUpPercent * 0.01f) * 3.0f);
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
