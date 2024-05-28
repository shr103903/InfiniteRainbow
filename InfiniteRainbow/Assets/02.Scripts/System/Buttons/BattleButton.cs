using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleButton : MonoBehaviour
{
    private Animator anim = null;

    private readonly int animActive = Animator.StringToHash("Active");

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Active(bool active)
    {
        anim.SetBool(animActive, active);
    }
}
