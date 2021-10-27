using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject SettingsUI, deSetUI;

    public AudioManager AudioManager;

    public GameObject PictureSetUI;

    public Image AsI, PsI;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);  //切換場景時保留
        SettingsUI.SetActive(false);  //設定介面
        deSetUI.SetActive(false);  //詳細設定介面

        PictureSetUI.SetActive(false);
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (deSetUI.activeSelf)
            {
                No();
            }
            else
            {
                if (!SettingsUI.activeSelf)
                {
                    SettingsUI.SetActive(true);
                    pause();
                }
                else
                {
                    SettingsUI.SetActive(false);
                    con();
                }
            }
        }
    }
    void pause()
    {
        Cursor.lockState = CursorLockMode.None; //游標無狀態模式
        //時間暫停
        Time.timeScale = 0f;
    }

    void con()
    {
        Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式
        //時間以正常速度運行
        Time.timeScale = 1f;
    }

    public void Back()  //繼續遊戲
    {
        SettingsUI.SetActive(false);
        con();
    }
    public void Set()
    {
        if (!deSetUI.activeSelf)
        {
            deSetUI.SetActive(true);
        }
        else
        {
            deSetUI.SetActive(false);
        }
    }
    public void Exit()
    {
        Debug.Log("離開遊戲");
    }
    public void Yes()
    {
        deSetUI.SetActive(false);
    }
    public void Re()
    {
        
    }
    public void No()
    {
        deSetUI.SetActive(false);
    }
    public void PictureSetButton()  //點開畫面設定
    {
        PictureSetUI.SetActive(true);
        AsI.color = new Color(0.643f, 0.643f, 0.643f, 1f);
        PsI.color = new Color(0.298f, 0.298f, 0.298f, 1f);
        AudioManager.AudioSourceUI.SetActive(false);
    }
}
