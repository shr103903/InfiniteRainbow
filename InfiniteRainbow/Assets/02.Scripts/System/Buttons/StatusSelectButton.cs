using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusSelectButton : MonoBehaviour
{
    public int num = 0;

    [SerializeField]
    private TMP_Text text = null;

    public void SetText()
    {
        switch (num)
        {
            case 0:
                text.text = $"체력\n{50 + 20 * GameManager.instance.battleManager.difficulty} 증가";
                break;
            case 1:
                text.text = $"공격력\n{10 + 2 * GameManager.instance.battleManager.difficulty} 증가";
                break;
            case 2:
                text.text = $"방어력\n{3 + 1 * GameManager.instance.battleManager.difficulty} 증가";
                break;
            case 3:
                text.text = $"회피율\n{3 + 1 * GameManager.instance.battleManager.difficulty}% 증가";
                break;
            case 4:
                text.text = $"치명타 확률\n{4 + 1 * GameManager.instance.battleManager.difficulty}% 증가";
                break;
            case 5:
                text.text = $"치명타 피해\n{8 + 2 * GameManager.instance.battleManager.difficulty}% 증가";
                break;
            case 6:
                text.text = $"속도\n{2 + 1 * GameManager.instance.battleManager.difficulty} 증가";
                break;
            default:
                text.text = $"";
                break;
        }
    }

    public void Select()
    {
        GameManager.instance.UpgradeStatus(num);
        GameManager.instance.battleManager.MoveNextFloor();
        SoundManager.instance.Play("UI/Button", Define.Sound.UI);
    }
}
