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
    public AnimEvents AnimEvents;

    public int MonsterType;  //怪物類型 0=蠍子 / 1= 螃蟹 / 2=雞 / 3=標靶
    public static int PS_MonsterType;  //怪物類型 0=蠍子 / 1= 螃蟹
    public float[] hpFull=new float[4]; // 血量上限
    public float hp; // 血量
    public bool 無敵=false;
    int HpLv;  //生命等級
    int Level;  //難度等級
    //public Image hpImage;

    private NavMeshAgent agent;
    public MonsterAI02 monster02;
    public MonsterAI03 monster03;
    public MonsterAI04 monster04;

    public GameObject PS_Dead;
    public GameObject Model;  //模型
    public GameObject[] c_Model;  //模型
    [SerializeField] float DeadTime;

    public GameObject HitUI;  //命中UI
    float HitUITime;
    bool Player;
    Color UIcolor;
    bool Dead;
    public SkinnedMeshRenderer SMeshR;
    public Material[] materials;
    public bool MonsterDead;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        cld = GetComponent<Collider>();
        if (MonsterType != 3)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        monster02 = GetComponent<MonsterAI02>();
        monster03 = GetComponent<MonsterAI03>();
    }
    void Start()
    {
        HitUI = Save_Across_Scene.HitUI;
        if(PS_Dead !=null) PS_Dead.SetActive(false);
        DeadTime = 0;
        DifficultyUp();  //難度調整 設定血量
        RefreshLifebar(); // 更新血條
        HitUI.SetActive(false);
        HitUITime = 0;
        Dead = MonsterDead = false;
        //ani = GetComponent<Animator>();
        PS_MonsterType = MonsterType;
        //RagdollActive(false); // 先關閉物理娃娃
        switch (MonsterType)
        {
            case 0:
                SMeshR.material = materials[0];
                break;
            case 1:
                monster03.ani.SetInteger("Level", 0);
                break;
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Y)) // 測試用
        //{
        //    monster02.enabled = false; // 關閉控制腳本
        //    agent.enabled = false;  //立即關閉尋徑功能
        //    ani.SetTrigger("Die");
        //}
        if (Level_1.MonsterDead)  //自爆
        {
            if (!MonsterDead)
            {
                MonsterDead = true;
                Player = false;
                Damage(hpFull[MonsterType]);
            }
        }
        if (PS_Dead != null)
        {
            if (PS_Dead.activeSelf)
            {
                DeadTime += 1.4f * Time.deltaTime;
                if (DeadTime >= 1)
                {
                    if (agent != null) agent.enabled = false; // 立即關閉尋徑功能

                    switch (MonsterType)
                    {
                        case 0:
                            GameObject.Find("ObjectPool").GetComponent<ObjectPool>().RecoveryMonster01(gameObject);
                            break;
                        case 1:
                            GameObject.Find("ObjectPool").GetComponent<ObjectPool>().RecoveryMonster02(gameObject);
                            break;
                        case 2:
                            GameObject.Find("ObjectPool").GetComponent<ObjectPool>().RecoveryMonster03(gameObject);
                            break;
                    }
                }
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
    public void Unit(bool player)  //攻擊者是否為玩家
    {
        Player = player;
    }
    public void Damage(float Power)
    {
        //print(Power);
        hp -= Power; // 扣血
        if (MonsterType == 3)
        {
            ObjectPool.harm(Power);
            hp = hpFull[MonsterType];  //補滿血量
        }
        else
        {
            transform.position += new Vector3(0, 0, 0.1f);  //擊退效果
        }
        if (無敵) hp = hpFull[MonsterType];  //補滿血量
        if (hp >0)
        {        
            if (Player)
            {
                HitUI.SetActive(true);
                HitUI.GetComponent<Image>().color = Color.white;
            }
        }
        if (hp <= hpFull[MonsterType] /2)  //怪物血量低於一半
        {
            switch (MonsterType)
            {
                case 0:
                    SMeshR.material = materials[1];
                    monster02.AngeStater();
                    break;
                case 1:
                    monster03.ani.SetInteger("Level", 1);
                    break;
                case 2:
                    //monster04.enabled = false;
                    break;
            }            
        }
        if (hp <= 0)
        {
            if (!Dead)
            {
                if (Player)
                {
                    HitUI.SetActive(true);
                    HitUI.GetComponent<Image>().color = Color.red;
                    //AudioManager.Hit(4);  //玩家擊殺音效
                }
                Dead = true;
                switch (MonsterType)  //關閉怪物AI 腳本
                {
                    case 0:
                    case 1:
                        AnimEvents.MonsterAudio(2);  //怪物爆汁音效
                        break;
                }
            }           
            hp = 0; // 不要扣到負值
            if (PS_Dead != null)  PS_Dead.SetActive(true);  //死亡爆炸
            if (Model !=null) Model.SetActive(false);
            for(int i=0; i< c_Model.Length; i++)
            {
                c_Model[i].SetActive(false);
            }
            switch (MonsterType)  //關閉怪物AI 腳本
            {
                case 0:
                    monster02.enabled = false;
                    break;
                case 1:
                    monster03.enabled = false;
                    break;
                case 2:
                    monster04.enabled = false;
                    break;
            }           
            //ani.SetTrigger("Die");           
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

        hpFull = new float[] { 12, 20f, 3, 999 };  // 血量上限
        hp = hpFull[MonsterType];  //補滿血量
    }
    void OnDisable()
    {
        Scoreboard.AddScore(true);  //怪物擊殺
        Shop.AddKillScore();  //怪物擊殺分數
        DifficultyUp();
        if (PS_Dead != null)  PS_Dead.SetActive(false);
        DeadTime = 0;
        switch (MonsterType)  //開啟怪物AI 腳本
        {
            case 0:
                monster02.enabled = true;
                monster02.ani.SetBool("Attack", true);
                SMeshR.material = materials[0];
                break;
            case 1: 
                monster03.enabled = true;
                monster03.ani.SetBool("Attack", true);
                monster03.ani.SetInteger("Level", 0);
                break;
            case 2:
                monster04.enabled = true;
                //monster04.ani.SetBool("Attack", false);
                break;
        }
        if(agent !=null) agent.enabled = true; // 開啟尋徑功能
        if (Model != null) Model.SetActive(true);
        for (int i = 0; i < c_Model.Length; i++)
        {
            c_Model[i].SetActive(true);
        }
    }
}
