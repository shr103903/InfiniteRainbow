using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ư��: ���� ȸ�Ƿ�
// �ʻ��: ���� �����̻� �ο�
// ���� �� Ÿ���� (�⺻) / �� 3�� ���� ���� �ʻ�� (�⺻)
public class Player_Skyblue : Player
{
    private System.Random random = new System.Random();

    //// �Ϲ� ���� (�⺻ ����)
    //public override void Attack()
    //{
    //}

    // ���� �����̻� �ο�
    public override void Finisher()
    {
        base.Finisher();

        // �� ��ӵ� �Լ����� ���������� �ʻ�� ����
        // �ൿ �κ�
        prevPos = transform.position;
        Vector3 targetPos = targetPos = new Vector3(middleTarget.position.x, middleTarget.position.y, middleTarget.position.z + 1);
        dist = Vector3.Distance(prevPos, targetPos);
        transform.DOMove(targetPos, dist * 0.1f).OnComplete(() =>
        {
            Debug.Log("�ϴ� �ʻ��");
            anim.SetBool(animFinisher, true);
        });
    }

    public override void FinisherEffect()
    {
        // ġ��Ÿ Ȯ�� �ݿ�
        float damage = atk * (1.0f + atkUpPercent * 0.01f);
        if (random.NextDouble() * 100 < criticalChance + 20)
        {
            damage = damage * (1.0f + criticalMultiplier / 100.0f);
        }

        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            pawn.Freeze();
            pawn.Hit(damage * 0.8f, physicsAtk);
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
