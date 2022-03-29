using System.Collections;
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

    public RawImage StartUI;  //遊戲開場UI
    public Texture2D[] Start_image;
    public bool START_bool;
    int SceneNub;  //當前場景編號
    int SceneCount;  //當前場景編號
    public bool EnterStart;  //起始場景切換開關

    public static float smoothSpeed;  //滑鼠速度
    public Slider mouse_Slider;  //滑鼠靈敏度
    public Text mouse_Nub;
    public static int Level;

    [SerializeField] GameObject Player;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);  //切換場景時保留
        SceneCount = SceneManager.sceneCount;
        SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號

        if (SceneCount <= 1)  //場景數為1
        {          
            if (SceneNub == 0)  //當前場景編號為0
            {
                if (EnterStart)
                {
                    SceneManager.LoadSceneAsync(1);
                }
                else
                {
                    SceneManager.LoadSceneAsync(2);
                }
                SceneManager.UnloadSceneAsync(0);
                print("進入關卡");
            }
        }
        //else  //場景數為2以上
        //{         
        //    if (SceneNub == 0)
        //    {
        //        SceneManager.UnloadSceneAsync(0);
        //        print(SceneNub + "+SceneCount");
        //    }
        //}

        SettingsUI.SetActive(false);  //設定介面
        deSetUI.SetActive(false);  //詳細設定介面
        START_bool = false;

        PictureSetUI.SetActive(false);
        
        instance = this;

        mouse_Slider.maxValue = 100;  //滑鼠最大靈敏度
        mouse_Slider.value = 13;  //滑鼠預設靈敏度


    }
    void Start()
    {
        ReStart();
    }
    void Update()
    {
        if (START_bool)
        {
            if (Input.GetKeyDown(KeyCode.Escape))  //按下Esc
            {
                if (deSetUI.activeSelf)
                {
                    No();
                }
                else
                {
                    if (!SettingsUI.activeSelf)
                    {
                        //print("暫停");
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
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    Cursor.lockState = CursorLockMode.None; //游標無狀態模式
        //    Settings.LoadScene("GamePlayScene");
        //}
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    Cursor.lockState = CursorLockMode.None; //游標無狀態模式
        //    Settings.LoadScene("Start");
        //}
        smoothSpeed = mouse_Slider.value;
        float ScrN = mouse_Slider.value / mouse_Slider.maxValue *100;
        int _Nub = (int)ScrN;
        mouse_Nub.text = _Nub + " %";
    }
    public static void pause()  //開啟設定介面
    {
        Cursor.lockState = CursorLockMode.None; //游標無狀態模式
        //時間暫停
        Time.timeScale = 0f;
    }

    public static void con()  //關閉設定介面
    {
        Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式
        //時間以正常速度運行
        Time.timeScale = 1f;
    }

    public void Back()  //繼續遊戲
    {
        ButtonAudio();
        SettingsUI.SetActive(false);
        con();
    }
    public void Set()  //設定
    {
        ButtonAudio();
    }
    void deSetUI_TF()  //設定介面開關
    {
        ButtonAudio();
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
        ButtonAudio();
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
        ButtonAudio();
    }
    public void No()   //設定取消
    {
        deSetUI_TF();
    }
    public void PictureSetButton()  //點開畫面設定
    {
        ButtonAudio();
        PictureSetUI.SetActive(true);
        AsI.color = new Color(0.643f, 0.643f, 0.643f, 1f);
        PsI.color = new Color(0.298f, 0.298f, 0.298f, 1f);
        AudioManager.AudioSourceUI.SetActive(false);
    }


    public void StartB()  //進入遊戲
    {
        ButtonAudio();
        START_bool = true;
        Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式        
        LoadNewScene("SampleScene");
        //SceneManager.LoadSceneAsync(2);
        SceneManager.UnloadSceneAsync(1);
        //Settings.LoadScene("SampleScene");
    }
    public void OptionB()
    {
        deSetUI_TF();
    }
    public void QuitB()  //離開遊戲
    {
        ButtonAudio();
        Application.Quit();
    }
    //非同步載入新場景
    public void LoadNewScene(string sceneName)
    {
        //儲存需要載入的目標場景
        Globe.nextSceneName = sceneName;

        SceneManager.LoadSceneAsync("Messenger");
    }

    public void ScreenSwitch(Dropdown dropdown)  //畫面設定介面
    {
        ButtonAudio();
        if (dropdown.value == 0)
        {
            Screen.fullScreen = true; //切換為全螢幕模式

        }
        else
        {
            Screen.fullScreen = false; //切換為視窗化模式
            //切換到 640 x 480 全屏
            //Screen.SetResolution(640, 480, true);
        }
    }
    public void SceneLevel(Dropdown dropdown)  //遊戲難度
    {
        ButtonAudio();
        Level = dropdown.value;
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
        activeScene = SceneManager.GetActiveScene();  //當前活動場景
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
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            START_bool = true;
            Player = GameObject.Find("POPP").gameObject;
            Player.GetComponent<PlayerMove>().enabled = true;
            Player.GetComponent<Shooting>().enabled = true;
            Player.GetComponent<HeroLife>().enabled = true;
        }
        SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        if (SceneNub == 1)
        {
            StartUI = GameObject.Find("StartUI").GetComponent<RawImage>();
            StartButton = GameObject.Find("StartB").GetComponent<Button>();
            OptionButton = GameObject.Find("OptionB").GetComponent<Button>();
            QuitButton = GameObject.Find("QuitB").GetComponent<Button>();
            StartButton.onClick.AddListener(StartB);
            OptionButton.onClick.AddListener(OptionB);
            QuitButton.onClick.AddListener(QuitB);
        }
        AudioManager.SettingsCanvas = this;
    }
    void ButtonAudio()
    {
        AudioManager.Button();
    }
}
