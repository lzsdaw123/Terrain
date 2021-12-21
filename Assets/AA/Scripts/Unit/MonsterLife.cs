using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterLife : MonoBehaviour
{
    [SerializeField] Transform root;

    public Animator ani;
    private Rigidbody rigid;
    private Collider cld;

    public int MonsterType;  //怪物類型 0=蠍子 / 1= 螃蟹
    public float[] hpFull = new float[] { 14, 20 }; // 血量上限
    public float hp; // 血量
    int HpLv;  //生命等級
    int Level;  //難度等級
    //public Image hpImage;

    private NavMeshAgent agent;
    public MonsterAI02 monster02;
    public MonsterAI03 monster03;

    public GameObject PS_Dead;
    [SerializeField] float DeadTime;

    public GameObject HitUI;  //命中UI
    float HitUITime;
    bool Player;
    Color UIcolor;
    bool Dead;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        cld = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        monster02 = GetComponent<MonsterAI02>();
        monster03 = GetComponent<MonsterAI03>();
        HitUI = GameObject.Find("HitUI").gameObject;
    }
    void Start()
    {
        PS_Dead.SetActive(false);
        hpFull = new float[] { 14, 20 };
        hp = hpFull[MonsterType];  //補滿血量
        DeadTime = 0;
        DifficultyUp();  //難度調整
        RefreshLifebar(); // 更新血條
        HitUI.SetActive(false);
        HitUITime = 0;
        Dead = false;
        //ani = GetComponent<Animator>();

        //RagdollActive(false); // 先關閉物理娃娃
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Y)) // 測試用
        //{
        //    monster02.enabled = false; // 關閉控制腳本
        //    agent.enabled = false;  //立即關閉尋徑功能
        //    ani.SetTrigger("Die");
        //}
        if (PS_Dead.activeSelf)
        {
            DeadTime += 1.6f * Time.deltaTime;
            if (DeadTime >= 1)
            {
                GameObject.Find("ObjectPool").GetComponent<ObjectPool>().RecoveryMonster01(gameObject);
            }
        }
    }
    // 開啟或關閉物理娃娃系統
    void RagdollActive(bool active)
    {
        ani.enabled = !active; // 關閉或開啟角色的 Animator
        rigid.isKinematic = active; // 關閉或開啟角色原本的 rigidbody
        //cld.enabled = !active; // 關閉或開啟角色原本的(膠囊)Collider

        // 搜尋骨架以下所有的 Rigidbody 關閉或開啟
        Rigidbody[] rigs = root.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].isKinematic = !active;
        }

        // 搜尋骨架以下所有的 Collider 關閉或開啟
        Collider[] clds = root.GetComponentsInChildren<Collider>();
        for (int i = 0; i < clds.Length; i++)
        {
            clds[i].enabled = active;
        }
    }

    // 開啟角色物理娃娃
    public void SetDeathRagdoll()
    {
        RagdollActive(true);
    }
    public void Unit(bool player)
    {
        Player = player;
    }
    public void Damage(float Power)
    {
        //print(Power);
        hp -= Power; // 扣血
        if (hp >0)
        {        
            if (Player)
            {
                HitUI.SetActive(true);
                HitUI.GetComponent<Image>().color = Color.white;
            }
        }
        if (hp <= 0)
        {
            if (!Dead)
            {
                HitUI.SetActive(true);
                HitUI.GetComponent<Image>().color = Color.red;
                Dead = true;
            }           
            hp = 0; // 不要扣到負值
            PS_Dead.SetActive(true);
            switch (MonsterType)  //關閉怪物AI 腳本
            {
                case 0:
                    monster02.enabled = false; 
                    break;
                case 1:
                    monster03.enabled = false;
                    break;
            }           
            agent.enabled = false; // 立即關閉尋徑功能
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
        HpLv = Level_1.MonsterLevel;
        Level = Settings.Level;
        Level = Level +1;
        if (HpLv > 0)
        {
            hpFull[MonsterType] = 7 + (HpLv * Level);
            if (hpFull[MonsterType] >= 7 + (5 * Level))
            {
                hpFull[MonsterType] = 7 + (5 * Level);
            }
        }
        //print("怪物血量:" + hpFull);  //最終血量 12 / 17 / 22 
        hpFull = new float[] { 14, 20 };
        hp = hpFull[MonsterType];  //補滿血量
    }
    void OnDisable()
    {
        Scoreboard.AddScore(true);  //怪物擊殺
        Shop.AddKillScore();  //怪物擊殺分數
        DifficultyUp();      
        PS_Dead.SetActive(false);
        DeadTime = 0;
        switch (MonsterType)  //開啟怪物AI 腳本
        {
            case 0:
                monster02.enabled = true;
                break;
            case 1: 
                monster03.enabled = true;
                break;
        }
        agent.enabled = true; // 開啟尋徑功能
    }
}
