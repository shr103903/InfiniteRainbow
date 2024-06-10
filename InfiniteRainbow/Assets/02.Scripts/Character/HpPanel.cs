using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpPanel : MonoBehaviour
{
    public Image hpImage = null;

    public Image shieldHpImage = null;

    [SerializeField]
    private Image backImage = null;

    [SerializeField]
    private GameObject burnStatusImg = null;

    [SerializeField]
    private GameObject freezeStatusImg = null;

    [HideInInspector]
    public Pawn pawn = null;

    private void Awake()
    {
        hpImage.fillAmount = 1.0f;
    }

    private void Start()
    {
        transform.position = Camera.main.WorldToScreenPoint(pawn.hpPanelPosition.position);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void ChangeBackColor(Color color)
    {
        backImage.color = color;
    }

    public void Burn(bool active)
    {
        if(burnStatusImg.activeSelf != active)
        {
            burnStatusImg.SetActive(active);    
        }
        if (active)
        {
            pawn.burnEfect.Play();
        }
        else
        {
            pawn.burnEfect.Stop();
        }
    }

    public void Freeze(bool active)
    {
        if (freezeStatusImg.activeSelf != active)
        {
            freezeStatusImg.SetActive(active);
        }
        if (active)
        {
            pawn.freezeEfect.Play();
        }
        else
        {
            pawn.freezeEfect.Stop();
        }
    }
}
