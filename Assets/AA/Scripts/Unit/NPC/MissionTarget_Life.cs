using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionTarget_Life : MonoBehaviour
{
    public float fullHp, hp, hp_R;  //滿血時數值, 實際, 紅血
    public Image hpImage, HP_R; //血球的UI物件
    public GameObject HP_O,warnUI, SeriousWarnUI;
    public static bool Dead;
    float time;
    public float UItime;
    public GameObject Exp,BigExp;  //爆炸,大爆炸
    //public GameObject FailUI;  //任務失敗UI
    float DeadTime;
    bool WarnT=true;
    bool Dialogue;

    void Awake()
    {
        //FailUI.SetActive(false);
        time = 0;
        DeadTime = 0;
    }
    void Start()
    {
        hp = fullHp= hp_R = 50; //遊戲一開始時先填滿血
        Dead = Dialogue = false;
        warnUI.SetActive(false);
        SeriousWarnUI.SetActive(false);
        UItime = 0;
        Exp.SetActive(false);
    }

    public void Damage(float Power) // 接受傷害
    {
        hp -= Power; // 扣血
        warnUI.SetActive(true);
        warnUI.gameObject.transform.GetChild(0).GetComponent<Animator>().SetInteger("Type", 0);
        warnUI.gameObject.transform.GetChild(0).GetComponent<Animator>().speed = 3f;
        //AudioManager.Warn(0);
        if (hp <= 0)  //第一關失敗
        {
            hp = 0; // 不要扣到負值
            //gameObject.SetActive(false);
            Destroyed();
        }      
    }
    void Update()
    {
        hpImage.fillAmount = hp / fullHp; //顯示血球
        HP_R.fillAmount = hp_R / fullHp; //顯示血球

        if (hp != hp_R)
        {
            time += 4 * Time.deltaTime;
            if (time >= 2)
            {
                time = 2;
                hp_R -= 1f * Time.deltaTime;
            }
        }
        if (hp_R <= hp)
        {
            hp_R = hp;
            time = 0;
        }
        if (Level_1.LevelA_ == 9)  //第一關勝利
        {
            SeriousWarnUI.SetActive(false);
        }
        if (!Dead)
        {
            if (hp <= fullHp * 0.12f)  //血量低於安全值
            {
                HP_O.SetActive(true);
                SeriousWarnUI.SetActive(true);
                SeriousWarnUI.gameObject.transform.GetChild(0).GetComponent<Animator>().SetInteger("Type", 1);
                if (WarnT)
                {
                    WarnT = false;
                    AudioManager.Warn(0);
                }
            }
            if (warnUI.activeSelf)
            {
                UItime += Time.deltaTime;
                if (UItime >= 2)
                {
                    warnUI.SetActive(false);
                    UItime = 0;
                }
            }
        }
        if (!SeriousWarnUI.activeSelf)
        {
            warnUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 8, 0);
        }
        else
        {
            warnUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, -78, 0);
        }
        if (Exp.activeSelf)
        {
            DeadTime += Time.deltaTime;
            AudioManager.Warn(-1);
        }
        if (DeadTime >= 3 && !Dialogue)
        {
            Dialogue = true;
            DialogueEditor.StartConversation(3, 2, 0, false, 0, true);
            BigExp.SetActive(false);
        }
        if (DeadTime >= 16)
        {
            Scoreboard.Settlement();
            PlayerResurrection.GameOverUI();
            //FailUI.SetActive(true);
            DeadTime = 10;
            Cursor.lockState = CursorLockMode.None; //游標無狀態模式
            Time.timeScale = 0f;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            //Destroyed();
        }
    }
    void Destroyed()
    {
        if (!Dead)
        {
            Dead = true;
            PlayerResurrection.Fail = true;
            //print("發電站已被摧毀");
            Exp.SetActive(true);
            AudioManager.explode();          
       
        }
    }
}
