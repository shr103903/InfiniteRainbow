using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public BattleManager battleManager = null;

    public JsonUtil saveJsonUtil = null;

    public List<GameObject> playerList = new List<GameObject>();

    [HideInInspector]
    public int mapNum = 0;

    public List<EnemyDeckData> map01LowEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map01MediumEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map01HighEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map01BossEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map02LowEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map02MediumEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map02HighEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map02BossEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map03LowEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map03MediumEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map03HighEnemyDeckDataList = new List<EnemyDeckData>();
    public List<EnemyDeckData> map03BossEnemyDeckDataList = new List<EnemyDeckData>();

    private IEnumerator sceneCor = null;

    private WaitForSeconds sceneCheck = new WaitForSeconds(0.1f);

    private WaitForSeconds second = new WaitForSeconds(0.1f);

    private System.Random random = new System.Random();


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                Destroy(instance.gameObject);
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void Start()
    {
        saveJsonUtil = GetComponent<JsonUtil>();
        FadeInOutUI.instance.StartFadeIn();
        SoundManager.instance.Play("BGM/Main", Define.Sound.Bgm);
    }

    public void StartGame()
    {
        StatusData.floor = 0;
        ResetStatus();
        NextGame();
    }

    public void LoadGame()
    {
        if (saveJsonUtil.LoadData())
        {
            NextGame();
        }
    }

    public bool SaveGame()
    {
        return saveJsonUtil.SaveData();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ActiveSoundPanel(bool active)
    {
        SoundManager.instance.ActiveSoundPanel(active);
    }

    public void NextGame()
    {
        if (sceneCor != null)
        {
            StopCoroutine(sceneCor);
        }
        sceneCor = CorCheckSceneLoad();
        StartCoroutine(sceneCor);
    }

    public void MoveFloor()
    {
        if (battleManager == null)
        {
            battleManager = GameObject.FindAnyObjectByType<BattleManager>();
        }

        if (StatusData.floor % 5 != 0)
        {
            battleManager.BuildMap();
            if(StatusData.floor % 5 == 4)
            {
                StatusData.floor++;
                battleManager.difficulty = 2; 
                battleManager.ActiveMapSelect(false);
                battleManager.DifficultySelectedAction();
                SoundManager.instance.Play("BGM/Boss", Define.Sound.Bgm);
                return;
            }
            battleManager.ActiveMapSelect(false);
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
        else
        {
            battleManager.ActiveMapSelect(true);
        }
        StatusData.floor++;
    }

    public void Defeat()
    {
        if(sceneCor != null)
        {
            StopCoroutine(sceneCor);
        }
        sceneCor = CorGoMain();
        StartCoroutine(sceneCor);
    }

    public void ResetStatus()
    {
        StatusData.SetData();
    }

    public void UpgradeStatus(int num)
    {
        if(num == 0)
        {
            StatusData.hpUpgrade[battleManager.difficulty]++;
        }
        else if (num == 1)
        {
            StatusData.atkUpgrade[battleManager.difficulty]++;
        }
        else if (num == 2)
        {
            StatusData.defUpgrade[battleManager.difficulty]++;
        }
        else if (num == 3)
        {
            StatusData.dodgeUpgrade[battleManager.difficulty]++;
        }
        else if (num == 4)
        {
            StatusData.criChanceUpgrade[battleManager.difficulty]++;
        }
        else if (num == 5)
        {
            StatusData.criMultiUpgrade[battleManager.difficulty]++;
        }
        else if (num == 6)
        {
            StatusData.speedUpgrade[battleManager.difficulty]++;
        }
    }

    public EnemyDeckData GetEnemyDeck(int difficulty)
    {
        if(mapNum == 0)
        {
            if (StatusData.floor % 5 == 0)
            {
                return map01BossEnemyDeckDataList[random.Next(map01BossEnemyDeckDataList.Count)];
            }
            else if (difficulty == 0)
            {
                return map01LowEnemyDeckDataList[random.Next(map01LowEnemyDeckDataList.Count)];
            }
            else if (difficulty == 1)
            {
                return map01MediumEnemyDeckDataList[random.Next(map01MediumEnemyDeckDataList.Count)];
            }
            else
            {
                return map01HighEnemyDeckDataList[random.Next(map01HighEnemyDeckDataList.Count)];
            }
        }
        else if (mapNum == 1)
        {
            if (StatusData.floor % 5 == 0)
            {
                return map02BossEnemyDeckDataList[random.Next(map02BossEnemyDeckDataList.Count)];
            }
            else if (difficulty == 0)
            {
                return map02LowEnemyDeckDataList[random.Next(map02LowEnemyDeckDataList.Count)];
            }
            else if (difficulty == 1)
            {
                return map02MediumEnemyDeckDataList[random.Next(map02MediumEnemyDeckDataList.Count)];
            }
            else 
            {
                return map02HighEnemyDeckDataList[random.Next(map02HighEnemyDeckDataList.Count)];
            }
        }
        else
        {
            if (StatusData.floor % 5 == 0)
            {
                return map03BossEnemyDeckDataList[random.Next(map03BossEnemyDeckDataList.Count)];
            }
            else if (difficulty == 0)
            {
                return map03LowEnemyDeckDataList[random.Next(map03LowEnemyDeckDataList.Count)];
            }
            else if (difficulty == 1)
            {
                return map03MediumEnemyDeckDataList[random.Next(map03MediumEnemyDeckDataList.Count)];
            }
            else
            {
                return map03HighEnemyDeckDataList[random.Next(map03HighEnemyDeckDataList.Count)];
            }
        }
    }

    private IEnumerator CorCheckSceneLoad()
    {
        FadeInOutUI.instance.StartFadeOut(NextSceneAction);
        yield return null;
        StopCoroutine(sceneCor);
    }

    private void NextSceneAction()
    {
        if (sceneCor != null)
        {
            StopCoroutine(sceneCor);
        }
        sceneCor = CorSceneLoad();
        StartCoroutine(sceneCor);
    }

    private IEnumerator CorSceneLoad()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(1);

        while (!op.isDone)
        {
            yield return sceneCheck;
        }
        MoveFloor();
        StopCoroutine(sceneCor);
    }

    private IEnumerator CorGoMain()
    {
        FadeInOutUI.instance.StartFadeOut(GoMainAction);
        yield return null;
        StopCoroutine(sceneCor);
    }

    private void GoMainAction()
    {
        SceneManager.LoadScene(0);
    }
}
