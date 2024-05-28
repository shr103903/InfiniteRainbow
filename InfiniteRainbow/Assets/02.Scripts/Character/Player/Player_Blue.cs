using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ư��: ���� ȿ�� ����
// �ʻ��: �Ʊ� ġ��Ÿ ���� ���� ���� �ʻ��
// �Ʊ� 1�� Ÿ���� (���� ����(�ӵ� 2�� ����, ���ݷ����� 2��, �������� 2��, ġ��Ÿ ���� ���� 1�� �� �� 1��) (ó������ ���� �Ʊ� ����)) / �Ʊ� ��ü Ÿ���� (�Ʊ� ��ü ġ��Ÿ ���� ���� 3��)
public class Player_Blue : Player
{
    private System.Random random = new System.Random();

    public override void StartTargeting()
    {
        Transform targetTr = null;
        int rand = random.Next(GameManager.instance.battleManager.playerList.Count);

        // ���� �Ʊ�
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

    // �Ϲ� ���� (�Ʊ� 1�� ���� ����)
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

    // �Ʊ� ġ��Ÿ ���� ���� ���� �ʻ��
    public override void Finisher()
    {
        base.Finisher();

        // �� ��ӵ� �Լ����� ���������� �ʻ�� ����
        // �ൿ �κ�
        prevPos = transform.position;
        transform.DOMove(prevPos, 0.5f).OnComplete(() =>
        {
            Debug.Log("�Ķ� �ʻ��");
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
