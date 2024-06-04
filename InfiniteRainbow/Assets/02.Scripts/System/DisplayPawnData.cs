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
        explainText.text = $"!<능력치>\n" +
                        $"속도: {speed}\n" +
                        $"HP: {hp}\n" +
                        $"공격력: {atk}\n" +
                        $"방어력: {def}\n" +
                        $"회피율: {dodge}%\n" +
                        $"치명타 확률: {criChance}%\n" +
                        $"치명타 피해: {criMulti}%\n\n";
        if (pawn.player)
        {
            explainText.text += $"!<기술 및 특징>\n" +
                        $"특징 및 기본공격: {pawn.attackExplain}\n" +
                        $"필살기: {pawn.finisherExplain}";
        }
        else
        {
            explainText.text += $"!<기술 및 특징>\n" +
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

        if (pawn.player)
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
    }
}
