using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

public class BattleManager : MonoBehaviour
{
    // 아군 덱
    public List<Pawn> playerList = new List<Pawn>();

    // 적 덱
    public List<Pawn> enemyList = new List<Pawn>();

    public DeckResult deck = null;

    public BattleTurnUI turnUI = null;

    public bool waitingAction = false;

    public bool playingAction = false;

    public List<Pawn> currentTurnList = new List<Pawn>();

    public List<Pawn> nextTurnList = new List<Pawn>();

    [HideInInspector]
    public BattlePositionList positionParent = null;

    [SerializeField]
    private int round = 0;

    [SerializeField]
    private List<GameObject> mapPrefab = new List<GameObject>();

    [SerializeField]
    private GameObject mapSelectPanel = null;

    [SerializeField]
    private GameObject difficultyPanel = null;

    [SerializeField]
    private GameObject statusSelectParent = null;

    [SerializeField]
    private GameObject buttonParent = null;

    [SerializeField]
    private GameObject selectDeckPanel = null;

    [SerializeField]
    private List<StatusSelectButton> statusButtonList = new List<StatusSelectButton>(); 

    [SerializeField]
    private TMP_Text floorText = null;

    [SerializeField]
    private BattleButton atkButton = null;

    [SerializeField]
    private BattleButton finisherButton = null;

    [SerializeField]
    private Transform hpCanvas = null;

    [SerializeField]
    private GameObject hpPanelPrefab = null;

    [SerializeField]
    private GameObject finishMask = null;

    [SerializeField]
    private TMP_Text finishCountText = null;

    [SerializeField]
    private DisplayPawnData displayDataPanel = null;

    public List<Pawn> targetList = new List<Pawn>();

    [HideInInspector]
    public Dictionary<int, Pawn> playerPositionDict = new Dictionary<int, Pawn>();

    [HideInInspector]
    public Dictionary<int, Pawn> enemyPositionDict = new Dictionary<int, Pawn>();

    [HideInInspector]
    public int difficulty = 0;

    private Ray ray;

    private RaycastHit rayHit;

    private bool finisherActive = false;

    private bool curActiveAtkBtn = true;

    private int finishCount = 0;

    private IEnumerator sceneBuildCor = null;

    private WaitForSeconds second = new WaitForSeconds(1.0f);

    private System.Random random = new System.Random();

    private void Awake()
    {
        finisherActive = false;
        finishMask.SetActive(true);
        finishCount = 0;
        curActiveAtkBtn = true;

        SoundManager.instance.Stop(Define.Sound.Bgm);
    }

    private void Start()
    {
        round = 0;
        waitingAction = false;
        playingAction = false;
        atkButton.Active(true);
        finisherButton.Active(false);

        GameManager.instance.battleManager = this;
        floorText.text = $"{StatusData.floor + 1}층";

        FadeInOutUI.instance.StartFadeIn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetSelectStatus();
            if (sceneBuildCor != null)
            {
                StopCoroutine(sceneBuildCor);
            }
            sceneBuildCor = CorWin();
            StartCoroutine(sceneBuildCor);
            //GameManager.instance.NextGame();
        }

        if (waitingAction)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Physics.Raycast(ray, out rayHit);
                if (rayHit.transform == null)
                {
                    return;
                }
                //if(!(rayHit.transform.CompareTag("Enemy") || rayHit.transform.CompareTag("Player")))
                //{
                //    return;
                //}

