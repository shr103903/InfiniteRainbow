using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField]
    private Pawn pawn = null;

    [SerializeField]
    private bool sword01 = false;

    [SerializeField]
    private ParticleSystem swordParticle1 = null;

    private void Awake()
    {
        pawn = GetComponentInParent<Pawn>();
        pawn.anim = this.GetComponent<Animator>();
    }

    public void Sword01()
    {
        if (sword01)
        {
            if (swordParticle1 != null)
            {
                swordParticle1.Play();
            }
        }
    }

    public void AttackEffect()
    {
        pawn.AttackEffect();
    }

    public void FinishAttack()
    {
        pawn.FinishAttack();
    }

    public void FinisherEffect()
    {
        pawn.FinisherEffect();
    }

    public void FinishFinisher()
    {
        pawn.FinishFinisher();
    }

    public void Dead()
    {
        Destroy(pawn.gameObject);
    }
}
