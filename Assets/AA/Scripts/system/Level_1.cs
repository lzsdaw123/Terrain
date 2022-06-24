using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Level_1 : MonoBehaviour
{
    public CameraMove cameraMove;
    public static bool Level_A_Start = false;  //任務觸發碰撞
    [SerializeField] bool SF_Level_A_Start;  //任務觸發碰撞
    public static int LevelA_, LevelB_;  //關卡判定
    [SerializeField] int SF_LevelA_, SF_LevelB_;  //關卡判定
    GameObject SpawnRay;
    public SpawnRay _SpawnRay;
    [SerializeField] GameObject explode;
    [SerializeField] GameObject PSexplode;
    [SerializeField] ParticleSystem PSsmoke;
    float GameStartTime = 0;
    [SerializeField] bool Lv1;
    public static int TotalStage;  //關卡階段 ------------TotalStage
    [SerializeField] int SF_TotalStage;  //關卡階段 ------------TotalStage
    public int missionLevel;  //任務關卡--------------Level
    static int missionStage;  //任務階段-----------Stage
    [SerializeField] int SF_missionStage;  //顯示任務階段
    public int EnemyWave;  //敵人波數
    public static float stageTime = -1;  //下次來襲時間
    [SerializeField] float SF_stageTime;
    public Text stageTimeText;  //怪物進攻倒數文字
    public LayerMask LayerMask;
    public static bool AttackStart = false;  //進攻開始
    public GameObject MissionTarget, MissionWarn;  //任務警告UI
    public GameObject[] MissionUI;  //怪物進攻倒數
    public GameObject tagetUI;  //任務目標UI
    public static int[] missionNumb = new int[] { 7, 1, 3, 3 };  //任務數量
    bool Mission_L1;  //關卡重生點
    public GameObject DialogBox;
    public Text MissonTxet;
    public string[] MissonStringL1; //L1任務標題
    public string[] MissonStringL1_2; //L1任務標題
    public string[] MissonStringL2; //L2任務標題
    public string[] MissonStringL2_2; //L2_2任務標題
    public string[] MissonStringL3; //L3任務標題
    public string[] MissonString; //當前任務標題
    [SerializeField] float UiTime;
    public static float MissionTime;  //任務切換時間-------------MissionTime
    [SerializeField] float SF_MissionTime;  //任務切換時間
    public static bool UiOpen;
    [SerializeField] bool SF_UiOpen;
    [SerializeField] public static int MonsterLevel;
    [SerializeField] float MLtime;
    int DifficultyLevel;  //難度等級
    public RectTransform DiffUI, DiffUI_s;
    public static float StartTime;  //開場倒數
    [SerializeField] float SF_StartTime;  //開場倒數
    bool PlayAu;  //音效
    [SerializeField] float Taget_distance;  //目標距離
    public static bool StartDialogue;  //開始對話
    public static bool MissionEnd = false;
    public static bool StopAttack; //暫停怪物進攻
    [SerializeField] bool SF_StopAttack; //暫停怪物進攻
    public GameObject[] Objects;  //開放使用物件
    public float DelayTime;  //延遲倒數
    public static bool minRain;  //降低下雨
    public GameObject[] M_Trigger;
    public static float BossDeadTime;
    public static bool MonsterDead;
    public static int BG_Level;

    void Awake()
    {
        Lv1 = false;
        MonsterLevel = 0;
        PlayAu = true;
        LevelA_= LevelB_ = 0;
        missionLevel = 0;
        EnemyWave = 0;
        MLtime = 0;
    }
    void Start()
    {
        Settings.BirthPoint.SetActive(true);
        PlayerResurrection.PlayerBirth();
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
        DelayTime  = -1;
        BossDeadTime = 0;
        MonsterDead = false;
        MissionWarn.SetActive(false);
        MissonStringL1 = new string[] { "管理與監控", "物資儲存", "取得武器", "補充彈藥", "修理與升級", "電力供應",  "到工作崗位"};
        MissonStringL1_2 = new string[] { "到工作崗位"};
        MissonStringL2 = new string[] { "大門防線", "武器開放", "不明生物來襲", "前往研究室", "重大發現", "尋找遺跡" };
        MissonStringL2_2 = new string[] { "第二防線", "保護發電廠", "任務失敗" };
        MissonStringL3 = new string[] { "遺跡探索", "攻擊弱點", "防禦機槍", "阻止黑球", "逃離", "垂直降落", "結束" };

        MissionWarn.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 94, 0);
        StartDialogue = true;
        StopAttack = false;
        Objects[3].GetComponent<Animator>().enabled = false;
        M_Trigger[0].SetActive(false);
        DontDestroyOnLoad(gameObject);  //切換場景時保留
    }
    void Update()
    {
        int SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        switch (SceneNub)
        {
            case 1:
                Destroy(gameObject);               
                break;
            case 2:
                TotalStage = 1;
                break;
            case 3:
                TotalStage = 2;
                MissonString = MissonStringL3;
                Shooting.SkipTeach = true;
                break;
        }
        BG_Level = 1;
        if (PlayerResurrection.ReDelete)
        {
             Destroy(gameObject);
        }
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
        SF_TotalStage = TotalStage;
        missionLevel = PlayerView.missionLevel;
        missionStage = PlayerView.missionStage;  //跟換目標
        SF_Level_A_Start = Level_A_Start;
        SF_missionStage = missionStage;
        Taget_distance = PlayerView.pu_distance;
        SF_stageTime = stageTime;
        SF_LevelA_ = LevelA_;
        SF_LevelB_ = LevelB_;
        SF_StopAttack = StopAttack;
        SF_MissionTime = MissionTime;
        SF_UiOpen = UiOpen;
        SF_StartTime = StartTime;

        if (!MissionEnd)  //任務目標是否結束
        {
            switch (TotalStage)
            {
                case 1:
                    switch (missionLevel)  //任務關卡
                    {
                        case 0:
                            MissonString = MissonStringL1;  //第一階段
                            if (missionStage != 2 && missionStage != 3 && missionStage != 4)  //靠近任務點
                            {
                                if (MissionTime >= 2.2 && LevelA_ != 2)  //任務UI浮現時間
                                {
                                    MissionTime = 2.2f;
                                    if (missionStage == 1 || missionStage == 5)
                                    {
                                        if (StartDialogue)
                                        {
                                            StartDialogue = false;
                                            DialogueEditor.StartConversation(missionLevel, missionStage, 0, true, 0, true);  //開始對話
                                        }
                                    }
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
                            if (missionStage == 3)
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
                                M_Trigger[0].SetActive(true);
                            }
                            break;
                        case 1:
                            MissonString = MissonStringL1_2;  //第一之一階段  跳過教學
                            if (missionStage == 0 && LevelA_ == 0)
                            {
                                LevelA_ = 1;
                                Level_A_Start = true;
                                M_Trigger[0].SetActive(true);
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
                case 3:  //怪物入侵
                    if (MissionTime >= 5) 
                    {
                        LevelA_ = 4;  //完整
                        LevelA_ = 6;  //試玩
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
                        Area_Loading.AreaLoading(1);
                    }
                    break;
                case 11:
                    if (MissionTime >= 10)
                    {
                        MissionTime = 10;
                        if (Taget_distance <= 2f)  //觸發距離
                        {
                            LevelA_ = 12;
                            //ExitGame();
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
            if (TotalStage==1)
            {
                if (StartTime >= 12)  //遊戲開始後 開啟任務UI
                {
                    StartTime = -1;
                    UiOpen = true;
                }
                else if (StartTime < 17 && StartTime >= 0)
                {
                    StartTime += 2 * Time.deltaTime;
                }
            }

            if (AttackStart)  //Input.GetKeyDown(KeyCode.F1)
            {
                AudioManager.explode();
                explode.SetActive(true);
                Force.開始破門();
                SpawnRay.SetActive(true);
                Lv1 = true;
                PlayAu = true;
                minRain = true;  //降低下雨量
            }
        }
        if (Lv1)  //開始第一關
        {
            TotalStage = 1;
            if (GameStartTime >= 0)
            {
                GameStartTime += Time.deltaTime;
            }
            if (GameStartTime >= 3)
            {
                PSexplode.SetActive(false);
            }
            if (GameStartTime >= 4)
            {
                var main = PSsmoke.main;
                main.loop = false;
                MissionTarget.SetActive(true);
            }
            if (GameStartTime >= 25f)
            {
                GameStartTime = -1;
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
        switch (TotalStage)
        {
            case 1:  // 第一關
                if (EnemyWave < 2)  //進攻波數
                {
                    if (StopAttack) return;  //處於暫停進攻狀態
                    if (stageTime <= 0 && stageTime > -1)  //下次來襲時間  開始進攻
                    {
                        _SpawnRay.StartBorn = true;  //怪物開始生成
                        _SpawnRay.BornTime = 0;  //怪物開始生成
                        EnemyWave++;
                        _SpawnRay.EnemyWaveNum(EnemyWave);
                        stageTime = -1;
                        if (LevelA_ >= 5)
                        {
                            MissionUI[1].SetActive(true);
                            MissionUI[0].GetComponent<Animator>().SetBool("Stop", true);  //倒數UI暫停
                            MissionUI[0].GetComponent<Animator>().speed = 0;
                            PlayAu = true;
                            PlayAudio();
                        }
                    }
                    else if (stageTime > 0)  //開始倒數
                    {
                        MissionUI[0].GetComponent<Animator>().SetBool("Stop", false);  //倒數UI開始
                        MissionUI[0].GetComponent<Animator>().SetTrigger("Start");  //倒數UI啟動
                        MissionUI[0].GetComponent<Animator>().speed = 1;
                        stageTime -= Time.deltaTime;
                    }
                    int Minute = 0;
                    int Second = 0;
                    string srtMinute = "";
                    string srtSecond = "";
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
                        if (Minute < 10) srtMinute = "0" + Minute;
                        if (Second < 10) srtSecond = "0" + Second;
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
                                stageTime = 6;  //怪物繼續倒數開始
                                PlayerView.missionChange(2, 2);
                                DialogueEditor.StartConversation(2, 3, 1, false, 0, true);
                                break;
                            case 9:  //前往研究室
                                print(LevelA_);
                                minRain = true;  //調回下雨量
                                UiOpen = true;
                                PlayerView.missionChange(2, 3);
                                DialogueEditor.StartConversation(2, 4, 0, false, 0, true);
                                Objects[4].GetComponent<BoxCollider>().enabled = true; //開放研究室
                                MissionTarget.SetActive(false);
                                MissionUI[0].SetActive(false);
                                MissionUI[1].SetActive(false);
                                break;
                        }
                    }
                }
                //當前波數結束
                if (SceneNub == 2)
                {
                    if (!_SpawnRay.StartBorn && _SpawnRay.counter[0] == 0 && _SpawnRay.counter[1] == 0 && _SpawnRay.counter[2] == 0)
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
                                cameraMove.CameraMovement(0, 6, 3, true);  //(哪台, 速度, 延遲, 二次)
                                DelayTime = 0;  //延遲倒數
                                break;
                            case 8: //擊敗水晶Boss並打光怪物
                                //print("8");
                                //MonsterDead = true;
                                //BossDeadTime = 0;
                                //LevelA_ = 9;
                                //StopAttack = true;
                                //DelayTime = 0;  //延遲倒數
                                //Area_Loading.AreaLoading(0);
                                break;
                        }
                    }
                }
                if (LevelA_ == 8)
                {
                    print("8");
                    MonsterDead = true;
                    BossDeadTime = 0;
                    LevelA_ = 9;
                    StopAttack = true;
                    DelayTime = 0;  //延遲倒數
                    EnemyWave = 2;
                    Area_Loading.AreaLoading(0);
                }
                break;
            case 2:  //第二關  TotalStage
                switch (missionStage)
                {
                    case 0:
                        //if (LevelB_ == 1)//第二關開場
                        //{
                        //    LevelB_ = 2;
                        //    PlayerView.Stop = false;  //UI隱藏
                        //    PlayerView.missionChange(4, 0);  //改變關卡
                        //    DialogueEditor.StartConversation(4, 0, 2, false, 0, true);  //開始對話
                        //    UiOpen = true;
                        //}                  
                        break;
                }
                break;
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
    public void MissionTrigger(int Level, GameObject _Objects)  //任務觸碰觸發
    {
        switch (Level)
        {
            case 0:
                Level_A_Start = false;
                Objects[3].GetComponent<Animator>().enabled = true;  //開起警報
                LevelA_ = 2;
                MissionTime = 0;
                break;
            case 1:
                _Objects.transform.GetChild(0).GetComponent<Animator>().SetBool("Start", true);
                cameraMove.CameraMovement(2,2, 0, false); //(哪台, 速度, 延遲, 二次)
                break;
        }
      
    }
    public void OnTriggerEnter(Collider other)  //開始第一關
    {
        //if (Level_A_Start)
        //{
        //    if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        //    {
        //        if (other.tag == "Player")
        //        {
        //            Level_A_Start = false;
        //            Objects[3].GetComponent<Animator>().enabled = true;  //開起警報
        //            LevelA_ = 2;
        //            MissionTime = 0;
        //        }
        //    }
        //}
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
    public int EnemyNum_A;  //怪物1波數
    public int EnemyNum_B;  //怪物2波數
    public int EnemyNum_C;  //怪物3波數

    /// <summary>
    /// 怪物進攻波次
    /// </summary>
    /// <param name="EnemyNum_A">怪物1波數</param>
    /// <param name="EnemyNum_B">怪物2波數</param>
    /// <param name="EnemyNum_C">怪物2波數</param>
    /// <returns></returns>
    public EnemyWaveNum(int enemyNumA, int enemyNumB, int enemyNumC)
    {
        EnemyNum_A = enemyNumA;
        EnemyNum_B = enemyNumB;
        EnemyNum_C = enemyNumC;
    }
}
[Serializable]
public class MonsterAttributes  //怪物屬性
{
    public float MinSize;  //怪物最小尺寸
    public float MaxSize;  //怪物最大尺寸

    /// <summary>
    /// 怪物屬性
    /// </summary>
    /// <param name="minSize">怪物最小尺寸</param>
    /// <param name="maxSize">怪物最大尺寸</param>
    /// <returns></returns>
    public MonsterAttributes(float minSize, float maxSize)
    {
        MinSize = minSize;
        MaxSize = maxSize;
    }
}
