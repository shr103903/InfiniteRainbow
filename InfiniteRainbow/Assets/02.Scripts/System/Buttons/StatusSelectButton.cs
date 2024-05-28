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
                text.text = $"ü��\n{50 + 20 * GameManager.instance.battleManager.difficulty} ����";
                break;
            case 1:
                text.text = $"���ݷ�\n{10 + 2 * GameManager.instance.battleManager.difficulty} ����";
                break;
            case 2:
                text.text = $"����\n{3 + 1 * GameManager.instance.battleManager.difficulty} ����";
                break;
            case 3:
                text.text = $"ȸ����\n{3 + 1 * GameManager.instance.battleManager.difficulty}% ����";
                break;
            case 4:
                text.text = $"ġ��Ÿ Ȯ��\n{4 + 1 * GameManager.instance.battleManager.difficulty}% ����";
                break;
            case 5:
                text.text = $"ġ��Ÿ ����\n{8 + 2 * GameManager.instance.battleManager.difficulty}% ����";
                break;
            case 6:
                text.text = $"�ӵ�\n{2 + 1 * GameManager.instance.battleManager.difficulty} ����";
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
    }
}
