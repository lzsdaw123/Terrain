using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Level_1 : MonoBehaviour
{
    public CameraMove cameraMove;
    public static bool Level_A_Start = false;  //任務觸發碰撞
    [SerializeField] bool SF_Level_A_Start;  //任務觸發碰撞
    public static int LevelA_;  //關卡判定
    [SerializeField] int SF_LevelA_;  //關卡判定
    GameObject SpawnRay;
    public SpawnRay _SpawnRay;
    [SerializeField] GameObject explode;
    [SerializeField] GameObject PSexplode;
    [SerializeField] ParticleSystem PSsmoke;
    float time = 0;
    [SerializeField] bool Lv1;
    int TotalStage;  //關卡階段
    public int EnemyWave;  //敵人波數
    public static float stageTime = -1;  //下次來襲時間
    [SerializeField] float SF_stageTime;
    public Text stageTimeText;  //怪物進攻倒數文字
    public LayerMask LayerMask;
    public static bool AttackStart = false;  //進攻開始
    public GameObject MissionTarget, MissionWarn;  //任務警告UI
    public GameObject[] MissionUI;  //怪物進攻倒數
    public GameObject tagetUI;  //任務目標UI
    public int missionLevel;  //任務關卡
    public static int[] missionNumb = new int[] { 7, 1, 3, 3 };  //任務數量
    static int missionStage;  //任務階段
    [SerializeField] int SF_missionStage;  //顯示任務階段
    bool Mission_L1;  //關卡重生點
    public GameObject DialogBox;
    public Text MissonTxet;
    public string[] MissonStringL1; //L1任務標題
    public string[] MissonStringL1_2; //L1任務標題
    public string[] MissonStringL2; //L2任務標題
    public string[] MissonStringL2_2; //L2_2任務標題
    public string[] MissonString; //當前任務標題
    [SerializeField] float UiTime;
    public static float MissionTime;  //任務切換時間
    [SerializeField] float SF_MissionTime;  //任務切換時間
    public static bool UiOpen;
    [SerializeField] public static int MonsterLevel;
    [SerializeField] float MLtime;
    int DifficultyLevel;  //難度等級
    public RectTransform DiffUI, DiffUI_s;
    [SerializeField] float StartTime;
    bool PlayAu;  //音效
    [SerializeField] float Taget_distance;  //目標距離
    public static bool StartDialogue;  //開始對話
    public static bool MissionEnd = false;
    public static bool StopAttack; //暫停怪物進攻
    [SerializeField] bool SF_StopAttack; //暫停怪物進攻
    public GameObject[] Objects;  //開放使用物件
    public float DelayTime;  //延遲倒數

    void Awake()
    {
        Lv1 = false;
        MonsterLevel = 0;
        PlayAu = true;
        LevelA_ = 0;
        missionLevel = 0;
        EnemyWave = 0;
        MLtime = 0;
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
        MissionUI[0].SetActive(false);
        tagetUI.SetActive(false);
        //DialogBox.SetActive(false);
        DelayTime = -1;
        MissionWarn.SetActive(false);
        MissonStringL1 = new string[] { "管理與監控", "物資儲存", "取得武器", "補充彈藥", "修理與升級", "電力供應",  "到工作崗位"};
        MissonStringL1_2 = new string[] { "到工作崗位"};
        MissonStringL2 = new string[] { "大門防線", "武器開放", "不明生物來襲", "前往研究室", "重大發現", "尋找遺跡" };
        MissonStringL2_2 = new string[] { "第二防線", "保護發電廠", "任務失敗" };

        MissionWarn.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 94, 0);
        StartDialogue = true;
        StopAttack = false;
        Objects[3].GetComponent<Animator>().enabled = false;
    }
    void Update()
    {
        //if (Input.GetKey(KeyCode.F5))
        //{
        //    PlayerView.missionLevel = 2;
        //    //PlayerView.missionStage=5;
        //    LevelA_ = 9;
        //    UiOpen = true;
        //    PlayerView.missionChange(2, 3);
        //    DialogueEditor.StartConversation(2, 4, 0, false, 0, true);
        //    Objects[4].GetComponent<BoxCollider>().enabled = true; //開放研究室
        //}
        missionLevel = PlayerView.missionLevel;
        missionStage = PlayerView.missionStage;  //跟換目標
        SF_Level_A_Start = Level_A_Start;
        SF_missionStage = missionStage;
        Taget_distance = PlayerView.pu_distance;
        SF_stageTime = stageTime;
        SF_LevelA_ = LevelA_;
        SF_StopAttack = StopAttack;
        SF_MissionTime = MissionTime;

        if (!MissionEnd)  //任務目標是否結束
        {
            switch (missionLevel)  //任務關卡
            {
                case 0:
                    MissonString = MissonStringL1;  //第一階段
                    if (missionStage != 2 && missionStage != 3 && missionStage != 4)  //靠近任務點
                    {
                        if (MissionTime >= 3 && LevelA_ != 2)  //任務UI浮現時間
                        {
                            MissionTime = 3;
                            if (Taget_distance <= 1.1f)  //觸發距離 原1.5f
                            {
                                if (StartDialogue)
                                {
                                    StartDialogue = false;
                                    DialogueEditor.StartConversation(missionLevel, missionStage, 0, true, 0, true);  //開始對話
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
                    if (missionStage == 5)
                    {
                        DialogueEditor.Delayed(4); //延遲對話
                    }
                    if (missionStage == 6 && LevelA_ == 0)
                    {
                        LevelA_ = 1;
                        Level_A_Start = true;
                    }
                    break;
                case 1:
                    MissonString = MissonStringL1_2;  //第一之一階段  跳過教學
                    if (missionStage == 0 && LevelA_ == 0)
                    {
                        LevelA_ = 1;
                        Level_A_Start = true;
                        Objects[0].GetComponent<BoxCollider>().enabled = true;
                        Objects[1].GetComponent<BoxCollider>().enabled = true;
                    }                   
                    break;
                case 2:
                    MissonString = MissonStringL2;  //第二階段
                    break;
                case 3:
                    MissonString = MissonStringL2_2;  //第二之二階段
                    break;
            }
            
            if (UiOpen)  //UI開啟
            {
                UiOpen = false;
                MissonTxet.text = MissonString[missionStage];
                MissionWarn.SetActive(true);  // 開啟任務UI
                MissionWarn.gameObject.transform.GetChild(0).GetComponent<Animator>().SetInteger("Type", 0);
                tagetUI.SetActive(true);
                PlayAu = true;
                PlayAudio();
            }
            if (MissionWarn.activeSelf)  //任務UI開啟
            {
                if (UiTime >= 6)  //UI浮現時間
                {
                    MissionWarn.SetActive(false);
                    UiTime = 0;
                }
                else
                {
                    UiTime += Time.deltaTime;
                }
            }
            switch (LevelA_)
            {
                case 2:
                    if (MissionTime >= 2)
                    {
                        PlayerView.missionChange(2, 0);  //改變關卡
                        DialogueEditor.StartConversation(2, 0, 1, false, 2.0f, true);  //開始對話
                        DialogueEditor.Delayed(2); //延遲對話
                        UiOpen = true;
                        LevelA_ = 3;
                        MissionTime = 0;
                    }
                    else MissionTime += Time.deltaTime;
                    break;
                case 3:
                    if (MissionTime >= 8)
                    {
                        LevelA_ = 4;
                        MissionTime = 0;
                        Objects[3].GetComponent<Animator>().enabled = false;  //關閉警報
                        GameObject.Find("Alarm").GetComponent<AudioSource>().enabled = false;
                        AttackStart = true;
                    }
                    else MissionTime += Time.deltaTime;
                    break;
                case 8:
                    _SpawnRay.BornTime = 20;  //怪物結束生成
                    _SpawnRay.StartBool[0] = true;  //怪物結束生成
                    break;
                case 9:
                    if (Objects[4].GetComponent<ElectricDoor>().Botton)  //與研究室主管對話
                    {
                        LevelA_ = 10;
                        UiOpen = true;
                        PlayerView.missionChange(2, 4);  //改變關卡
                    }
                    break;
                case 10:
                    if (Taget_distance <= 1.5f)  //觸發距離
                    {
                        LevelA_ = 11;
                        DialogueEditor.StartConversation(2, 5, 2, true, 0, false);  //開始對話
                        Objects[5].GetComponent<BoxCollider>().enabled = true; //開放研究室後門
                    }
                    break;
                case 11:
                    if (MissionTime >= 10)
                    {
                        MissionTime = 10;
                        if (Taget_distance <= 2f)  //觸發距離
                        {
                            LevelA_ = 12;
                            ExitGame();
                        }
                    }
                    else MissionTime += Time.deltaTime;
                    break;
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
            if (Input.GetKeyDown(KeyCode.F1) || AttackStart)
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
            TotalStage = 1;
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
            //if (MonsterLevel != 5)  //升級時間冷卻
            //{
            //    MLtime += Time.deltaTime;
            //    //MLtime += 10*Time.deltaTime;
            //}
            //DifficultyLevel = Settings.Level;
            //DifficultyLevel = 90 / (DifficultyLevel + 1);
            //if (MLtime >= DifficultyLevel)  //難度設定 90 / 45 / 30 秒升級
            //{
            //    MLtime = 0;
            //    MonsterLevel++;
            //    //print("難度等級"+ Settings.Level + " / 怪物等級"+MonsterLevel+" / 難度升級時間:"+ Level);
            //}
        }
        if (TotalStage==1)  //第一大關
        {                
            if(EnemyWave <2)  //進攻波數
            {
                if (StopAttack) return;  //處於暫停進攻狀態
                if (stageTime<= 0 && stageTime>-1)  //下次來襲時間  開始進攻
                {
                    _SpawnRay.StartBorn = true;  //怪物開始生成
                    _SpawnRay.BornTime = 0;  //怪物開始生成
                    EnemyWave++;
                    _SpawnRay.EnemyWaveNum(EnemyWave);
                    stageTime = -1;
                    if (LevelA_>=5)
                    {
                        MissionUI[1].SetActive(true);
                        MissionUI[0].GetComponent<Animator>().SetBool("Stop", true);
                        MissionUI[0].GetComponent<Animator>().speed = 0;
                        PlayAu = true;
                        PlayAudio();
                    }
                }
                else if(stageTime >0)  //開始倒數
                {
                    MissionUI[0].GetComponent<Animator>().SetBool("Stop", false);
                    MissionUI[0].GetComponent<Animator>().speed = 1;
                    stageTime -= Time.deltaTime;
                }
                int Minute = 0;
                int Second = 0;
                string srtMinute = "";
                string srtSecond="";
                if (stageTime <= -1)  
                {
                    stageTimeText.text = "  ";
                }
                else
                {
                    Minute = (int)stageTime / 60;
                    Second = (int)stageTime - (Minute * 60);
                    srtMinute = "" + Minute;
                    srtSecond = "" + Second;
                    if(Minute < 10) srtMinute = "0" + Minute;
                    if(Second <10) srtSecond = "0" + Second;
                    stageTimeText.text = srtMinute + " : " + srtSecond;
                }
            }
            if (DelayTime >= 0)  //延遲倒數
            {
                DelayTime += Time.deltaTime;
                if (DelayTime >= 2)
                {
                    DelayTime = -1;
                    switch (LevelA_)
                    {
                        case 7:  //出現水晶Boss
                            UiOpen = true;  //開啟任務UI與音效
                            stageTime = 5;  //怪物繼續倒數開始
                            PlayerView.missionChange(2, 2);
                            DialogueEditor.StartConversation(2, 3, 1, false, 0, true);
                            break;
                        case 9:  //前往研究室
                            UiOpen = true;
                            PlayerView.missionChange(2, 3);
                            DialogueEditor.StartConversation(2, 4, 0, false, 0, true);
                            Objects[4].GetComponent<BoxCollider>().enabled = true; //開放研究室
                            break;
                    }
                }
            }
            if (!_SpawnRay.StartBorn && _SpawnRay.counter[0] == 0 && _SpawnRay.counter[1] == 0)  //當前波數結束
            {
                switch (LevelA_)
                {
                    case 4:  //第一波結束
                        MissionUI[0].SetActive(true);
                        LevelA_ = 5;
                        StopAttack = true;
                        UiOpen = true;
                        PlayerView.missionChange(2, 1);  //改變關卡
                        DialogueEditor.StartConversation(2, 1, 0, false, 0, true);  //開放左輪使用
                        Objects[2].GetComponent<BoxCollider>().enabled = true;
                        break;
                    case 6:  //第二波結束
                        print("6");
                        LevelA_ = 7;
                        StopAttack = true;
                        cameraMove.StartBoss1();
                        DelayTime = 0;  //延遲倒數
                        break;
                    case 8: //擊敗水晶Boss並打光怪物
                        print("8");
                        LevelA_ = 9;
                        StopAttack = true;
                        DelayTime = 0;  //延遲倒數
                        break;
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
            if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
            {
                if (other.tag == "Player")
                {
                    Level_A_Start = false;
                    Objects[3].GetComponent<Animator>().enabled = true;  //開起警報
                    LevelA_ = 2;
                    MissionTime = 0;
                }
            }
        }
    }
    public void ExitGame()
    {
        Settings.pause();
        Settings.ExitGame();
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
[Serializable]
public class MonsterAttributes  //怪物屬性
{
    public float MinSize;  //怪物1波數
    public float MaxSize;  //怪物2波數
                           //public int EnemyNumC;  //怪物3波數

    /// <summary>
    /// 怪物屬性
    /// </summary>
    /// <param name="minSize">怪物1波數</param>
    /// <param name="maxSize">怪物2波數</param>
    /// <returns></returns>
    public MonsterAttributes(float minSize, float maxSize)
    {
        MinSize = minSize;
        MaxSize = maxSize;
    }
}
