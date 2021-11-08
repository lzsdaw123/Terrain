using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject MissonUI,warnUI;
    [SerializeField]public static int MonsterLevel;
    [SerializeField] float MLtime = 0;
    int Level;
    public RectTransform DiffUI, DiffUI_s;

    void Awake()
    {
        Lv1 = false;
        MonsterLevel = 0;
    }
    void Start()
    {
        SpawnRay= GameObject.Find("SpawnRay");
        SpawnRay.SetActive(false);
        explode.SetActive(false);
        MissonUI.SetActive(false);
        warnUI.SetActive(false);
        warnUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 373, 0);
    }

    void Update()
    {
        if (!Lv1)
        {
            if (Input.GetKeyDown(KeyCode.F1) || start)
            {
                AudioManager.explode();
                Level_1_Start = true;
                explode.SetActive(true);
                Force.開始破門();
                SpawnRay.SetActive(true);
                Lv1 = true;
            }
        }
        if (Lv1)
        {
            time += Time.deltaTime;
            if (time >= 3)
            {
                PSexplode.SetActive(false);
            }
            if (time >= 6f)
            {
                MissonUI.SetActive(true);
                warnUI.SetActive(true);
                //AudioManager.Warn(0);
                var main = PSsmoke.main;
                main.loop = false;
            }
            if (time >= 15f)
            {
                warnUI.SetActive(false);
            }
            if (time >=25f)
            {
                explode.SetActive(false);
            }
            if (MonsterLevel != 5)
            {
                //MLtime += Time.deltaTime;
                MLtime += 10*Time.deltaTime;
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
}
