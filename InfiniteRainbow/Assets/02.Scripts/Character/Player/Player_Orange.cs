using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ư��: ���� ����
// �ʻ��: �Ʊ� ��ȣ�� �ο� �ʻ��
// ���� �� Ÿ���� (�⺻) / �Ʊ� 3�� ���� ���� �ʻ�� (ü���� ���� ���� �Ʊ�)
public class Player_Orange : Player
{
    private System.Random random = new System.Random();

    //// �Ϲ� ���� (�⺻ ����)
    //public override void Attack()
    //{
    //}

    public override void StartFinisherTargeting()
    {
        Transform targetTr = null;

        // ���� �߽ɿ��� ���� ������ Ÿ����
        // ü���� ���� ���� ��
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
                    // �߽� �ε��� ��ȯ
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
                        // �߽� �ε��� ��ȯ
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

    // �Ʊ� ��ȣ�� �ο�
    public override void Finisher()
    {
        base.Finisher();

        // �� ��ӵ� �Լ����� ���������� �ʻ�� ����
        // �ൿ �κ�
        prevPos = transform.position;
        transform.DOMove(prevPos, 0.5f).OnComplete(() =>
        {
            Debug.Log("��Ȳ �ʻ��");
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
