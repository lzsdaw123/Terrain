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
    public static int RebirthPonitNub;
    public GameObject Player;  //取得玩家
    public GameObject Gun;  //玩家武器
    public GameObject DeadUI;  //死亡UI
    public GameObject FailUI;  //失敗UI
    public GameObject RebirthUI;  //重生UI
    public Text FailText;
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
    public static bool Fail;  //任務失敗
    [SerializeField] bool SF_Fail;  //任務失敗
    public GameObject[] ReObjects;
    public static bool ReO;
    public static bool GameRe;

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
                Player.transform.localRotation = RebirthPonit[0].localRotation;
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
            ReO = false;
            GameRe = false;
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
        Fail = false;
    }

    void Update()
    {
        if (ReO && SceneNub==3)
        {
            ReObjects[0] = GameObject.Find("BOSS 2").gameObject;
            ReObjects[1] = GameObject.Find("Boss2_").gameObject;
            ReObjects[2] = GameObject.Find("Lv2 Trigger").gameObject;
            ReObjects[3] = GameObject.Find("B2_測試用").gameObject;
            for (int i = 0; i < ReObjects.Length; i++)
            {
                ReObjects[i].SetActive(true);
            }
            ReO = false;
        }
        SF_Fail = Fail;
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
        if (HeroLife.Dead)  //玩家死亡
        {
            if (Dead)
            {
                Dead = false;
                //DeadUI.SetActive(true);
                Player.GetComponent<PlayerMove>().enabled = false;
                Player.GetComponent<Shooting>().enabled = false;
                pause();
                if (Fail)
                {
                    GameOverUI();
                }
                else
                {
                    Re();
                }
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
            Player.GetComponent<HeroLife>().closeDamageEffects();
            //DeadUI.SetActive(false);
            Dead = true;         
        }
        if (time >= 1.3f)  //復活結束
        {
            RebirthUI.SetActive(false);
            Shooting.Reload = false;
            Player.GetComponent<Shooting>().LayDown = false;
            Player.GetComponent<Shooting>().shooting = true;
            Player.GetComponent<PlayerMove>().m_Jump = false;
        }
        if (time >= 4.3f)  //無敵時間結束
        {
            time = 0;
            RePlay = false;
            Player.GetComponent<HeroLife>().無敵 = false;
        }
        if (GameOver)  //顯示遊戲失敗UI
        {
            SceneNub = Settings.SceneNub; //取得當前場景編號
            switch (SceneNub)
            {
                case 2:
                    FailText.text = "任務失敗\n 發電站已被破壞";
                    break;
                case 3:
                    FailText.text = "任務失敗\n 被黑球吞噬";
                    break;
            }
            Cursor.lockState = CursorLockMode.None; //游標無狀態模式
            FailUI.SetActive(true);
            GameOver = false;
        }
    }
    public static void PlayerBirth() //生成玩家
    {
        if (!GameOver)
        {
            PlayerBirthT = true;
        }
    }
    public void Birth()  //轉場誕生
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
                Player.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                HeroLife.HpLv = 1;
                break;
            case 3:  //第二關
                if (Boss02_AI.StartAttack  || RebirthPonitNub==2)
                {
                    Player.transform.position = RebirthPonit[2].position;
                    Player.transform.rotation = RebirthPonit[2].localRotation;
                }
                else
                {
                    Player.transform.position = RebirthPonit[1].position;
                    Player.transform.rotation = RebirthPonit[1].localRotation;
                }
                HeroLife.HpLv = 2;
                HeroLife.hp = HeroLife.fullHp = 20 * HeroLife.HpLv;
                break;
        }
        Player.SetActive(true);
        Player.GetComponent<PlayerMove>().enabled = true;
        Player.GetComponent<Shooting>().enabled = true;
        if (GameRe)
        {
            GameRe = false;
            HeroLife.GameRe();  //血量回復
            Shooting.PlayerRe();  //彈藥回復
        }
        con();
        Dead = true;
    }
    public void Re()  //復活
    {
        Scoreboard.ReScore(1);
        AudioManager.Button();
        RebirthUI.SetActive(true);  //死亡UI
        RePlay = true;
        HeroLife.LiftTime = 12;
        HeroLife.PlayerRe();  //血量回復
        Shooting.PlayerRe();  //彈藥回復
        Player.SetActive(false);
        if (StartGame || RespawnPoint || SF_Mission_L1)  //如果遊戲開始
        {
            SceneNub = Settings.SceneNub; //取得當前場景編號
            switch (SceneNub)
            {
                case 2:  //第一關
                    Player.transform.position = RebirthPonit[0].position;
                    Player.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    break;
                case 3:  //第二關
                    if (Boss02_AI.StartAttack || RebirthPonitNub==2)
                    {
                        Player.transform.position = RebirthPonit[2].position;
                        Player.transform.rotation = RebirthPonit[2].localRotation;
                    }
                    else
                    {
                        Player.transform.position = RebirthPonit[1].position;
                        Player.transform.rotation = RebirthPonit[1].localRotation;
                    }
                    break;
            }
        }
        else  //測試用重生
        {
            Player.transform.position = RebirthPonit[InputSceneNub].position;
            Player.transform.rotation = RebirthPonit[InputSceneNub].localRotation;           
        }
        Player.SetActive(true);
        Player.GetComponent<PlayerMove>().enabled = true;
        Player.GetComponent<Shooting>().enabled = true;
        con();
        Dead = true;
    }
    public static void GameOverUI()  //遊戲失敗
    {
        GameOver = true;
    }
    public void SceneRe()  //重新關卡
    {
        GameRe = true;
        FailUI.SetActive(false);
        Fail = false;
        Player.SetActive(false);
        SceneNub = Settings.SceneNub; //取得當前場景編號
        switch (SceneNub)
        {
            case 2:  //第一關
                RebirthPonitNub = 0;
                SceneManager.UnloadSceneAsync("SampleScene");
                Settings.LoadNewScene("SampleScene");
                break;
            case 3:  //第二關
                RebirthPonitNub = 2;

                SceneManager.UnloadSceneAsync("Scene_2");
                for (int i = 0; i < ReObjects.Length; i++)
                {
                    ReObjects[i].SetActive(false);
                }
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
