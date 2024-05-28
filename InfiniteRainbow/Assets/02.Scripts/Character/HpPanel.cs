using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpPanel : MonoBehaviour
{
    public Image hpImage = null;

    public Image shieldHpImage = null;

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

    public void Burn(bool active)
    {
        if(burnStatusImg.activeSelf != active)
        {
            burnStatusImg.SetActive(active);
        }
    }

    public void Freeze(bool active)
    {
        if (freezeStatusImg.activeSelf != active)
        {
            freezeStatusImg.SetActive(active);
        }
    }
}
