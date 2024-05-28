using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTurnUIBlock : MonoBehaviour
{
    private Image turnImage = null;

    private void Awake()
    {
        turnImage = GetComponent<Image>();
    }

    public void SetImage(Sprite sprite)
    {
        turnImage.sprite = sprite;
    }
}
