﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class Settings : MonoBehaviour
{
    static Settings instance;
    Scene activeScene;
    public static UnityAction onScenesLoadingEvent;
    public static UnityAction onScenesLoadedEvent;
    private AsyncOperation operation;

    public Button StartButton;
    public Button OptionButton;
    public Button QuitButton;

    public GameObject SettingsUI, deSetUI;

    public AudioManager AudioManager;

    public GameObject PictureSetUI;

    public Image AsI, PsI;

    public RawImage StartUI;
    public Texture2D[] Start_image;
    public bool START_bool;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);  //切換場景時保留
        SceneManager.LoadScene(1);
        SceneManager.UnloadSceneAsync(0);
        SettingsUI.SetActive(false);  //設定介面
        deSetUI.SetActive(false);  //詳細設定介面
        

        PictureSetUI.SetActive(false);
        START_bool = false;
        instance = this;
    }
    void Start()
    {
        ReStart();

    }
    void Update()
    {
        if (START_bool)
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
        else
        {
            if (operation != null)
            {
                if (operation.progress == 1)
                {
                    operation = null;
                    ReStart();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Cursor.lockState = CursorLockMode.None; //游標無狀態模式
            Settings.LoadScene("GamePlayScene");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Cursor.lockState = CursorLockMode.None; //游標無狀態模式
            Settings.LoadScene("Start");
        }
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

    public void Back()  //繼續遊戲
    {
        SettingsUI.SetActive(false);
        con();
    }
    public void Set()  //設定
    {
        deSetUI_TF();
    }
    void deSetUI_TF()  //設定介面開關
    {
        if (!deSetUI.activeSelf)
        {
            deSetUI.SetActive(true);
        }
        else
        {
            if (StartUI !=null)
            {
                StartUI.GetComponent<RawImage>().texture = Start_image[0];
            }          
            deSetUI.SetActive(false);
        }
    }
    public void Exit()  //回到標題
    {
        START_bool = false;
        SettingsUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None; //游標無狀態模式    
        operation = SceneManager.LoadSceneAsync(1);
        SceneManager.UnloadSceneAsync(2);
        //Settings.LoadScene("Start");
    }
    public void Yes()  //設定確定
    {
        deSetUI_TF();

    }
    public void Re()
    {
        
    }
    public void No()   //設定取消
    {
        deSetUI_TF();
    }
    public void PictureSetButton()  //點開畫面設定
    {
        PictureSetUI.SetActive(true);
        AsI.color = new Color(0.643f, 0.643f, 0.643f, 1f);
        PsI.color = new Color(0.298f, 0.298f, 0.298f, 1f);
        AudioManager.AudioSourceUI.SetActive(false);
    }


    public void StartB()  //進入遊戲
    {
        StartUI.GetComponent<RawImage>().texture = Start_image[1];
        START_bool = true;
        Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式        
        SceneManager.LoadSceneAsync(2);
        SceneManager.UnloadSceneAsync(1);
        //Settings.LoadScene("SampleScene");
    }
    public void OptionB()
    {
        //StartUI.GetComponent<RawImage>().texture = Start_image[2];
        Set();
    }
    public void QuitB()  //離開遊戲
    {
        StartUI.GetComponent<RawImage>().texture = Start_image[3];
        Application.Quit();
    }

    public static void LoadScene(string sceneName)
    {
        instance.LoadingScene(sceneName);
    }
    public void LoadingScene(string sceneName)
    {
        StartCoroutine(LoadingRoutine(sceneName));
    }
    IEnumerator LoadingRoutine(string sceneName)
    {
        SceneManager.LoadScene("Messenger", LoadSceneMode.Additive);

        yield return null;

        onScenesLoadingEvent?.Invoke();
        activeScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(activeScene);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        yield return null;

        onScenesLoadedEvent?.Invoke();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        SceneManager.UnloadSceneAsync("Messenger");
        ReStart();
    }
    void ReStart()
    {
        StartUI = GameObject.Find("StartUI").GetComponent<RawImage>();
        StartButton = GameObject.Find("StartB").GetComponent<Button>();
        OptionButton = GameObject.Find("OptionB").GetComponent<Button>();
        QuitButton = GameObject.Find("QuitB").GetComponent<Button>();
        StartButton.onClick.AddListener(StartB);
        OptionButton.onClick.AddListener(OptionB);
        QuitButton.onClick.AddListener(QuitB);
        AudioManager.SettingsCanvas = this;
    }
}
