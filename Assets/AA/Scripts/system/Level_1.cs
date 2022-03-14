using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Level_1 : MonoBehaviour
{
    public static bool Level_A_Start=false;
    public static int LevelA_;
    GameObject SpawnRay;
    public SpawnRay _SpawnRay;
    [SerializeField] GameObject explode;
    [SerializeField] GameObject PSexplode;
    [SerializeField] ParticleSystem PSsmoke;
    float time = 0;
    [SerializeField] bool Lv1;
    int stage;  //關卡階段
    public int EnemyWave=0;  //敵人波數
    public static float stageTime=-1;
    [SerializeField] float SF_stageTime;
    public LayerMask LayerMask;
    public static bool start=false;
    public GameObject MissionTarget, MissionWarn;  //任務警告UI
    public GameObject tagetUI;  //任務目標UI
    public int missionLevel=0;  //任務關卡
    public static int[] missionNumb= new int[] {7,5 };  //任務數量
    static int missionStage;  //任務階段
    [SerializeField] int SF_missionStage;  //任務階段
    bool Mission_L1;
    public GameObject DialogBox;
    public Text MissonTxet;
    bool MissT;
    public string[] MissonStringL1; //L1任務標題
    public string[] MissonStringL1_2; //L1任務標題
    public string[] MissonStringL2; //L2任務標題
    public string[] MissonString; //當前任務標題
    [SerializeField] float UiTime;
    public static float MissionTime;  //任務切換時間
   public static bool UiOpen;
    [SerializeField]public static int MonsterLevel;
    [SerializeField] float MLtime = 0;
    int Level;
    public RectTransform DiffUI, DiffUI_s;
    [SerializeField] float StartTime;
    bool PlayAu;
    [SerializeField] float Taget_distance;
    public static bool StartDialogue;
    public static bool MissionEnd=false;
    bool Lv1End;
    public GameObject[] Objects; 

    void Awake()
    {
        Lv1 = false;
        MonsterLevel = 0;
        PlayAu = true;
        LevelA_ = 0;
    }
    void Start()
    {
        Mission_L1 = PlayerResurrection.Mission_L1;
        StartTime = 0;
        if (Mission_L1) StartTime = 15;
        SpawnRay = GameObject.Find("SpawnRay");
        _SpawnRay = SpawnRay.GetComponent<SpawnRay>();
        SpawnRay.SetActive(false);
        explode.SetActive(false);
        MissionTarget.SetActive(false);
        tagetUI.SetActive(false);
        DialogBox.SetActive(false);
        MissionWarn.SetActive(false);
        MissonStringL1 = new string[] { "管理與監控", "物資儲存", "取得武器", "取得彈藥", "修理與升級", "電力供應",  "到工作崗位"};
        MissonStringL1_2 = new string[] { "到工作崗位"};
        MissonStringL2 = new string[] { "大門防線", "第二防線", "保護發電廠", "任務完成" };

        MissionWarn.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 368, 0);
        StartDialogue = true;
    }
    void Update()
    {
        missionLevel = PlayerView.missionLevel;
        missionStage = PlayerView.missionStage;  //跟換目標
        SF_missionStage = missionStage;
        Taget_distance = PlayerView.pu_distance;
        SF_stageTime = stageTime;
        if (!MissionEnd)  //任務目標是否結束
        {
            switch (missionLevel)
            {
                case 0:
                    MissonString = MissonStringL1;
                    if (missionStage != 2 && missionStage != 3)  //靠近任務點
                    {
                        if (MissionTime >= 3 && LevelA_ != 2)  //任務UI浮現時間
                        {
                            MissionTime = 3;
                            if (Taget_distance <= 1f)
                            {
                                if (StartDialogue)
                                {
                                    StartDialogue = false;
                                    DialogueEditor.StartConversation(missionLevel, missionStage, 0);  //開始對話
                                }
                            }
                        }
                        else MissionTime += Time.deltaTime;
                    }
                    if (missionStage==3)
                    {
                        Objects[0].GetComponent<BoxCollider>().enabled = true;
                        Objects[1].GetComponent<BoxCollider>().enabled = true;
                    }
                    if (missionStage == 6 && LevelA_ == 0)
                    {
                        LevelA_ = 1;
                        Level_A_Start = true;
                    }
                    break;
                case 1:
                    MissonString = MissonStringL1_2;
                    if (missionStage == 0 && LevelA_ == 0)
                    {
                        LevelA_ = 1;
                        Level_A_Start = true;
                    }                   
                    break;
                case 2:
                    MissonString = MissonStringL2;
                    break;
            }
            
            if (UiOpen)
            {
                UiOpen = false;
                MissonTxet.text = MissonString[missionStage];
                MissionWarn.SetActive(true);  // 開啟任務UI
                tagetUI.SetActive(true);
                PlayAu = true;
                PlayAudio();
            }
            if (MissionWarn.activeSelf)  //任務UI開啟
            {
                if (UiTime >= 3.6)  //UI浮現時間
                {
                    MissionWarn.SetActive(false);
                    UiTime = 0;
                }
                else
                {
                    UiTime += Time.deltaTime;
                }
            }
            if (LevelA_ == 2)
            {
                if (MissionTime >= 4)
                {
                    LevelA_ = 3;
                    PlayerView.missionLevel = 2;
                    PlayerView.missionStage = 0;
                    DialogueEditor.StartConversation(2, 0, 0);  //開始對話
                    MissionTime = 0;
                    UiOpen = true;
                }
                else
                {
                    MissionTime += Time.deltaTime;
                }
            }
        }
        else
        {
            MissionWarn.SetActive(false);
        }
        if (!Lv1)
        {
            if (StartTime >= 17)  //遊戲開始後 開啟任務UI
            {
                StartTime = -1;
                UiOpen = true;
            }
            else if(StartTime < 17 && StartTime>=0)
            {
                StartTime += 2 * Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.F1) || start)
            {
                AudioManager.explode();
                explode.SetActive(true);
                Force.開始破門();
                SpawnRay.SetActive(true);
                Lv1 = true;
                PlayAu = true;
            }
        }
        if (Lv1)
        {
            stage = 1;
            time += Time.deltaTime;
            if (time >= 3)
            {
                PSexplode.SetActive(false);
            }
            if (time >= 4)
            {
                var main = PSsmoke.main;
                main.loop = false;
                MissionTarget.SetActive(true);
            }
            if (time >= 25f)
            {
                explode.SetActive(false);
            }
            if (MonsterLevel != 5)  //升級時間冷卻
            {
                MLtime += Time.deltaTime;
                //MLtime += 10*Time.deltaTime;
            }
            Level = Settings.Level;
            Level = 90 / (Level + 1);
            if (MLtime >= Level)  //難度設定 90 / 45 / 30 秒升級
            {
                MLtime = 0;
                MonsterLevel++;
                //print("難度等級"+ Settings.Level + " / 怪物等級"+MonsterLevel+" / 難度升級時間:"+ Level);
            }
        }
        if (stage==1)
        {                
            if(EnemyWave <2)  //進攻波數
            {
                if (stageTime>= 25)  //進階時間
                {
                    _SpawnRay.StartBorn = true;
                    EnemyWave++;
                    _SpawnRay.EnemyWaveNum(EnemyWave);
                    stageTime = -1;
                }
                else if(stageTime >=0)
                {
                    stageTime += Time.deltaTime;
                }
            }
            else
            {
                if(!Lv1End && _SpawnRay.counter[0] == 0 && _SpawnRay.counter[1] == 0)
                {
                    Lv1End = true;
                    //DialogueEditor.StartConversation(missionLevel, 9, 0);
                    PlayerView.missionStage = 8;
                }
            }
        }
        switch (Settings.Level)  //難度圖示
        {
            case 0:
                DiffUI.sizeDelta = new Vector2(129, 100);
                DiffUI_s.anchoredPosition3D = new Vector3(0, 0, 0);
                break;
            case 1:
                DiffUI.sizeDelta = new Vector2(175, 100);
                DiffUI_s.anchoredPosition3D = new Vector3(50, 0, 0);
                break;
            case 2:
                DiffUI.sizeDelta = new Vector2(265, 100);
                DiffUI_s.anchoredPosition3D = new Vector3(0, 0, 0);
                break;
        }
    }
    void PlayAudio()
    {
        if (PlayAu)
        {
            PlayAu = false;
            AudioManager.Warn(1);
        }
    }
    public void OnTriggerEnter(Collider other)  //開始第一關
    {
        if (Level_A_Start)
        {
            Level_A_Start = false;
            if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
            {
                if (other.tag == "Player")
                {
                    start = true;
                    LevelA_ = 2;
                    MissionTime = 0;
                }
            }
        }
    }

    public static void NextTask(int nextTask)
    {
        DialogueEditor.StartConversation(0, nextTask, 0);  //開始對話
    }
}


[Serializable]
public class EnemyWaveNum  //敵人每波數量
{
    public int EnemyNumA;  //怪物1波數
    public int EnemyNumB;  //怪物2波數
    public int EnemyNumC;  //怪物3波數

    /// <summary>
    /// 怪物進攻波次
    /// </summary>
    /// <param name="enemyNumA">怪物1波數</param>
    /// <param name="enemyNumB">怪物2波數</param>
    /// <returns></returns>
    public EnemyWaveNum(int enemyNumA, int enemyNumB)
    {
        EnemyNumA = enemyNumA;
        EnemyNumB = enemyNumB;
    }
}
