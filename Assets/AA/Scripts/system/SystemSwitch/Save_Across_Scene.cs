using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Save_Across_Scene : MonoBehaviour
{
    public static GameObject Play;
    public static Shooting Shooting;
    public static GameObject Take;
    public static GameObject ObjectText;
    public static GameObject Am_zero_Warn;
    public static Camera Gun_Camera;
    public static GameObject Aim;
    public static GameObject HitUI;
    public static AudioManager AudioManager;

    void Awake()
    {
        Play = GameObject.Find("POPP").gameObject;
        Shooting = Play.GetComponent<Shooting>();
        Take = GameObject.Find("Take");
        ObjectText = GameObject.Find("ObjectText");
        Am_zero_Warn = GameObject.Find("Am_zero_Warn").gameObject;
        Gun_Camera = GameObject.Find("Gun_Camera").GetComponent<Camera>();
        Aim = GameObject.Find("Aim").gameObject;
        HitUI = GameObject.Find("HitUI").gameObject;
    }
    void Start()
    {
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();  //聲音控制器

    }

    void Update()
    {
        float SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        if (SceneNub == 1)
        {
            Destroy(gameObject);
        }
    }
}
