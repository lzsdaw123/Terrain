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
    static int missionLevel;  //任務階段
    public GameObject DialogBox;
    public Text MissonTxet;
    [SerializeField]public static int MonsterLevel;
    [SerializeField] float MLtime = 0;
    int Level;
    public RectTransform DiffUI, DiffUI_s;
    float StartTime;
    bool PlayAu;
    [SerializeField] float Taget_distance;
    bool StartDialogue;

    void Awake()
    {
        Lv1 = false;
        MonsterLevel = 0;
        StartTime = 0;
        PlayAu = true;
    }
    void Start()
    {
        SpawnRay= GameObject.Find("SpawnRay");
        SpawnRay.SetActive(false);
        explode.SetActive(false);
        MissionTarget.SetActive(false);
        tagetUI.SetActive(false);
        DialogBox.SetActive(false);
        MissionWarn.SetActive(false);
        MissonTxet.text = "前往主管室";
        MissionWarn.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 368, 0);
        StartDialogue = true;
    }

    void Update()
    {
        Taget_distance = PlayerView.pu_distance;
        if (Taget_distance <= 0.9f)
        {
            if (StartDialogue)
            {
                StartDialogue = false;
                DialogueEditor.StartConversation(0, 0);  //開始對話
            }
        }

        if (!Lv1)
        {
            StartTime += 2 * Time.deltaTime;
            if (StartTime >= 17)
            {
                MissionWarn.SetActive(true);
                tagetUI.SetActive(true);
                PlayAudio();
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
    public static void FirstWeapon()
    {
        missionLevel = 2;
        DialogueEditor.StartConversation(1, 0);  //開始對話
    }
}
