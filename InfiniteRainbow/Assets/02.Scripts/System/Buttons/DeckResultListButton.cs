using UnityEngine;
using UnityEngine.UI;

public class DeckResultListButton : MonoBehaviour
{
    [SerializeField]
    private Image portraitImage = null;

    private Pawn playerPawn = null;

    private bool havePlayer = false;

    private Button btn = null;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => DeselectPlayer());
    }

    public void UpdateDeck(Pawn player, bool active)
    {
        if (player != null)
        {
            this.playerPawn = player;
            portraitImage.sprite = player.pawnSprite;
        }
        else
        {
            this.playerPawn = null;
            portraitImage.sprite = null;
        }
        havePlayer = active;
    }

    public void DeselectPlayer()
    {
        if(havePlayer)
        {
            GameManager.instance.battleManager.deck.RemovePlayer(this);
        }
    }
}
