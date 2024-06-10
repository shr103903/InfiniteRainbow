using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlyingDemonAnim : MonoBehaviour
{
    [SerializeField]
    private FlyingDemon pawn = null;

    [SerializeField]
    private List<ParticleSystem> flameParticleList = new List<ParticleSystem>();

    [SerializeField]
    private ParticleSystem fireBall = null;

    [SerializeField]
    private Transform fireballStartPos = null;

    private void Awake()
    {
        pawn = GetComponentInParent<FlyingDemon>();
        pawn.anim = this.GetComponent<Animator>();
    }

    public void FlameStart()
    {
        foreach (ParticleSystem particle in flameParticleList)
        {
            particle.Play();
        }
    }

    public void FlameFinish()
    {
        foreach (ParticleSystem particle in flameParticleList)
        {
            particle.Stop();
        }
    }

    public void ThrowBall()
    {
        fireBall.transform.parent.position = fireballStartPos.position;
        fireBall.Play();
        fireBall.transform.DOMove(pawn.targetTr.position + new Vector3(0, 0.5f, 0), 1.0f).OnComplete(()=>
        {
            fireBall.Stop();
        });
    }
}
