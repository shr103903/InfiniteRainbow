using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Pawn : MonoBehaviour
{
    public enum StatusEffect
    {
        redFinisher,
        skyBlueFinisher,
    }

    public enum Buff
    {
        Speed,
        ATKUp,
        DEFUp,
        CriticalChance,
        CriticalMultiplier
    }

    public enum Debuff
    {
        Speed,
        DEFUp
    }

    public Sprite pawnSprite = null;

    public int pawnNumber = 0;

    public bool player = false;

    public bool physicsAtk = true;

    public bool basicPhysicsAttack = true;

    public float hp = 100;

    public int speed = 0;

    public float atk = 100;

    public float def = 0;

    public float dodge = 0;

    public float criticalChance = 0;

    public float criticalMultiplier = 0;

    public string attackExplain = "";

    public string finisherExplain = "";

    [SerializeField]
    protected float physicalAtkResistance = 0;

    [SerializeField]
    protected float magicalAtkResistance = 0;

    [HideInInspector]
    public GameObject selectMark = null;

    [HideInInspector]
    public GameObject curTurnMark = null;

    [HideInInspector]
    public Transform hpPanelPosition = null;

    [HideInInspector]
    public HpPanel hpPanel = null;

    [HideInInspector]
    public Animator anim = null;

    [HideInInspector]
    public Vector3 prevPos = Vector3.zero;

    [HideInInspector]
    public float defUpPercent = 0;

    [HideInInspector]
    public float dist = 0;

    protected Transform middleTarget = null;

    protected bool haveShield = false;

    protected float shieldHp = 0;

    private Dictionary<int, BuffInfo> buffDict = new Dictionary<int, BuffInfo>();

    private Dictionary<int, DebuffInfo> debuffDict = new Dictionary<int, DebuffInfo>();

    protected float atkUpPercent = 0;

    protected float maxHp = 100;

    private int burnCount = 0;

    private int freezeCount = 0;

    private int buffIndex = 0;

    protected IEnumerator actionCor = null;

    protected IEnumerator enemyCor = null;

    private WaitForSeconds actionTime = new WaitForSeconds(1.0f);

    private System.Random random = new System.Random();

    protected readonly int animAttack = Animator.StringToHash("Attack");
    protected readonly int animFinisher = Animator.StringToHash("Finisher");
    protected readonly int animHit = Animator.StringToHash("Hit");
    protected readonly int animDead = Animator.StringToHash("Dead");


    protected virtual void Awake()
    {
        maxHp = hp;
        shieldHp = 0;
        haveShield = false;
        hpPanelPosition = transform.GetChild(0).transform;
        selectMark = transform.GetChild(1).gameObject;
        selectMark.SetActive(false);
        curTurnMark = transform.GetChild(2).gameObject;
        curTurnMark.SetActive(false);
    }

    public virtual void StartAction()
    {
        // �����̻� Ȯ��
        CheckStatus();

        if(freezeCount > 0)
        {
            GameManager.instance.battleManager.waitingAction = false;
            curTurnMark.SetActive(true);

            if (actionCor != null)
            {
                StopCoroutine(actionCor);
            }
            actionCor = CorAction();
            StartCoroutine(actionCor);
            return;
        }

        Debug.Log(gameObject.name + "�ൿ ����");
        if (player)
        {
            GameManager.instance.battleManager.waitingAction = true;
            GameManager.instance.battleManager.ActiveAttackButton(true);
        }
        curTurnMark.SetActive(true);
        StartTargeting();
    }

    public virtual bool Hit(float damage, bool physicalAtk, bool statusDamage = false)
    {
        float def = this.def * (1.0f + defUpPercent * 0.01f);
        if(def >= 70)
        {
            def = 70;
        }
        else if(def < 0)
        {
            def = 0;
        }

        // ȸ��
        if (random.NextDouble() * 100 < dodge + def * 0.05)
        {
            Debug.Log("ȸ�� ����");
            return false;
        }

        // ������ ���� �ݿ�
        float damageResult = damage * (100 - def) * 0.01f;
        // ���� ���� ���� ���
        if (physicalAtkResistance > 0 && physicalAtk)
        {
            if(physicalAtkResistance >= 70)
            {
                damageResult = damageResult * (100 - 70) * 0.01f;
            }
            else
            {
                damageResult = damageResult * (100 - physicalAtkResistance) * 0.01f;
            }
        }
        if (magicalAtkResistance > 0 && !physicalAtk)
        {
            if (magicalAtkResistance >= 70)
            {
                damageResult = damageResult * (100 - 70) * 0.01f;
            }
            else
            {
                damageResult = damageResult * (100 - magicalAtkResistance) * 0.01f;
            }
        }
        if(damageResult <= 0)
        {
            return false;
        }
        if (player)
        {
            GameManager.instance.battleManager.PlayerHit();
        }

        if (player)
        {
            Debug.Log(damageResult + "������ �ǰ�");
        }
        else
        {
            Debug.Log(damageResult + "������ Ÿ��");
        }

        if (haveShield)
        {
            if (shieldHp > damageResult)
            {
                shieldHp -= damageResult;
                hpPanel.shieldHpImage.fillAmount = shieldHp / maxHp;
                anim.SetTrigger(animHit);
                return true;
            }
            else
            {
                haveShield = false;
                hp = hp + shieldHp - damageResult;
                shieldHp = 0;
                hpPanel.shieldHpImage.fillAmount = 0.0f;
            }
        }
        else
        {
            hp -= damageResult;
        }

        if (hp > 0)
        {
            anim.SetTrigger(animHit);
        }
        else
        {
            hp = 0;
            anim.SetBool(animDead, true);
            if (statusDamage)
            {
                GameManager.instance.battleManager.playingAction = false;
                CheckBuff();
                GameManager.instance.battleManager.PawnAction(this);
                GameManager.instance.battleManager.FinishAction();
            }
            Destroy(hpPanel.gameObject);

            GameManager.instance.battleManager.Killed(this);
            return true;
        }

        hpPanel.hpImage.fillAmount = hp / maxHp;
        return true;
    }

    public void Burn()
    {
        burnCount = 3;
        hpPanel.Burn(true);
    }

    public void Freeze()
    {
        freezeCount = 3;
        hpPanel.Freeze(true);
    }

    public void Shield()
    {
        haveShield = true;
        shieldHp += maxHp * 0.2f;
        if(shieldHp > maxHp)
        {
            shieldHp = maxHp;
        }
        hpPanel.shieldHpImage.fillAmount = shieldHp / maxHp;
    }

    public void Heal(float healHp)
    {
        hp += healHp;
        if(hp > maxHp)
        {
            hp = maxHp;
        }
        hpPanel.hpImage.fillAmount = hp / maxHp;
    }

    public void GetBuff(Buff buffName, int turn, float percent)
    {
        BuffInfo _buff = new BuffInfo(buffIndex, buffName, turn, percent, this);
        buffDict.Add(buffIndex, _buff);
        buffIndex++;
        if (buffName.Equals(Buff.Speed))
        {
            speed += (int)percent;
            Debug.Log(gameObject.name + "�ӵ� ���� ����");
            GameManager.instance.battleManager.ChangePawnSpeed();
        }
        else if (buffName.Equals(Buff.ATKUp))
        {
            atkUpPercent += percent;
            Debug.Log(gameObject.name + "���ݷ� ���� ����");
        }
        else if (buffName.Equals(Buff.DEFUp))
        {
            defUpPercent += percent;
            Debug.Log(gameObject.name + "���� ���� ����");
        }
        else if (buffName.Equals(Buff.CriticalChance))
        {
            criticalChance += percent;
            Debug.Log(gameObject.name + "ġȮ ���� ����");
        }
        else if (buffName.Equals(Buff.CriticalMultiplier))
        {
            criticalMultiplier += percent;
            Debug.Log(gameObject.name + "ġ�� ���� ����");
        }
    }

    public void GetDebuff(Debuff debuffName, int turn, float percent)
    {
        DebuffInfo _buff = new DebuffInfo(buffIndex, debuffName, turn, percent, this);
        debuffDict.Add(buffIndex, _buff);
        buffIndex++;
        if (debuffName.Equals(Debuff.Speed))
        {
            speed -= (int)percent;
            Debug.Log(gameObject.name + "�ӵ� ���� �����");
            GameManager.instance.battleManager.ChangePawnSpeed();
        }
        else if (debuffName.Equals(Debuff.DEFUp))
        {
            defUpPercent -= percent;
            Debug.Log(gameObject.name + "���� ���� �����");
        }
    }

    public void FinishBuff(Buff buffName, float percent, int buffIndex)
    {
        buffDict[buffIndex] = null;
        if (buffName.Equals(Buff.Speed))
        {
            speed -= (int)percent;
            Debug.Log(gameObject.name + "�ӵ� ���� ���� ����");
            GameManager.instance.battleManager.ChangePawnSpeed();
        }
        else if (buffName.Equals(Buff.ATKUp))
        {
            atkUpPercent -= percent;
            Debug.Log(gameObject.name + "���ݷ� ���� ���� ����");
        }
        else if (buffName.Equals(Buff.DEFUp))
        {
            defUpPercent -= percent;
            Debug.Log(gameObject.name + "���� ���� ���� ����");
        }
        else if (buffName.Equals(Buff.CriticalChance))
        {
            criticalChance -= percent;
            Debug.Log(gameObject.name + "ġȮ ���� ���� ����");
        }
        else if (buffName.Equals(Buff.CriticalMultiplier))
        {
            criticalMultiplier -= percent;
            Debug.Log(gameObject.name + "ġ�� ���� ���� ����");
        }
    }

    public void FinishDebuff(Debuff debuffName, float percent, int buffIndex)
    {
        debuffDict[buffIndex] = null;
        if (debuffName.Equals(Debuff.Speed))
        {
            speed += (int)percent;
            Debug.Log(gameObject.name + "�ӵ� ���� ����� ����");
            GameManager.instance.battleManager.ChangePawnSpeed();
        }
        else if (debuffName.Equals(Debuff.DEFUp))
        {
            defUpPercent += percent;
            Debug.Log(gameObject.name + "���� ���� ����� ����");
        }
    }

    public void ActiveTargetingMark(bool active)
    {
        if (selectMark.activeSelf != active)
        {
            selectMark.SetActive(active);
        }
    }

    public virtual void Targetting(Transform targetTr)
    {
        if (player)
        {
            if (targetTr.CompareTag("Enemy"))
            {
                foreach(Pawn pawn in GameManager.instance.battleManager.playerList)
                {
                    pawn.ActiveTargetingMark(false);
                }
                foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
                {
                    if (pawn.transform.Equals(targetTr))
                    {
                        pawn.ActiveTargetingMark(true);
                        GameManager.instance.battleManager.targetList.Clear();
                        GameManager.instance.battleManager.targetList.Add(pawn);
                    }
                    else
                    {
                        pawn.ActiveTargetingMark(false);
                    }
                }
            }
        }
    }

    public virtual void FinisherTargetting(Transform targetTr)
    {
        if (player)
        {
            if (targetTr.CompareTag("Enemy"))
            {
                foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
                {
                    pawn.ActiveTargetingMark(false);
                }
                foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
                {
                    pawn.ActiveTargetingMark(false);
                }

                middleTarget = targetTr;
                int middleIndex = 0;
                foreach (int index in GameManager.instance.battleManager.enemyPositionDict.Keys)
                {
                    if (GameManager.instance.battleManager.enemyPositionDict[index] != null)
                    {
                        if (GameManager.instance.battleManager.enemyPositionDict[index].transform.Equals(targetTr))
                        {
                            // �߽� �ε��� ��ȯ
                            middleIndex = index;
                            break;
                        }
                    }
                }
                GameManager.instance.battleManager.targetList.Clear();
                for (int i = middleIndex - 1; i <= middleIndex + 1; i++)
                {
                    if (i >= 0 && i < 5)
                    {
                        if (GameManager.instance.battleManager.enemyPositionDict[i] != null)
                        {
                            GameManager.instance.battleManager.enemyPositionDict[i].ActiveTargetingMark(true);
                            GameManager.instance.battleManager.targetList.Add(GameManager.instance.battleManager.enemyPositionDict[i]);
                        }
                    }
                }
            }
        }
    }

    public virtual void Attack()
    {
        GameManager.instance.battleManager.waitingAction = false;
        GameManager.instance.battleManager.playingAction = true;
        GameManager.instance.battleManager.ActiveAttackButton(false);
        curTurnMark.SetActive(false);

        if (!basicPhysicsAttack)
        {
            return;
        }

        // �ൿ �κ�
        prevPos = transform.position;
        Vector3 targetPos = targetPos = new Vector3(GameManager.instance.battleManager.targetList[0].transform.position.x, GameManager.instance.battleManager.targetList[0].transform.position.y,
                player? GameManager.instance.battleManager.targetList[0].transform.position.z + 1 : GameManager.instance.battleManager.targetList[0].transform.position.z - 1);
        dist = Vector3.Distance(prevPos, targetPos);
        transform.DOMove(targetPos, dist * 0.1f).OnComplete(() =>
        {
            if (anim != null)
            {
                anim.SetBool(animAttack, true);
            }

            if (anim == null)
            {
                //Debug.Log(gameObject.name + " �Ϲݰ���");
                // ġ��Ÿ Ȯ�� �ݿ�
                float damage = atk * (1.0f + atkUpPercent * 0.01f);
                if (random.NextDouble() * 100 < criticalChance)
                {
                    damage = damage * (1.0f + criticalMultiplier / 100.0f);
                }

                GameManager.instance.battleManager.targetList[0].Hit(damage, physicsAtk);


                transform.DOMove(prevPos, dist * 0.2f).OnComplete(() =>
                {
                    if (actionCor != null)
                    {
                        StopCoroutine(actionCor);
                    }
                    actionCor = CorAction();
                    StartCoroutine(actionCor);
                });
            }
        });
    }

    public virtual void AttackEffect()
    {
        Debug.Log(gameObject.name + " �Ϲݰ���");
        float damage = atk * (1.0f + atkUpPercent * 0.01f);
        if (random.NextDouble() * 100 < criticalChance)
        {
            damage = damage * (1.0f + criticalMultiplier / 100.0f);
        }

        GameManager.instance.battleManager.targetList[0].Hit(damage, physicsAtk);
    }

    public virtual void FinishAttack()
    {
        anim.SetBool(animAttack, false);

        transform.DOMove(prevPos, dist * 0.2f).OnComplete(() =>
        {
            if (actionCor != null)
            {
                StopCoroutine(actionCor);
            }
            actionCor = CorAction();
            StartCoroutine(actionCor);
        });
    }

    public virtual void Finisher()
    {
        GameManager.instance.battleManager.waitingAction = false;
        GameManager.instance.battleManager.playingAction = true;
        GameManager.instance.battleManager.ActiveAttackButton(false);
        curTurnMark.SetActive(false);

        // �� ��ӵ� �Լ����� ���������� �ʻ�� ����
        // �ൿ �κ�
        //Vector3 prevPos = transform.position;
        //Vector3 moveDir = new Vector3(middleTarget.position.x - transform.position.x, 0, middleTarget.position.z - transform.position.z);
        //transform.DOMove(transform.position + moveDir.normalized * (Mathf.Abs(moveDir.magnitude) - 1), 0.5f).OnComplete(() =>
        //{
        //    Debug.Log(gameObject.name + " �ʻ��");
        //    // ġ��Ÿ Ȯ�� �ݿ�
        //    float damage = atk;
        //    if (random.NextDouble() * 100 < criticalChance + 20)
        //    {
        //        damage = damage * (1.0f + criticalMultiplier / 100.0f);
        //    }

        //    foreach (Pawn pawn in GameManager.instance.battleManager.targetList)
        //    {
        //        pawn.Hit(damage * 1.0f, physicsAtk);
        //    }

        //    transform.DOMove(prevPos, 1.0f).OnComplete(() =>
        //    {
        //        if (actionCor != null)
        //        {
        //            StopCoroutine(actionCor);
        //        }
        //        actionCor = CorAction();
        //        StartCoroutine(actionCor);
        //    });
        //});
    }

    public virtual void FinisherEffect()
    {
    }

    public virtual void FinishFinisher()
    {
    }

    // �⺻ Ÿ���� (ü���� ���� ���� ������ 1��(33%) �Ǵ� ������ ���� ���� ������ 1��(66%))
    public virtual void StartTargeting()
    {
        Transform targetTr = null;
        int rand = random.Next(3);

        if (player)
        {
            // ü���� ���� ���� ��
            if (rand == 0)
            {
                float minHp = 10000;
                Pawn minHpPawn = null;
                for (int i = 0; i < GameManager.instance.battleManager.enemyList.Count; i++)
                {
                    if (GameManager.instance.battleManager.enemyList[i].hp <= minHp)
                    {
                        minHp = GameManager.instance.battleManager.enemyList[i].hp;
                        minHpPawn = GameManager.instance.battleManager.enemyList[i];
                    }
                }
                targetTr = minHpPawn.transform;
            }
            // ������ ���� ���� ��
            else
            {
                float maxDef = -10;
                Pawn maxDefPawn = null;
                for (int i = 0; i < GameManager.instance.battleManager.enemyList.Count; i++)
                {
                    if (GameManager.instance.battleManager.enemyList[i].def * (1.0f + GameManager.instance.battleManager.enemyList[i].defUpPercent * 0.01f) >= maxDef)
                    {
                        maxDef = GameManager.instance.battleManager.enemyList[i].def * (1.0f + GameManager.instance.battleManager.enemyList[i].defUpPercent * 0.01f);
                        maxDefPawn = GameManager.instance.battleManager.enemyList[i];
                    }
                }
                targetTr = maxDefPawn.transform;
            }


            foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
            {
                pawn.ActiveTargetingMark(false);
            }
            foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
            {
                if (pawn.transform.Equals(targetTr))
                {
                    pawn.ActiveTargetingMark(true);
                    GameManager.instance.battleManager.targetList.Clear();
                    GameManager.instance.battleManager.targetList.Add(pawn);
                }
                else
                {
                    pawn.ActiveTargetingMark(false);
                }
            }
        }
        else
        {
            if (rand == 0)
            {
                float minHp = 10000;
                Pawn minHpPawn = null;
                for (int i = 0; i < GameManager.instance.battleManager.playerList.Count; i++)
                {
                    if (GameManager.instance.battleManager.playerList[i].hp <= minHp)
                    {
                        minHp = GameManager.instance.battleManager.playerList[i].hp;
                        minHpPawn = GameManager.instance.battleManager.playerList[i];
                    }
                }
                targetTr = minHpPawn.transform;
            }
            else
            {
                float maxDef = -10;
                Pawn maxDefPawn = null;
                for (int i = 0; i < GameManager.instance.battleManager.playerList.Count; i++)
                {
                    if (GameManager.instance.battleManager.playerList[i].def * (1.0f + GameManager.instance.battleManager.playerList[i].defUpPercent * 0.01f) >= maxDef)
                    {
                        maxDef = GameManager.instance.battleManager.playerList[i].def * (1.0f + GameManager.instance.battleManager.playerList[i].defUpPercent * 0.01f);
                        maxDefPawn = GameManager.instance.battleManager.playerList[i];
                    }
                }
                targetTr = maxDefPawn.transform;
            }

            foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
            {
                pawn.ActiveTargetingMark(false);
            }
            foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
            {
                if (pawn.transform.Equals(targetTr))
                {
                    pawn.ActiveTargetingMark(true);
                    GameManager.instance.battleManager.targetList.Clear();
                    GameManager.instance.battleManager.targetList.Add(pawn);
                }
                else
                {
                    pawn.ActiveTargetingMark(false);
                }
            }

            EnemyAttack();
        }
    }

    // �ʻ�� Ÿ���� (ü���� ���� ���� ������ 1��(33%) �Ǵ� ������ ���� ���� ������ 1��(66%))
    public virtual void StartFinisherTargeting()
    {
        Transform targetTr = null;
        int rand = random.Next(3);

        // ���� �߽ɿ��� ���� ������ Ÿ����
        // ü���� ���� ���� ��
        if (rand == 0)
        {
            float minHp = 10000;
            Pawn minHpPawn = null;
            for (int i = 0; i < GameManager.instance.battleManager.enemyList.Count; i++)
            {
                if (GameManager.instance.battleManager.enemyList[i].hp <= minHp)
                {
                    minHp = GameManager.instance.battleManager.enemyList[i].hp;
                    minHpPawn = GameManager.instance.battleManager.enemyList[i];
                }
            }
            targetTr = minHpPawn.transform;
        }
        // ������ ���� ���� ��
        else
        {
            float maxDef = -10;
            Pawn maxDefPawn = null;
            for (int i = 0; i < GameManager.instance.battleManager.enemyList.Count; i++)
            {
                if (GameManager.instance.battleManager.enemyList[i].def * (1.0f + GameManager.instance.battleManager.enemyList[i].defUpPercent * 0.01f) >= maxDef)
                {
                    maxDef = GameManager.instance.battleManager.enemyList[i].def * (1.0f + GameManager.instance.battleManager.enemyList[i].defUpPercent * 0.01f);
                    maxDefPawn = GameManager.instance.battleManager.enemyList[i];
                }
            }
            targetTr = maxDefPawn.transform;
        }
        middleTarget = targetTr;

        foreach (Pawn pawn in GameManager.instance.battleManager.playerList)
        {
            pawn.ActiveTargetingMark(false);
        }
        foreach (Pawn pawn in GameManager.instance.battleManager.enemyList)
        {
            pawn.ActiveTargetingMark(false);
        }

        int middleIndex = 0;
        foreach (int index in GameManager.instance.battleManager.enemyPositionDict.Keys)
        {
            if (GameManager.instance.battleManager.enemyPositionDict[index] != null)
            {
                if (GameManager.instance.battleManager.enemyPositionDict[index].transform.Equals(targetTr))
                {
                    // �߽� �ε��� ��ȯ
                    middleIndex = index;
                    break;
                }
            }
        }
        GameManager.instance.battleManager.targetList.Clear();
        for (int i = middleIndex - 1; i <= middleIndex + 1; i++)
        {
            if (i >= 0 && i < 5)
            {
                if (GameManager.instance.battleManager.enemyPositionDict[i] != null)
                {
                    GameManager.instance.battleManager.enemyPositionDict[i].ActiveTargetingMark(true);
                    GameManager.instance.battleManager.targetList.Add(GameManager.instance.battleManager.enemyPositionDict[i]);
                }
            }
        }
    }

    protected virtual void EnemyAttack()
    {
        if(enemyCor != null)
        {
            StopCoroutine(enemyCor);
        }
        enemyCor = CorEnemyAttack();
        StartCoroutine(enemyCor);
    }

    private void CheckStatus()
    {
        if(burnCount > 0)
        {
            burnCount--;
            Hit(hp * 0.1f, false, true);
            if(burnCount == 0)
            {
                hpPanel.Burn(false);
            }
        }
        if (freezeCount > 0)
        {
            freezeCount--;
            Hit(hp * 0.1f, false, true);
            if (freezeCount == 0)
            {
                hpPanel.Freeze(false);
            }
        }
    }

    protected void CheckBuff()
    {
        if (buffDict.Count > 0)
        {
            for(int i = 0; i < buffIndex + 1; i++)
            {
                if (buffDict.ContainsKey(i))
                {
                    if (buffDict[i] != null)
                    {
                        buffDict[i].Turn();
                    }
                }
            }
            //foreach (int key in buffDict.Keys)
            //{
            //    if (buffDict[key] != null)
            //    {
            //        buffDict[key].Turn();
            //    }
            //}
        }
        if (debuffDict.Count > 0)
        {
            for (int i = 0; i < buffIndex + 1; i++)
            {
                if (debuffDict.ContainsKey(i))
                {
                    if (debuffDict[i] != null)
                    {
                        debuffDict[i].Turn();
                    }
                }
            }
            //foreach (int key in debuffDict.Keys)
            //{
            //    if (debuffDict[key] != null)
            //    {
            //        debuffDict[key].Turn();
            //    }
            //}
        }
    }

    protected IEnumerator CorEnemyAttack()
    {
        yield return new WaitForSeconds(0.5f);

        GameManager.instance.battleManager.AttackButton(false);
    }

    // �ൿ�ϴ� �Լ� (������Ÿ���� ���Ƿ� 1�ʸ��� �ൿ�ϱ� ������ �ڷ�ƾ���� ��������)
    protected IEnumerator CorAction()
    {
        //int prevSpeed = speed;
        //GameManager.instance.battleManager.PawnAction(this);

        //// ���Ƿ� �ൿ �� �ӵ� -10 ~ +10 ��ȭ�� ��
        //ChangeSpeed();
        //Debug.Log(gameObject.name + " �ൿ " + "      ���� �ӵ�: " + prevSpeed + "       �ൿ �� �ӵ�: " + speed);
        //GameManager.instance.battleManager.ChangePawnSpeed();

        //int randNum = 0;
        //// ���Ƿ� ����� ������ 1���� hp�� 20 ����
        //if (player)
        //{
        //    randNum = random.Next(GameManager.instance.battleManager.enemyList.Count);
        //    Debug.Log(gameObject.name + " ====== ���� =====>  " + GameManager.instance.battleManager.enemyList[randNum].gameObject.name);
        //    GameManager.instance.battleManager.enemyList[randNum].Hit(20);
        //}
        //else
        //{
        //    randNum = random.Next(random.Next(GameManager.instance.battleManager.playerList.Count));
        //    Debug.Log(gameObject.name + " ====== ���� =====>  " + GameManager.instance.battleManager.playerList[randNum].gameObject.name);
        //    GameManager.instance.battleManager.playerList[random.Next(GameManager.instance.battleManager.playerList.Count)].Hit(20);
        //}

        //Attack();

        yield return actionTime;

        if (curTurnMark.activeSelf)
        {
            curTurnMark.SetActive(false);
        }

        GameManager.instance.battleManager.playingAction = false;
        CheckBuff();
        GameManager.instance.battleManager.PawnAction(this);
        GameManager.instance.battleManager.turnUI.FInishTurn(this);
        GameManager.instance.battleManager.FinishAction();

        StopCoroutine(actionCor);
    }
}

