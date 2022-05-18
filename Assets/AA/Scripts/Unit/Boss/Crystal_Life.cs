using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Rendering.HighDefinition;

public class Crystal_Life : MonoBehaviour
{
    [SerializeField] Transform root;

    public Animator ani;
    public Animator CrystalAni;
    private Rigidbody rigid;
    private Collider cld;
    public AnimEvents AnimEvents;
    public bool isBoss;
    public int MonsterType;  //水晶類型 0=場景水晶, 1= 目標水晶, 2=機槍塔
    public static int PS_MonsterType;  //水晶類型 0=場景水晶, 1= 目標水晶, 2=機槍塔
    public int BossLevel;  //Boss2等級
    public float[] hpFull; // 血量上限
    public float hp; // 血量
    public bool 無敵=false;
    int HpLv;  //生命等級
    int Level;  //難度等級
    //public Image hpImage;

    private NavMeshAgent agent;
    public Boss01_AI boss01_AI;
    public Boss02_AI boss02_AI;
    public MG_Turret_AI mg_Turret_AI;
    public Level_1 level_1;
    //public Level_2 level_2;

    public GameObject PS_Dead;  //死亡特效
    public GameObject Model;  //模型
    public MeshRenderer MR;  //模型
    public DecalProjector Decal;  //貼花
    public MeshCollider meshCollider;
    public Collider Collider;
    [SerializeField] float DeadTime;

    public GameObject HitUI;  //命中UI
    float HitUITime;
    bool Player;
    Color UIcolor;
    public bool Dead;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        cld = GetComponent<Collider>();
        //agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        HitUI = Save_Across_Scene.HitUI;
        if (PS_Dead!=null) PS_Dead.SetActive(false);
        if (cld != null) cld.enabled = true;
        DeadTime = 0;
        DifficultyUp();  //難度調整
        RefreshLifebar(); // 更新血條
        HitUITime = 0;
        Dead = false;
        //ani = GetComponent<Animator>();

        PS_MonsterType = MonsterType;
        //RagdollActive(false); // 先關閉物理娃娃
        switch (MonsterType)
        {
            case 0:
                MR = GetComponent<MeshRenderer>();
                gameObject.layer = LayerMask.NameToLayer("Wall");
                gameObject.tag = "Crystal";
                break;
            case 1:
                gameObject.layer = LayerMask.NameToLayer("Monster");
                gameObject.tag = "Crystal";
                Decal.fadeFactor = 0.5f;
                Decal.gameObject.transform.GetChild(0).GetComponent<SphereCollider>().enabled = false;
                break;
            case 2:
                gameObject.layer = LayerMask.NameToLayer("Monster");
                gameObject.tag = "Enemy";
                mg_Turret_AI.enabled = true;
                break;
        }
        if (MR != null) MR.enabled = true;
        if (meshCollider != null) meshCollider.enabled = true;
        if (Collider != null) Collider.enabled = true;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Y)) // 測試用
        //{
        //    monster02.enabled = false; // 關閉控制腳本
        //    agent.enabled = false;  //立即關閉尋徑功能
        //    ani.SetTrigger("Die");
        //}
        if(PS_Dead!=null)
        {
            if (PS_Dead.activeSelf)
            {
                DeadTime += 1.4f * Time.deltaTime;
                if (DeadTime >= 1)
                {
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
    public void Damage(float Power)  //受到傷害
    {
        //print(Power);
        hp -= Power; // 扣血
        if (無敵) hp = hpFull[MonsterType];  //補滿血量
        if (hp >0)
        {
            if (MonsterType != 0)
            {
                HitUI.SetActive(true);
                HitUI.GetComponent<Image>().color = Color.white;
                if (Player)
                {
                    HitUI.SetActive(true);
                    HitUI.GetComponent<Image>().color = Color.white;
                }
            }
        }
        if (hp <= hpFull[MonsterType] /2)  //怪物血量低於一半
        {
            switch (MonsterType)
            {
                case 0:
                    break;
                case 1:
                    //monster03.ani.SetInteger("Level", 1);
                    break;
            }            
        }
        if (hp <= 0)  //死亡
        {
            if (!Dead)
            {
                if (MonsterType != 0)
                {
                    HitUI.SetActive(true);
                    HitUI.GetComponent<Image>().color = Color.red;
                    if (Player)
                    {
                        HitUI.SetActive(true);
                        HitUI.GetComponent<Image>().color = Color.red;
                        //AudioManager.Hit(4);  //玩家擊殺音效
                    }
                }

                Dead = true;
                //AnimEvents.MonsterAudio(2);  //怪物爆汁音效
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Untagged";
            }
            if(Decal !=null)
            {
                Decal.fadeFactor = 1;
                Decal.gameObject.transform.GetChild(0).GetComponent<SphereCollider>().enabled = true;  //弱點血量
            }
            hp = 0; // 不要扣到負值
            if (PS_Dead != null) PS_Dead.SetActive(true);  //死亡爆炸
            if (Model !=null) Model.SetActive(false);
            if (MR != null) MR.enabled = false;
            if (cld != null) cld.enabled = false;
            if (meshCollider != null) meshCollider.enabled = false;
            if (Collider != null) Collider.enabled = false;
            switch (MonsterType)  //關閉怪物AI 腳本
            {
                case 0:
                    break;
                case 1:  //目標水晶
                    break;
                case 2:
                    ani.SetTrigger("Dead");
                    mg_Turret_AI.enabled = false;
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
        hpFull = new float[] { 50, 25, 25};
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
                break;
            case 1:
                break;
            case 2:
                mg_Turret_AI.enabled = true;
                break;
        }
        //agent.enabled = true; // 開啟尋徑功能
        if (Model != null) Model.SetActive(true);
    }
}
