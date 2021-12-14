using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level_1 : MonoBehaviour
{
    public bool Level_1_Start=false;
    GameObject SpawnRay;
    [SerializeField] GameObject explode;
    [SerializeField] GameObject PSexplode;
    [SerializeField] ParticleSystem PSsmoke;
    float time = 0;
    [SerializeField] bool Lv1;
    public LayerMask LayerMask;
    bool start=false;
    public GameObject MissionTarget, MissionWarn;  //任務警告UI
    public GameObject tagetUI;  //任務目標UI
    static int missionLevel;  //任務階段 0=主管,1=武器
    bool Mission_L1;
    public GameObject DialogBox;
    public Text MissonTxet;
    public string[] MissonT;
    [SerializeField] float UiTime;
    public static float MissionTime;  //任務切換時間
   public static bool UiOpen=true;
    [SerializeField]public static int MonsterLevel;
    [SerializeField] float MLtime = 0;
    int Level;
    public RectTransform DiffUI, DiffUI_s;
    [SerializeField] float StartTime;
    bool PlayAu;
    [SerializeField] float Taget_distance;
    public static bool StartDialogue;

    void Awake()
    {
        Lv1 = false;
        MonsterLevel = 0;
        PlayAu = true;
    }
    void Start()
    {
        Mission_L1 = PlayerResurrection.Mission_L1;
        StartTime = 0;
        if (Mission_L1) StartTime = 15;
        SpawnRay = GameObject.Find("SpawnRay");
        SpawnRay.SetActive(false);
        explode.SetActive(false);
        MissionTarget.SetActive(false);
        tagetUI.SetActive(false);
        DialogBox.SetActive(false);
        MissionWarn.SetActive(false);
        MissonT = new string[] { "尋找主管", "前往倉庫", "取得武器與彈藥", "前往工作間" };
        MissonTxet.text = MissonT[0];
        MissionWarn.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 368, 0);
        StartDialogue = true;
    }
    void Update()
    {
        missionLevel = PlayerView.missionLevel;  //跟換目標
        Taget_distance = PlayerView.pu_distance;
        if(missionLevel==0 || missionLevel == 1)
        {
            if (MissionTime >= 3)  //UI浮現時間
            {
                MissionTime = 3;
                if (Taget_distance <= 0.9f)
                {
                    if (StartDialogue)
                    {
                        StartDialogue = false;
                        DialogueEditor.StartConversation(missionLevel, 0);  //開始對話
                    }
                }
            }
            else MissionTime += Time.deltaTime;
        }

        if (MissionWarn.activeSelf)
        {
            MissonTxet.text = MissonT[missionLevel];
            if (UiTime >= 3)  //UI浮現時間
            {
                MissionWarn.SetActive(false);
                UiTime = 0;
            }
            else
            {
                UiTime += Time.deltaTime;
            }
        }

        if (!Lv1)
        {
            if (StartTime >= 17 && UiOpen)  //遊戲開始後
            {
                StartTime = 17;
                UiOpen = false;
                MissionWarn.SetActive(true);
                tagetUI.SetActive(true);
                PlayAu = true;
                PlayAudio();
            }
            else if(StartTime < 17)
            {
                StartTime += 2 * Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.F1) || start)
            {
                AudioManager.explode();
                Level_1_Start = true;
                explode.SetActive(true);
                Force.開始破門();
                SpawnRay.SetActive(true);
                Lv1 = true;
                PlayAu = true;
            }
        }
        if (Lv1)
        {
            MissionWarn.SetActive(false);
            time += Time.deltaTime;
            if (time >= 3)
            {
                PSexplode.SetActive(false);
            }
            if (time >= 4)
            {
                MissionTarget.SetActive(true);
                MissionWarn.SetActive(true);
                PlayAudio();
                MissonTxet.text = "保護發電站";
                var main = PSsmoke.main;
                main.loop = false;
            }
            if (time >= 10f)
            {
                MissionWarn.SetActive(false);
            }
            if (time >=25f)
            {
                explode.SetActive(false);
            }
            if (MonsterLevel != 5)
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
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                start = true;
            }
        }
    }
    public static void NextTask(int nextTask)
    {
        DialogueEditor.StartConversation(nextTask, 0);  //開始對話
    }
}