                if (curActiveAtkBtn)
                {
                    currentTurnList[0].Targetting(rayHit.transform);
                }
                else
                {
                    currentTurnList[0].FinisherTargetting(rayHit.transform);
                }
            }
        }
    }

    public void DisplayPawnData(Pawn pawn)
    {
        displayDataPanel.DisplayData(pawn);
    }

    public void MoveNextFloor()
    {
        GameManager.instance.SaveGame();
        GameManager.instance.NextGame();
    }

    public void SetDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
        if (sceneBuildCor != null)
        {
            StopCoroutine(sceneBuildCor);
        }
        sceneBuildCor = CorDifficultySelected(difficulty);
        StartCoroutine(sceneBuildCor);
    }

    public void SetMap(int num)
    {
        GameManager.instance.mapNum = num;
        if (sceneBuildCor != null)
        {
            StopCoroutine(sceneBuildCor);
        }
        sceneBuildCor = CorMapSelected();
        StartCoroutine(sceneBuildCor);
    }

    public void BuildMap()
    {
        GameObject map = GameObject.Instantiate(mapPrefab[GameManager.instance.mapNum]);
    }

    public void StartBattle()
    {
        if (deck.deckPlayerList.Count == 0)
        {
            return;
        }

        if (sceneBuildCor != null)
        {
            StopCoroutine(sceneBuildCor);
        }
        sceneBuildCor = CorStartBattle();
        StartCoroutine(sceneBuildCor);

        if (StatusData.floor % 5 == 1)
        {
            if (GameManager.instance.mapNum == 0)
            {
                SoundManager.instance.Play("BGM/Desert", Define.Sound.Bgm);
            }
            else if (GameManager.instance.mapNum == 1)
            {
                SoundManager.instance.Play("BGM/Forest", Define.Sound.Bgm);
            }
            else
            {
                SoundManager.instance.Play("BGM/Dungeon", Define.Sound.Bgm);
            }
        }
    }

    public void AttackButton(bool player = true)
    {
        if ((waitingAction && player) || !player)
        {
            if (!curActiveAtkBtn)
            {
                curActiveAtkBtn = true;
                atkButton.Active(true);
                finisherButton.Active(false);
                currentTurnList[0].StartTargeting();
                return;
            }

            currentTurnList[0].Attack();
            FinishTargeting();
        }
    }

    public void FinisherButton()
    {
        if (waitingAction)
        {
            if (curActiveAtkBtn)
            {
                curActiveAtkBtn = false;
                atkButton.Active(false);
                finisherButton.Active(true);
                currentTurnList[0].StartFinisherTargeting();
                return;
            }

            finisherActive = false;
            finishMask.SetActive(true);
            finishCount = 0;
            finishCountText.text = $"[HIT {finishCount}/5]";
            currentTurnList[0].Finisher();
            FinishTargeting();
        }
    }

    public void ActiveAttackButton(bool active)
    {
        if (buttonParent.activeSelf != active)
        {
            buttonParent.SetActive(active);
        }
        curActiveAtkBtn = true;
        atkButton.Active(true);
        finisherButton.Active(false);
    }

    public void ActiveMapSelect(bool active)
    {
        if (active)
        {
            RandomMap();
        }
       
        if (mapSelectPanel.activeSelf != active)
        {
            mapSelectPanel.SetActive(active);
        }
    }

    private void RandomMap()
    {
        for(int i = 0; i < mapSelectPanel.transform.childCount; i++)
        {
            if (mapSelectPanel.transform.GetChild(i).gameObject.activeSelf)
            {
                mapSelectPanel.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        int prev = random.Next(mapSelectPanel.transform.childCount);
        mapSelectPanel.transform.GetChild(prev).gameObject.SetActive(true);

        int rand = 0;
        while (true)
        {
            rand = random.Next(mapSelectPanel.transform.childCount);
            if (rand != prev)
            {
                mapSelectPanel.transform.GetChild(rand).gameObject.SetActive(true);
                break;
            }
        }
    }

    private void FinishTargeting()
    {
        foreach (Pawn pawn in enemyList)
        {
            pawn.ActiveTargetingMark(false);
        }
        foreach (Pawn pawn in playerList)
        {
            pawn.ActiveTargetingMark(false);
        }
    }

    public void PawnAction(Pawn pawn)
    {
        currentTurnList.Remove(pawn);
    }

    // 속도가 변한 폰 존재
    public void ChangePawnSpeed()
    {
        List<Pawn> prevTurnList = new List<Pawn>();
        if(currentTurnList.Count > 0)
        {
            for(int i = 0; i < currentTurnList.Count; i++)
            {
                prevTurnList.Add(currentTurnList[i]);
            }
            ChangeTurn(ref currentTurnList, true);
        }
        for (int i = 0; i < playerList.Count + enemyList.Count - currentTurnList.Count; i++)
        {
            prevTurnList.Add(nextTurnList[i]);
        }
        ChangeTurn(ref nextTurnList, true);

        List<Pawn> changeTurnList = new List<Pawn>();
        if (currentTurnList.Count > 0)
        {
            for (int i = 0; i < currentTurnList.Count; i++)
            {
                changeTurnList.Add(currentTurnList[i]);
            }
        }
        for (int i = 0; i < playerList.Count + enemyList.Count - currentTurnList.Count; i++)
        {
            changeTurnList.Add(nextTurnList[i]);
        }
        turnUI.ChangeTurn(ref prevTurnList, ref changeTurnList);
    }

    public void PlayerHit()
    {
        if (finisherActive)
        {
            return;
        }
        finishCount++;
        if(finishCount >= 5)
        {
            finisherActive = true;
            finishMask.SetActive(false);
            return;
        }
        finishCountText.text = $"[HIT {finishCount}/5]";
    }

    // 행동 종료
    public void FinishAction()
    {
        if(playerList.Count == 0)
        {
            Debug.Log("패배");
            GameManager.instance.Defeat();
            return;
        }
        else if (enemyList.Count == 0)
        {
            Debug.Log("승리");
            SetSelectStatus();
            if (sceneBuildCor != null)
            {
                StopCoroutine(sceneBuildCor);
            }
            sceneBuildCor = CorWin();
            StartCoroutine(sceneBuildCor);
            //GameManager.instance.NextGame();
            return;
        }


        if (currentTurnList.Count == 0)
        {
            currentTurnList = nextTurnList.ToList();
            Debug.Log(round + "라운드 종료");
            round++;
        }

        currentTurnList[0].StartAction();
    }

    // HP가 0이하인 폰 존재
    public void Killed(Pawn pawn)
    {
        if(currentTurnList.Count > 0)
        {
            if (currentTurnList.Contains(pawn))
            {
                currentTurnList.Remove(pawn);
            }
        }
        if (nextTurnList.Contains(pawn))
        {
            nextTurnList.Remove(pawn);
        }
        if (playerList.Contains(pawn))
        {
            playerList.Remove(pawn);
        }
        if (enemyList.Contains(pawn))
        {
            enemyList.Remove(pawn);
        }

        turnUI.RemoveTurn(pawn);
    }

    private void SetSelectStatus()
    {
        List<int> numList = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
        int num = 0;
        for(int i = 0; i < 3; i++)
        {
            num = numList[random.Next(numList.Count)];
            numList.Remove(num);
            statusButtonList[i].num = num;
            statusButtonList[i].SetText();
        }
    }

    private void SetTurn(ref List<Pawn> list)
    {
        if(list.Count > 0)
        {
            list.Clear();
        }

        foreach(Pawn pawn in playerList)
        {
            list.Add(pawn);
        }
        foreach (Pawn pawn in enemyList)
        {
            list.Add(pawn);
        }
        list = list.OrderBy(a => random.Next()).ToList();

        ChangeTurn(ref list);
    }

    // 행동 순서 업데이트하는 함수
    private void ChangeTurn(ref List<Pawn> pawnList, bool action = false)
    {
        List<Pawn> sortList = new List<Pawn>();
        foreach (Pawn pawn in pawnList)
        {
            sortList.Add(pawn);
            if (sortList.Count != 1)
            {
                for (int i = 0; i < sortList.Count - 1; i++)
                {
                    if(i == 0 && action)
                    {
                        continue;
                    }

                    if (pawn.speed > sortList[i].speed)
                    {
                        for (int j = sortList.Count - 2; j >= i; j--)
                        {
                            sortList[j + 1] = sortList[j];
                        }
                        sortList[i] = pawn;
                        break;
                    }
                    else if (pawn.speed == sortList[i].speed)
                    {
                        if (pawn.pawnNumber < sortList[i].pawnNumber)
                        {
                            for (int j = sortList.Count - 2; j >= i; j--)
                            {
                                sortList[j + 1] = sortList[j];
                            }
                            sortList[i] = pawn;
                            break;
                        }
                    }
                }
            }
        }

        pawnList = sortList.ToList();
    }

    private void SetPosition()
    {
        for(int i = 0; i < 4; i++)
        {
            if(deck.deckPlayerList.Count <= i)
            {
                break;
            }
            GameObject player = GameObject.Instantiate(GameManager.instance.playerList[deck.deckPlayerList[i].playerNum]);
            Pawn pawn = player.GetComponent<Pawn>();
            playerList.Add(pawn);

            HpPanel hpPanel = GameObject.Instantiate(hpPanelPrefab, hpCanvas).GetComponent<HpPanel>();
            pawn.hpPanel = hpPanel;
            hpPanel.pawn = pawn;
            hpPanel.transform.position = Camera.main.WorldToScreenPoint(pawn.hpPanelPosition.position);
            hpPanel.transform.position = new Vector3(hpPanel.transform.position.x, hpPanel.transform.position.y, 0);
        }
        for (int i = 0; i < enemyList.Count; i++)
        {
            HpPanel hpPanel = GameObject.Instantiate(hpPanelPrefab, hpCanvas).GetComponent<HpPanel>();
            enemyList[i].hpPanel = hpPanel;
            hpPanel.pawn = enemyList[i];
            hpPanel.transform.position = Camera.main.WorldToScreenPoint(enemyList[i].hpPanelPosition.position);
            hpPanel.transform.position = new Vector3(hpPanel.transform.position.x, hpPanel.transform.position.y, 0);
        }

        for(int i = 0; i < 5; i++)
        {
            enemyPositionDict.Add(i, null);
        }
        for (int i = 3; i >= 0; i--)
        {
            playerPositionDict.Add(i, null);
            if (playerList.Count > i)
            {
                playerList[i].transform.position = positionParent.transform.GetChild(0).GetChild(i).transform.position;
                playerPositionDict[i] = playerList[i];
            }
        }

        for (int i = 4; i >= 0; i--)
        {
            int j = 0;
            if (enemyList.Count > i)
            {
                if (i == 4) j = 4;
                else if (i == 3) j = 0;
                else if (i == 2) j = 3;
                else if (i == 1) j = 1;
                else if (i == 0) j = 2;
                enemyList[i].transform.position = positionParent.transform.GetChild(1).GetChild(i).transform.position;
                enemyPositionDict[j] = enemyList[i];
            }
        }
    }

    private IEnumerator CorMapSelected()
    {
        FadeInOutUI.instance.StartFadeOut(MapSelectedAction);
        StopCoroutine(sceneBuildCor);
        yield return null;
    }

    private void MapSelectedAction()
    {
        BuildMap();
        ActiveMapSelect(false);

        FadeInOutUI.instance.StartFadeIn();
    }

    private IEnumerator CorDifficultySelected(int difficulty)
    {
        FadeInOutUI.instance.StartFadeOut(DifficultySelectedAction);
        StopCoroutine(sceneBuildCor);
        yield return null;
    }

    public void DifficultySelectedAction()
    {
        deck.AddEnemy(difficulty);
        difficultyPanel.SetActive(false);

        FadeInOutUI.instance.StartFadeIn();
    }

    private IEnumerator CorStartBattle()
    {
        FadeInOutUI.instance.StartFadeOut(StartBattleAction);
        StopCoroutine(sceneBuildCor);
        yield return null;
    }

    private void StartBattleAction()
    {
        SetPosition();
        SetTurn(ref currentTurnList);
        SetTurn(ref nextTurnList);

        turnUI.setTurn();

        selectDeckPanel.SetActive(false);

        //currentTurnList[0].StartAction();

        FadeInOutUI.instance.StartFadeIn();

        if (sceneBuildCor != null)
        {
            StopCoroutine(sceneBuildCor);
        }
        sceneBuildCor = CorStartTurn();
        StartCoroutine(sceneBuildCor);
    }

    private IEnumerator CorStartTurn()
    {
        yield return second;
        yield return second;

        currentTurnList[0].StartAction();

        StopCoroutine(sceneBuildCor);
    }

    private IEnumerator CorWin()
    {
        FadeInOutUI.instance.StartFadeOut(StartTurnAction);
        yield return null;
        StopCoroutine(sceneBuildCor);
    }

    private void StartTurnAction()
    {
        statusSelectParent.SetActive(true);
        FadeInOutUI.instance.StartFadeIn();
    }
}
