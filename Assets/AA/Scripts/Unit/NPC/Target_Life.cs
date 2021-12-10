using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Target_Life : MonoBehaviour
{
    public Animator ani;
    private Rigidbody rigid;
    private Collider cld;

    public float hpFull =100; // 血量上限
    public float hp; // 血量
    //public Image hpImage;

    public GameObject HitUI;  //命中UI
    bool Player;
    Color UIcolor;
    public float time;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        cld = GetComponent<Collider>();
        HitUI = GameObject.Find("HitUI").gameObject;
    }
    void Start()
    {
        hpFull = 100;
        hp = hpFull;  //補滿血量
        DifficultyUp();  //難度調整
        RefreshLifebar(); // 更新血條
        HitUI.SetActive(false);
    }

    void Update()
    {
        if (HitUI.activeSelf)  //命中UI
        {
            //HitUI.transform.localScale -= new Vector3(0.15f, 0.15f, 0f) * 10 * Time.deltaTime;
            //if (HitUI.transform.localScale.x <= 0)
            //{
            //    HitUI.SetActive(false);
            //    HitUI.transform.localScale = new Vector3(0f, 0f, 0f);
            //}
            HitUI.transform.localScale += new Vector3(0.15f, 0.15f, 0f) * 12 * Time.deltaTime;
            UIcolor = HitUI.GetComponent<Image>().color;
            if (UIcolor == Color.white)
            {
                if (HitUI.transform.localScale.x >= 0.8)
                {
                    HitUI.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                    HitUI.SetActive(false);
                }
            }
            else if(UIcolor== Color.red)
            {
                if (HitUI.transform.localScale.x >= 1.4)
                {
                    HitUI.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
                    HitUI.SetActive(false);
                }
            }
        }
        if (hp != hpFull)
        {
            time += Time.deltaTime;
            if (time >= 1)
            {
                time = 0;
                hp = hpFull;
            }
        }
    }
    public void Unit(bool player)
    {
        Player = player;
    }
    public void Damage(float Power)
    {
        hp -= Power; // 扣血
        if (hp >-10)
        {        
            if (Player)
            {
                //HitUI.SetActive(true);
                //HitUI.transform.localScale = new Vector3(0f, 0f, 1f);
                //HitUI.GetComponent<Image>().color = Color.white;
            }
        }
        if (hp <= 0)
        {
            //HitUI.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            //HitUI.GetComponent<Image>().color = Color.red;
            hp = -10; // 不要扣到負值         
            ani.SetTrigger("Die");           
        }
        RefreshLifebar(); // 更新血條
    }

    void RefreshLifebar() // 更新血條 UI
    {
        //hpImage.fillAmount = hp / hpFull; //顯示血球
    }
    void DifficultyUp()  //難度設定
    {
        hp = hpFull;  //補滿血量
    }
    void OnDisable()
    {
        Scoreboard.AddScore(true);  //怪物擊殺
        Shop.AddKillScore();  //怪物擊殺分數
        DifficultyUp();      
    }
}
