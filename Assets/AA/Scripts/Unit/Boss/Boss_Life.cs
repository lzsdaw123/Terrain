using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss_Life : MonoBehaviour
{
    [SerializeField] Transform root;

    public Animator ani;
    public Animator CrystalAni;
    private Rigidbody rigid;
    private Collider cld;
    public AnimEvents AnimEvents;
    public bool isBoss;
    public int MonsterType;  //Boss類型 0=水晶 / 1= 機械
    public static int PS_MonsterType;  //Boss類型 0=水晶 / 1= 機械
    public float[] hpFull; // 血量上限
    public float hp; // 血量
    public bool 無敵=false;
    int HpLv;  //生命等級
    int Level;  //難度等級
    //public Image hpImage;

    private NavMeshAgent agent;
    public Boss01_AI boss01_AI;
    public Boss02_AI boss02_AI;
    public Level_1 level_1;
    //public Level_2 level_2;

    public GameObject PS_Dead;  //死亡特效
    public GameObject Model;  //模型
    [SerializeField] float DeadTime;

    public GameObject HitUI;  //命中UI
    float HitUITime;
    bool Player;
    Color UIcolor;
    bool Dead;
    public GameObject HitObject;
    public GameObject[] Crystal_Weakness;  //水晶弱點
    public GameObject[] WeaknessObject;
    public GameObject[] WeaknessObjectPS;
    public float[] Weakness_Hp;  //弱點血量
    bool YesStart;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        cld = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        HitUI = Save_Across_Scene.HitUI;
        if (PS_Dead!=null) PS_Dead.SetActive(false);
        //hpFull = new float[] { 100, 24 };  // 血量上限
        //hp = hpFull[MonsterType];  //補滿血量
        DeadTime = 0;
        DifficultyUp();  //難度調整
        RefreshLifebar(); // 更新血條
        HitUITime = 0;
        Dead = false;
        //ani = GetComponent<Animator>();
        PS_MonsterType = MonsterType;
        //RagdollActive(false); // 先關閉物理娃娃
        gameObject.layer = LayerMask.NameToLayer("Monster");
        gameObject.tag = "Enemy";
        Weakness_Hp = new float[] { 25, 25, 25, 25,100 };
        boss02_AI.RangeObject[0].SetActive(true);
        for(int i=0; i< Crystal_Weakness.Length; i++)
        {
            Crystal_Weakness[i].GetComponent<Crystal_Life>().無敵 = true;
            YesStart = false;
        }
    }

    void Update()
    {
        if (Boss02_AI.StartAttack && !YesStart)
        {
            YesStart = true;
            Crystal_Weakness[0].GetComponent<Crystal_Life>().無敵 = false;
            PlayerView.Crystal_Weakness = 0;
        }
        //if (Input.GetKeyDown(KeyCode.Y)) // 測試用
        //{
        //    monster02.enabled = false; // 關閉控制腳本
        //    agent.enabled = false;  //立即關閉尋徑功能
        //    ani.SetTrigger("Die");
        //}
        if (PS_Dead!=null)
        {
            if (PS_Dead.activeSelf)
            {
                DeadTime += 1.4f * Time.deltaTime;
                if (DeadTime >= 1)
                {
                    agent.enabled = false; // 立即關閉尋徑功能
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
    public void Weakness(GameObject gameObject)  //哪個弱點
    {
        HitObject = gameObject;
    }
    public void Damage(float Power)  //-----受到傷害----//
    {
        if (hp > 0)
        {
            if (Player)
            {
                HitUI.SetActive(true);
                HitUI.GetComponent<Image>().color = Color.white;
            }
        }
        //print(Power);
        switch (MonsterType)
        {
            case 0:  //水晶Boss
                hp -= Power; // 扣血
                if (hp <= hpFull[MonsterType] / 2)  //怪物血量低於一半
                {

                }
                break;
            case 1:  //機械Boss
                for (int i = 0; i < WeaknessObject.Length; i++)
                {
                    if (WeaknessObject[i] == HitObject)
                    {
                        Weakness_Hp[i] -= Power;
                        hp -= Power;
                    }
                    if (Weakness_Hp[i] <= 0)
                    {
                        WeaknessObject[i].GetComponent<SphereCollider>().enabled = false;
                        HitUI.SetActive(true);
                        HitUI.GetComponent<Image>().color = Color.red;  //紅色擊殺UI
                    }
                }
                if (Weakness_Hp[0] <= 0)  //左手臂弱點
                {
                    Weakness_Hp[0] = 60;
                    WeaknessObjectPS[0].SetActive(true);
                    boss02_AI.Level = 2;
                    boss02_AI.ani.SetInteger("Level", 2);
                    boss02_AI.ani.SetTrigger("LevelUp");
                    Crystal_Weakness[1].GetComponent<Crystal_Life>().無敵 = false;
                    Crystal_Weakness[2].GetComponent<Crystal_Life>().無敵 = false;
                    PlayerView.Crystal_Weakness = 1;
                }
                if(Weakness_Hp[1] <= 0)  //腹部上弱點擊破
                {
                    WeaknessObjectPS[1].SetActive(true);
                    Weakness_Hp[1] = 0;
                    PlayerView.Crystal_Weakness = 2;
                }
                if (Weakness_Hp[2] <= 0)  //腹部下弱點擊破
                {
                    WeaknessObjectPS[2].SetActive(true);
                    Weakness_Hp[2] = 0;
                    PlayerView.Crystal_Weakness = 3;

                }
                if (Weakness_Hp[1] == 0 && Weakness_Hp[2] == 0)  //腹部兩個弱點
                {
                    Weakness_Hp[1] = 60;
                    Weakness_Hp[2] = 60;
                    boss02_AI.Level = 3;
                    boss02_AI.ani.SetInteger("Level", 3);
                    boss02_AI.ani.SetTrigger("LevelUp");
                    boss02_AI.RangeObject[0].SetActive(false);
                    boss02_AI.RangeObject[1].SetActive(true);
                    Crystal_Weakness[3].GetComponent<Crystal_Life>().無敵 = false;
                    Boss02_AI.BulletType = 2;
                    PlayerView.Crystal_Weakness = 4;
                }
                if (Weakness_Hp[3] <= 0)  //左胸弱點擊破
                {
                    Weakness_Hp[3] = 60;
                    Weakness_Hp[4] = 200;
                    WeaknessObjectPS[3].SetActive(true);
                    boss02_AI.Level = 4;
                    boss02_AI.ani.SetInteger("Level", 4);
                    boss02_AI.ani.SetTrigger("LevelUp");
                    boss02_AI.ani.SetBool("Attack1", false);  //第一階攻擊模式
                    boss02_AI.ani.SetBool("Attack2", false);  //第二階攻擊模式
                    PlayerView.Crystal_Weakness = 5;
                }
                if (Weakness_Hp[4] <= 0)  //頭部弱點擊破
                {
                    Weakness_Hp[4] = 600;
                    boss02_AI.ani.SetTrigger("Dead");
                    boss02_AI.ani.SetBool("Attack1", false);  //第一階攻擊模式
                    boss02_AI.ani.SetBool("Attack2", false);  //第二階攻擊模式
                    PlayerView.Crystal_Weakness = 6;
                }
                break;
        }
        if (無敵) hp = hpFull[MonsterType];  //補滿血量
        if (hp <= 0)  //死亡
        {
            if (!Dead)
            {
                if (Player)
                {
                    HitUI.SetActive(true);
                    HitUI.GetComponent<Image>().color = Color.red;  //紅色擊殺UI
                    //AudioManager.Hit(4);  //玩家擊殺音效
                }
                Dead = true;
                switch (MonsterType)
                {
                    case 0:
                        //AnimEvents.MonsterAudio(2);  //怪物爆汁音效
                        if (CrystalAni != null) CrystalAni.SetTrigger("Dead");
                        gameObject.layer = LayerMask.NameToLayer("Default");
                        gameObject.tag = "Untagged";
                        Level_1.LevelA_ = 8;  //關卡8
                        break;
                    case 1:
                        boss02_AI.ani.SetTrigger("Dead");
                        break;
                }
            }           
            hp = 0; // 不要扣到負值
            if (PS_Dead != null) PS_Dead.SetActive(true);  //死亡爆炸
            if (Model !=null) Model.SetActive(false);
            switch (MonsterType)  //關閉怪物AI 腳本
            {
                case 0:
                    boss01_AI.enabled = false;
                    break;
                case 1:
                    boss02_AI.enabled = false;
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
        //HpLv = Level_1.MonsterLevel;
        //Level = Settings.Level;
        //Level = Level +1;
        //if (HpLv > 0)
        //{
        //    hpFull[MonsterType] = 7 + (HpLv * Level);
        //    if (hpFull[MonsterType] >= 7 + (5 * Level))
        //    {
        //        hpFull[MonsterType] = 7 + (5 * Level);
        //    }
        //}
        //print("怪物血量:" + hpFull);  //最終血量 12 / 17 / 22 
        hpFull = new float[] { 310, 400 };
        hp = hpFull[MonsterType];  //補滿血量
    }
    void OnDisable()
    {
        Scoreboard.AddScore(true);  //怪物擊殺
        Shop.AddKillScore();  //怪物擊殺分數
        DifficultyUp();
        if (PS_Dead != null) PS_Dead.SetActive(false);
        DeadTime = 0;
        switch (MonsterType)  //開啟怪物AI 腳本
        {
            case 0:
                boss01_AI.enabled = true;
                boss01_AI.ani.SetBool("Attack", true);
                break;
            case 1:
                boss02_AI.enabled = true;
                break;
        }
        agent.enabled = true; // 開啟尋徑功能
        if (Model != null) Model.SetActive(true);
    }
}
