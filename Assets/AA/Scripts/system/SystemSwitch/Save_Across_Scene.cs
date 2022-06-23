using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Save_Across_Scene : MonoBehaviour
{
    public static GameObject Play;
    public static Shooting Shooting;
    public static HeroLife heroLife;
    public static GameObject Take;
    public static GameObject ObjectText;
    public static GameObject Am_zero_Warn;
    public static Camera Gun_Camera;
    public static GameObject Aim;
    public static GameObject HitUI;
    public static AudioManager AudioManager;

    public GameObject[] Boss2HpUI;
    public static GameObject[] ps_Boss2HpUI;
    public Image[] HP_W, HP_R;
    public static Image[] ps_HP_W, ps_HP_R;
    public static ObjectPool pool_Hit;


    void Awake()
    {
        Play = GameObject.Find("POPP").gameObject;
        Shooting = Play.GetComponent<Shooting>();
        heroLife = Play.GetComponent<HeroLife>();
        Take = GameObject.Find("Take");
        ObjectText = GameObject.Find("ObjectText");
        Am_zero_Warn = GameObject.Find("Am_zero_Warn").gameObject;
        Gun_Camera = GameObject.Find("Gun_Camera").GetComponent<Camera>();
        Aim = GameObject.Find("Aim").gameObject;
        HitUI = GameObject.Find("HitUI").gameObject;
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
    }
    void Start()
    {
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();  //聲音控制器
        ps_Boss2HpUI = Boss2HpUI;
        ps_HP_W = HP_W;
        ps_HP_R = HP_R;
        DontDestroyOnLoad(gameObject);  //切換場景時保留
    }

    void Update()
    {
        float SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        if (SceneNub == 1 || PlayerResurrection.ReDelete)
        {
            Destroy(gameObject);
        }

    }
}
