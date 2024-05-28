using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckPlayerListButton : MonoBehaviour
{
    public bool isPlayer = false;

    public int playerNum = 0;

    [HideInInspector]
    public Pawn pawn = null;

    [HideInInspector]
    public Sprite playerSprite = null;

    private Image spriteImage = null;

    private GameObject selectedMark = null;

    private Button btn = null;

    private bool selected = false;

    private void Awake()
    {
        if (isPlayer)
        {
            selected = false;
            selectedMark = transform.GetChild(0).gameObject;
            playerSprite = GetComponent<Image>().sprite;

            btn = GetComponent<Button>();
            btn.onClick.AddListener(() => Select());

            pawn = GameManager.instance.playerList[playerNum].GetComponent<Pawn>();
        }
        else
        {
            spriteImage = GetComponent<Image>();
            spriteImage.enabled = false;
            btn = GetComponent<Button>();
            btn.onClick.AddListener(() => ShowCharacterInfo());
        }
    }

    public void Select()
    {
        // �̹� ���õ� ������ ��� ������ ����
        if (selected)
        {
            Deselect();
            GameManager.instance.battleManager.deck.RemovePlayer(this);
        }
        // ���õ��� ���� ���¶�� ���� �� �ڸ��� �ִ��� Ȯ���� �� ���� ��ġ
        else
        {
            if (GameManager.instance.battleManager.deck.AddPlayer(this))
            {
                selected = true;
                selectedMark.SetActive(true);
            }

            ShowCharacterInfo();
        }
        ShowCharacterInfo();
    }

    public void SetEnemy(Pawn pawn)
    {
        spriteImage.enabled = true;
        playerSprite = pawn.pawnSprite;
        spriteImage.sprite = playerSprite;
        this.pawn = pawn;
    }

    private void ShowCharacterInfo()
    {
        GameManager.instance.battleManager.DisplayPawnData(pawn);
    }

    public void Deselect()
    {
        selected = false;
        selectedMark.SetActive(false);
    }
}
