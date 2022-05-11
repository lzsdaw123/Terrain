using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerResurrection : MonoBehaviour
{
    public int SceneNub;
    public int InputSceneNub;  //測試用重生
    public bool Sw_Scene;
    public Transform R1;  //第一關起始點
    public Transform R2;  //第二關起始點
    public Transform[] RebirthPonit;  //玩家重生點
    public GameObject Player;  //取得玩家
    public GameObject Gun;  //玩家武器
    public GameObject DeadUI;  //死亡UI
    public GameObject FailUI;  //失敗UI
    public GameObject RebirthUI;  //重生UI
    public float time;
    public bool StartGame;
    public static bool Mission_L1;
    [SerializeField] private bool SF_Mission_L1;
    public bool RespawnPoint;  //是否於重生點重生
    public int RePointNub;
    public Transform[] RePoint;  //重生點

    public Settings Settings;
    bool Dead;
    bool RePlay;
    public MouseLook mouseLook;
    public float StartTime;
    public static bool GameOver;
    public static bool PlayerBirthT;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);  //切換場景時保留
        DeadUI.SetActive(false);
        FailUI.SetActive(false);
        RebirthUI.SetActive(false);
        Mission_L1 = SF_Mission_L1;
        //Gun = GameObject.Find("Gun").gameObject;
        SceneNub = Settings.SceneNub; //取得當前場景編號
        if(SceneNub == 2 || SceneNub == 3)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {

        Settings = GameObject.Find("SettingsCanvas").GetComponent<Settings>();
        SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        if (SceneNub == 2 || SceneNub == 3)
        {
            Player = GameObject.Find("POPP").gameObject;
            Gun = Player.transform.GetChild(2).gameObject;
            if (StartGame)
            {
                Player.SetActive(false);
                Player.transform.position = R1.position;
                Player.transform.localRotation = R1.localRotation;
                //Gun.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            }
            if (Mission_L1)
            {
                Player.SetActive(false);
                Player.transform.position = RebirthPonit[0].position;
                Player.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                Gun.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            if (RespawnPoint)
            {
                Player.SetActive(false);
                Player.transform.position = RePoint[RePointNub].position;
            }

            RebirthUI.SetActive(false);
            Dead = true;
            RePlay = false;
        }
        //Cursor.lockState = CursorLockMode.Locked; //游標鎖定模式
        if (Player == null)
        {
            Player = GameObject.Find("POPP").gameObject;
            Gun = Player.transform.GetChild(2).gameObject;
            Start();
            return;
        }
        Player.SetActive(true);
        Gun.transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("LayDown", true);
        mouseLook = GameObject.Find("Gun_Camera").GetComponent<MouseLook>();
        //Player.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        Player.transform.rotation = RebirthPonit[0].rotation;
        mouseLook.enabled = false;
        StartTime = 0;
        GameOver = false;
        Sw_Scene = true;
        PlayerBirthT = false;
    }

    void Update()
    {
        if (PlayerBirthT)
        {
            PlayerBirthT = false;
            Birth();
        }
        //SceneNub = Settings.SceneNub; //取得當前場景編號)
        //if (SceneNub == 3 && Sw_Scene)
        //{
        //    Player.SetActive(false);
        //    Sw_Scene = false;
        //    Player.transform.position = R2.position;
        //    Player.transform.localRotation = R2.localRotation;
        //    Player.SetActive(true);
        //}
        if (StartTime >= 0)
        {
            StartTime += Time.deltaTime;
            Player.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

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
            Player.SetActive(true);
            Player.GetComponent<PlayerMove>().enabled = true;
            Player.GetComponent<Shooting>().enabled = true;
            Player.GetComponent<HeroLife>().enabled = true;
            Player.GetComponent<HeroLife>().無敵 = true;
            //DeadUI.SetActive(false);
            Dead = true;         
        }
        if (time >= 1.3f)  //復活結束
        {
            RebirthUI.SetActive(false);
        }
        if (time >= 4.3f)  //無敵時間結束
        {
            time = 0;
            RePlay = false;
            Player.GetComponent<HeroLife>().無敵 = false;
        }
        if (GameOver)  //顯示遊戲失敗UI
        {
            GameOver = false;
            FailUI.SetActive(true);
        }
    }
    public static void PlayerBirth() //生成玩家
    {
        PlayerBirthT = true;
    }
    public void Birth()
    {
        AudioManager.Button();
        Player.SetActive(false);
        //HeroLife.PlayerRe();  //血量回復
        //Shooting.PlayerRe();  //彈藥回復
        SceneNub = Settings.SceneNub; //取得當前場景編號
        switch (SceneNub)
        {
            case 2:  //第一關
                Player.transform.position = RebirthPonit[0].position;
                Player.transform.rotation = RebirthPonit[0].localRotation;
                HeroLife.HpLv = 1;
                break;
            case 3:  //第二關
                Player.transform.position = RebirthPonit[1].position;
                Player.transform.rotation = RebirthPonit[1].localRotation;
                HeroLife.HpLv = 2;
                HeroLife.hp = HeroLife.fullHp = 20 * HeroLife.HpLv;
                break;
        }
        Player.SetActive(true);
        con();
    }
    public void Re()  //復活
    {
        Scoreboard.ReScore(1);
        AudioManager.Button();
        RebirthUI.SetActive(true);
        Player.SetActive(false);
        RePlay = true;
        HeroLife.PlayerRe();  //血量回復
        Shooting.PlayerRe();  //彈藥回復
        if (StartGame)  //如果遊戲開始
        {
            SceneNub = Settings.SceneNub; //取得當前場景編號
            switch (SceneNub)
            {
                case 2:  //第一關
                    Player.transform.position = RebirthPonit[0].position;
                    Player.transform.rotation = RebirthPonit[0].localRotation;
                    break;
                case 3:  //第二關
                    Player.transform.position = RebirthPonit[1].position;
                    Player.transform.rotation = RebirthPonit[1].localRotation;
                    break;
            }
        }
        else  //測試用重生
        {
            Player.transform.position = RebirthPonit[InputSceneNub].position;
            Player.transform.rotation = RebirthPonit[InputSceneNub].localRotation;           
        }
        Player.SetActive(true);
        con();
    }
    public static void GameOverUI()  //遊戲失敗
    {
        GameOver = true;
    }
    public void SceneRe()  //重新關卡
    {
        FailUI.SetActive(false);
        SceneNub = Settings.SceneNub; //取得當前場景編號
        switch (SceneNub)
        {
            case 2:  //第一關
                SceneManager.UnloadSceneAsync("SampleScene");
                Settings.LoadNewScene("SampleScene");
                break;
            case 3:  //第二關
                SceneManager.UnloadSceneAsync("Scene_2");
                Settings.LoadNewScene("Scene_2");
                break;
        }
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
