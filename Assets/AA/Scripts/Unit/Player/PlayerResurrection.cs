using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerResurrection : MonoBehaviour
{
    public Transform R1, R2;
    public Transform[] R_transform;
    public GameObject Player;
    public GameObject Gun;
    public GameObject DeadUI;  //死亡UI
    public GameObject FailUI;  //失敗UI
    public GameObject Black;
    public float time;
    public bool StartGame;
    public static bool Mission_L1;
    [SerializeField] private bool SF_Mission_L1;
    public bool RespawnPoint;
    public int RePointNub;
    public Transform[] RePoint;

    public Settings Settings;
    bool Dead;
    bool RePlay;
    public MouseLook mouseLook;
    public float StartTime;

    void Awake()
    {
        DeadUI.SetActive(false);
        Mission_L1 = SF_Mission_L1;
        //Gun = GameObject.Find("Gun").gameObject;
        if (StartGame)
        {
            Player.SetActive(false);
            Player.transform.position = R1.position;
            //Player.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            //Gun.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            //print("0");
        }
        if (Mission_L1)
        {
            Player.SetActive(false);
            Player.transform.position = R2.position;
            Player.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            Gun.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        Black.SetActive(false);
        Dead = true;
        RePlay = false;
        if (RespawnPoint)
        {
            Player.SetActive(false);
            Player.transform.position = RePoint[RePointNub].position;
        }
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式
        Player.SetActive(true);
        Settings = GameObject.Find("SettingsCanvas").GetComponent<Settings>();
        mouseLook = GameObject.Find("Gun_Camera").GetComponent<MouseLook>();
        //Player.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        Player.transform.rotation = R_transform[0].rotation;
        mouseLook.enabled = false;
        StartTime = 0;

    }

    void Update()
    {
        if (StartTime >= 0)
        {
            StartTime += Time.deltaTime;
            if (StartTime >= 1)
            {
                StartTime = -1;
                mouseLook.enabled = true;
            }
        }
        if (HeroLife.Dead)
        {
            if (Dead)
            {
                Dead = false;
                //DeadUI.SetActive(true);
                Player.GetComponent<PlayerMove>().enabled = false;
                Player.GetComponent<Shooting>().enabled = false;
                pause();
                Re();
            }
        }
        if (RePlay)
        {
            time += Time.deltaTime;          
        }
        if (time >= 1)   //復活時間
        {
            time =0;
            RePlay = false;
            Black.SetActive(false);
            Player.SetActive(true);
            Player.GetComponent<PlayerMove>().enabled = true;
            Player.GetComponent<Shooting>().enabled = true;
            Player.GetComponent<HeroLife>().enabled = true;
            //DeadUI.SetActive(false);
            Dead = true;         
        }
    }
    public void Re()  //復活
    {
        Scoreboard.ReScore(1);
        AudioManager.Button();
        Black.SetActive(true);
        Player.SetActive(false);
        RePlay = true;
        HeroLife.PlayerRe();  //血量回復
        Shooting.PlayerRe();  //彈藥回復
        Player.transform.position = R2.position;
        Player.transform.rotation = R2.localRotation;
        con();
    }
    public void SceneRe()  //重新關卡
    {
        FailUI.SetActive(false);
        Settings.LoadNewScene("SampleScene");
        //Black.SetActive(true);
        //SceneManager.LoadScene("SampleScene");
        Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式 
        Time.timeScale = 1f;
    }
    public void Exit()
    {
        Settings.Exit();
    }
    void pause()  //開啟設定介面
    {
        Cursor.lockState = CursorLockMode.None; //游標無狀態模式
        //時間暫停
        Time.timeScale = 0f;
    }

    void con()  //關閉設定介面
    {
        Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式
        //時間以正常速度運行
        Time.timeScale = 1f;
    }
}
