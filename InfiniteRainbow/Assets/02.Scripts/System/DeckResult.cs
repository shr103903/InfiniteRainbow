using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckResult : MonoBehaviour
{
    public List<DeckPlayerListButton> deckPlayerList = new List<DeckPlayerListButton>();

    [SerializeField]
    private List<DeckResultListButton> deckButtonList = new List<DeckResultListButton>();

    [SerializeField]
    private List<DeckPlayerListButton> enemySpriteList = new List<DeckPlayerListButton>();

    private System.Random random = new System.Random();


    public bool AddPlayer(DeckPlayerListButton player)
    {
        if (deckPlayerList.Count >= 4)
        {
            return false;
        }
        deckPlayerList.Add(player);

        UpdateImage();
        return true;
    }

    public void RemovePlayer(DeckResultListButton player)
    {
        foreach(DeckResultListButton playerButton in deckButtonList)
        {
            if (player.Equals(playerButton))
            {
                deckPlayerList[deckButtonList.IndexOf(playerButton)].Deselect();
                deckPlayerList.RemoveAt(deckButtonList.IndexOf(playerButton));

                break;
            }
        }

        UpdateImage();
    }

    public void RemovePlayer(DeckPlayerListButton player)
    {
        deckPlayerList.Remove(player);
        UpdateImage();
    }

    public void AddEnemy(int difficulty)
    {
        GameManager.instance.battleManager.enemyList.Clear();
        EnemyDeckData enemyDeckData = GameManager.instance.GetEnemyDeck(difficulty);
        for (int i = 0; i < enemyDeckData.enemyArr.Length; i++)
        {
            GameObject mob = GameObject.Instantiate(enemyDeckData.enemyArr[i]);
            Pawn pawn = mob.GetComponent<Pawn>();
            foreach (Pawn enemy in GameManager.instance.battleManager.enemyList)
            {
                if (pawn.pawnSprite.Equals(enemy.pawnSprite))
                {
                    pawn.pawnNumber++;
                }
            }
            GameManager.instance.battleManager.enemyList.Add(pawn);
            enemySpriteList[i].SetEnemy(pawn);
        }
    }

    private void UpdateImage()
    {
        for (int i = 3; i >= 0; i--)
        {
            if(deckPlayerList.Count > i)
            {
                deckButtonList[i].UpdateDeck(deckPlayerList[i].pawn, true);
            }
            else
            {
                deckButtonList[i].UpdateDeck(null, false);
            }
        }
    }
}
