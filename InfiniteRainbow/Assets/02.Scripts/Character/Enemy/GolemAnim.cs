using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAnim : MonoBehaviour
{
    [SerializeField]
    private Pawn pawn = null;

    [SerializeField]
    private ParticleSystem ringEffect = null;

    [SerializeField]
    private List<ParticleSystem> debuffParticleList = new List<ParticleSystem>();

    private void Awake()
    {
        pawn = GetComponentInParent<Pawn>();
        pawn.anim = this.GetComponent<Animator>();
    }

    public void Roar()
    {
        ringEffect.Play();
        int index = 0;
        foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        {
            debuffParticleList[index].transform.parent.position = pawn.transform.position;
            debuffParticleList[index].Play();
            index++;
        }
    }
}