public class BuffInfo
{
    private int buffIndex = 0;
    private Pawn.Buff buffName = 0;
    private int buffLeftTurn = 0;
    private float buffChangePercent = 0;
    private Pawn pawn = null;

    public BuffInfo(int buffIndex, Pawn.Buff buffName, int buffLeftTurn, float buffChangePercent, Pawn pawn)
    {
        this.buffIndex = buffIndex;
        this.buffName = buffName;
        this.buffLeftTurn = buffLeftTurn;
        this.buffChangePercent = buffChangePercent;
        this.pawn = pawn;
    }

    public void Turn()
    {
        buffLeftTurn--;
        if(buffLeftTurn <= 0)
        {
            pawn.FinishBuff(buffName, buffChangePercent, buffIndex);
        }
    }
}

public class DebuffInfo
{
    private int debuffIndex = 0;
    private Pawn.Debuff debuffName = 0;
    private int debuffLeftTurn = 0;
    private float debuffChangePercent = 0;
    private Pawn pawn = null;

    public DebuffInfo(int debuffIndex, Pawn.Debuff debuffName, int debuffLeftTurn, float debuffChangePercent, Pawn pawn)
    {
        this.debuffIndex = debuffIndex;
        this.debuffName = debuffName;
        this.debuffLeftTurn = debuffLeftTurn;
        this.debuffChangePercent = debuffChangePercent;
        this.pawn = pawn;
    }

    public void Turn()
    {
        debuffLeftTurn--;
        if (debuffLeftTurn <= 0)
        {
            pawn.FinishDebuff(debuffName, debuffChangePercent, debuffIndex);
        }
    }
}