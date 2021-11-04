using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerResurrection : MonoBehaviour
{
    public Transform R1, R2;
    public GameObject Player;
    public GameObject DeadUI;
    public GameObject Black;
    public float time;
    public bool StartGame;
    public Settings Settings;
    bool Dead=true;
    float fullHp;
    void Awake()
    {
        DeadUI.SetActive(false);
        if (StartGame)
        {
            Player.SetActive(false);
            Player.transform.position = R1.position;
        }       
        Black.SetActive(false);     
    }

    void Start()
    {
        Player.SetActive(true);
        Settings = GameObject.Find("SettingsCanvas").GetComponent<Settings>();
        fullHp = HeroLife.fullHp;
    }

    void Update()
    {
        if (HeroLife.Dead)
        {
            if (Dead)
            {
                Dead = false;
                DeadUI.SetActive(true);
                pause();
            }
        }
        if (Black.activeSelf)
        {
            time += Time.deltaTime;          
        }
        if (time >= 1)   //復活時間
        {
            time =0;

            Black.SetActive(false);
            Player.SetActive(true);
            DeadUI.SetActive(false);
            Dead = true;
        }
    }
    public void Re()  //復活
    {
        AudioManager.Button();
        Black.SetActive(true);
        Player.SetActive(false);
        HeroLife.PlayerRe();  //血量回復
        Shooting.PlayerRe();  //彈藥回復
        Player.transform.position = R2.position;
        con();
    }
    public void SceneRe()  //重新關卡
    {
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
