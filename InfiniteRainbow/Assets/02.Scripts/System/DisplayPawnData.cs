using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayPawnData : MonoBehaviour
{
    [SerializeField]
    private Image portraitImage = null;

    [SerializeField]
    private TMP_Text explainText = null;

    private float hp = 0;
    private float atk = 0;
    private float def = 0;
    private float dodge = 0;
    private float criChance = 0;
    private float criMulti = 0;
    private int speed = 0;

    public void DisplayData(Pawn pawn)
    {
        portraitImage.sprite = pawn.pawnSprite;
        CalcStatus(pawn);
        explainText.text = $"!<�ɷ�ġ>\n" +
                        $"�ӵ�: {speed}\n" +
                        $"HP: {hp}\n" +
                        $"���ݷ�: {atk}\n" +
                        $"����: {def}\n" +
                        $"ȸ����: {dodge}%\n" +
                        $"ġ��Ÿ Ȯ��: {criChance}%\n" +
                        $"ġ��Ÿ ����: {criMulti}%\n\n";
        if (pawn.player)
        {
            explainText.text += $"!<��� �� Ư¡>\n" +
                        $"Ư¡ �� �⺻����: {pawn.attackExplain}\n" +
                        $"�ʻ��: {pawn.finisherExplain}";
        }
        else
        {
            explainText.text += $"!<��� �� Ư¡>\n" +
                        $"{pawn.attackExplain}";
        }
    }

    private void CalcStatus(Pawn pawn)
    {
        hp = pawn.hp;
        atk = pawn.atk;
        def = pawn.def;
        dodge = pawn.dodge;
        criChance = pawn.criticalChance;
        criMulti = pawn.criticalMultiplier;
        speed = pawn.speed;

        if (!pawn.player)
        {
            hp += (StatusData.floor / 5) * 100.0f;
            atk += (StatusData.floor / 5) * 20.0f;
            def += (StatusData.floor / 5) * 5.0f;
            criChance += (StatusData.floor / 5) * 5.0f;
            criMulti += (StatusData.floor / 5) * 10.0f;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                hp += (50.0f + 20.0f * i) * StatusData.hpUpgrade[i];
            }
            for (int i = 0; i < 3; i++)
            {
                atk += (10.0f + 2.0f * i) * StatusData.atkUpgrade[i];
            }
            for (int i = 0; i < 3; i++)
            {
                def += (3.0f + 1.0f * i) * StatusData.defUpgrade[i];
            }
            for (int i = 0; i < 3; i++)
            {
                dodge += (3.0f + 1.0f * i) * StatusData.dodgeUpgrade[i];
            }
            for (int i = 0; i < 3; i++)
            {
                criChance += (4.0f + 1.0f * i) * StatusData.criChanceUpgrade[i];
            }
            for (int i = 0; i < 3; i++)
            {
                criMulti += (8.0f + 2.0f * i) * StatusData.criMultiUpgrade[i];
            }
            for (int i = 0; i < 3; i++)
            {
                speed += (2 + 1 * i) * StatusData.speedUpgrade[i];
            }

        }

        print(StatusData.criMultiUpgrade[0]);
        print(StatusData.criMultiUpgrade[1]);
        print(StatusData.criMultiUpgrade[2]);
    }
}
