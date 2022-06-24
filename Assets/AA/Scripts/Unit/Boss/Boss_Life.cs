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
    public float[] hp_W, hp_R; // 血量
    public float[] UiTime;
    public bool 無敵=false;
    int HpLv;  //生命等級
    int Level;  //難度等級

    private NavMeshAgent agent;
    public Boss01_AI boss01_AI;
    public Boss02_AI boss02_AI;
    public Level_1 level_1;
    //public Level_2 level_2;

    public GameObject[] ChildGameObject;  //子物件
    public GameObject PS_Dead;  //死亡特效
    public GameObject Model;  //模型
    [SerializeField] float DeadTime;

    public GameObject HitUI;  //命中UI
    public GameObject Boss1HpUI;
    public GameObject[] Boss2HpUI;
    public Image[] i_HP_W, i_HP_R;
    bool Player;
    public bool Dead;
    public GameObject HitObject;
    public GameObject[] Crystal_Weakness;  //水晶弱點
    public GameObject[] WeaknessObject;
    public GameObject[] WeaknessObjectPS;
    public float[] Weakness_Hp;  //弱點血量
    public float[] Weakness_Hp_R;  //弱點血量
    public float[] Weakness_Hp_Max;  //弱點血量
    public int Hp_Type;
    bool YesStart;
    public float cTime;
    public int D_Level;
    public bool ReHpUI;  //更新血量UI

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
        cTime = -1;
        D_Level = 0;
        Dead  = false;
        //ani = GetComponent<Animator>();
        PS_MonsterType = MonsterType;
        //RagdollActive(false); // 先關閉物理娃娃
        gameObject.layer = LayerMask.NameToLayer("Monster");
        gameObject.tag = "Enemy";   
        switch (MonsterType)
        {
            case 0:
                Boss1HpUI.SetActive(false);
                for (int i = 0; i < ChildGameObject.Length; i++)
                {
                    ChildGameObject[i].layer = LayerMask.NameToLayer("Monster");                    
                }
                UiTime = new float[1];
                break;
            case 1:
                Boss2HpUI = Save_Across_Scene.ps_Boss2HpUI;
                for (int i = 0; i < Boss2HpUI.Length; i++)
                {
                    Boss2HpUI[i].SetActive(false);
                }
                boss02_AI.RangeObject[0].SetActive(true);
                UiTime = new float[6];
                break;
        }
        for(int i=0; i< Crystal_Weakness.Length; i++)
        {
            Crystal_Weakness[i].GetComponent<Crystal_Life>().無敵 = true;
            YesStart = false;
        }
    }

    void Update()
    {
        switch (MonsterType)
        {
            case 1:
                if (Boss02_AI.StartAttack && !YesStart)
                {
                    YesStart = true;
                    Crystal_Weakness[0].GetComponent<Crystal_Life>().無敵 = false;
                    cTime = 0;
                }
                break;
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
        if (cTime>=0)
        {
            cTime += Time.deltaTime;
            switch (D_Level)
            {
                case 0:  //弱點提示
                    if(cTime >= 2.5f)
                    {
                        cTime = -1;
                        PlayerView.Crystal_Weakness = 0;
                        PlayerView.missionChange(4, 1);  //改變關卡
                        DialogueEditor.StartConversation(4, 1, 2, false, 0, true);  //開始對話
                        Boss02_AI.D_level = 2;
                        Level_1.UiOpen = true;
                    }
                    break;
                case 1:  //第四階段
                    if (cTime >= 14 && !DialogueEditor.Talking)
                    {
                        cTime = -1;
                        PlayerView.Crystal_Weakness = 6;
                        Boss2HpUI[5].SetActive(true);
                        PlayerView.missionChange(4, 3);  //改變關卡
                        DialogueEditor.StartConversation(4, 3, 2, false, 0, true);  //開始對話
                        Level_1.UiOpen = true;
                    }
                    break;
            }
        }
        //if (ReHpUI)
        //{
        //    RefreshLifebar();
        //}
        RefreshLifebar();
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
        if (hp_W[MonsterType] > 0)
        {
            if (Player)
            {
                HitUI.SetActive(true);
                HitUI.GetComponent<Image>().color = Color.white;
            }
        }
        switch (MonsterType)
        {
            case 0:  //水晶Boss
                hp_W[0] -= Power; // 扣血
                if (hp_W[0] <= hpFull[MonsterType] / 2)  //怪物血量低於一半
                {
                    //ani.SetBool("Move", true);
                }
                
                break;
            case 1:  //機械Boss
                for (int i = 0; i < WeaknessObject.Length; i++)
                {
                    if (WeaknessObject[i] == HitObject)
                    {
                        Weakness_Hp[i] -= Power;
                        Hp_Type = i;
                        //hp -= Power;                      
                    }
                    if (Weakness_Hp[i] <= 0)
                    {
                        WeaknessObject[i].GetComponent<SphereCollider>().enabled = false;
                        HitUI.SetActive(true);
                        HitUI.GetComponent<Image>().color = Color.red;  //紅色擊殺UI
                    }
                }
                if (Weakness_Hp[0] <= 0)  //左手臂弱點擊破
                {
                    hp_W[1] -= Weakness_Hp_Max[0];
                    i_HP_W[0].fillAmount = hp_W[1] / hpFull[1]; //顯示血球
                    Weakness_Hp[0] = 60;
                    WeaknessObjectPS[0].SetActive(true);
                    boss02_AI.Level = 2;
                    boss02_AI.ani.SetInteger("Level", 2);
                    boss02_AI.ani.SetTrigger("LevelUp");
                    Crystal_Weakness[1].GetComponent<Crystal_Life>().無敵 = false;
                    Crystal_Weakness[2].GetComponent<Crystal_Life>().無敵 = false;
                    Boss2HpUI[1].SetActive(false);
                    Boss2HpUI[2].SetActive(true);
                    Boss2HpUI[3].SetActive(true);
                    PlayerView.Crystal_Weakness = 1;
                }
                if(Weakness_Hp[1] <= 0 && Weakness_Hp[1] > -99)  //腹部上弱點擊破
                {
                    hp_W[1] -= Weakness_Hp_Max[1];
                    i_HP_W[0].fillAmount = hp_W[1] / hpFull[1]; //顯示血球
                    WeaknessObjectPS[1].SetActive(true);
                    Weakness_Hp[1] = -99;
                    PlayerView.Crystal_Weakness = 2;
                }
                if (Weakness_Hp[2] <= 0 && Weakness_Hp[2] >-99)  //腹部下弱點擊破
                {
                    hp_W[1] -= Weakness_Hp_Max[2];
                    i_HP_W[0].fillAmount = hp_W[1] / hpFull[1]; //顯示血球
                    WeaknessObjectPS[2].SetActive(true);
                    Weakness_Hp[2] = -99;
                    PlayerView.Crystal_Weakness = 3;

                }
                if (Weakness_Hp[1] <= 0 && Weakness_Hp[2] <= 0)  //腹部兩個弱點
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
                    Boss2HpUI[2].SetActive(false);
                    Boss2HpUI[3].SetActive(false);
                    Boss2HpUI[4].SetActive(true);
                    PlayerView.Crystal_Weakness = 4;
                }
                if (Weakness_Hp[3] <= 0)  //左胸弱點擊破
                {
                    hp_W[1] -= Weakness_Hp_Max[3];
                    i_HP_W[0].fillAmount = hp_W[1] / hpFull[1]; //顯示血球
                    Weakness_Hp[3] = 60;
                    Weakness_Hp[4] = 100;
                    WeaknessObjectPS[3].SetActive(true);
                    boss02_AI.Level = 4;
                    boss02_AI.ani.SetInteger("Level", 4);
                    boss02_AI.ani.SetTrigger("LevelUp");
                    boss02_AI.ani.SetBool("Attack1", false);  //第一階攻擊模式
                    boss02_AI.ani.SetBool("Attack2", false);  //第二階攻擊模式
                    cTime = 0;
                    Boss2HpUI[4].SetActive(false);
                    PlayerView.Crystal_Weakness = 5;
                    D_Level = 1;
                }
                if (Weakness_Hp[4] <= 0)  //頭部弱點擊破
                {
                    hp_W[1] -= Weakness_Hp_Max[4];
                    i_HP_W[0].fillAmount = hp_W[1] / hpFull[1]; //顯示血球
                    Weakness_Hp[4] = 600;
                    boss02_AI.ani.SetTrigger("Dead");
                    boss02_AI.ani.SetBool("Attack1", false);  //第一階攻擊模式
                    boss02_AI.ani.SetBool("Attack2", false);  //第二階攻擊模式
                    PlayerView.missionChange(4, 4);  //改變關卡
                    DialogueEditor.StartConversation(4, 4, 2, false, 0, true);  //開始對話
                    Level_1.UiOpen = true;
                    Boss2HpUI[0].SetActive(false);
                    Boss2HpUI[5].SetActive(false);
                    PlayerView.Crystal_Weakness = 7; //關弱點
                    Dead = true;
                }
                break;
        }
        if (無敵) hp_W[MonsterType] = hpFull[MonsterType];  //補滿血量
        if (hp_W[MonsterType] <= 0)  //死亡
        {
            if (!Dead)
            {
                if (Player)
                {
                    HitUI.SetActive(true);
                    HitUI.GetComponent<Image>().color = Color.red;  //紅色擊殺UI
                    //AudioManager.Hit(4);  //玩家擊殺音效
                }
                switch (MonsterType)
                {
                    case 0:
                        //AnimEvents.MonsterAudio(2);  //怪物爆汁音效
                        if (CrystalAni != null) CrystalAni.SetTrigger("Dead");
                        gameObject.layer = LayerMask.NameToLayer("Default");
                        gameObject.tag = "Untagged";
                        for (int i = 0; i < ChildGameObject.Length; i++)
                        {
                            ChildGameObject[i].layer = LayerMask.NameToLayer("Default");
                        }
                        Level_1.LevelA_ = 8;  //關卡8
                        Boss1HpUI.SetActive(false);
                        break;
                    case 1:
                        boss02_AI.ani.SetTrigger("Dead");
                        break;
                }
                Dead = true;
            }
            hp_W[MonsterType] = 0; // 不要扣到負值
            if (PS_Dead != null) PS_Dead.SetActive(true);  //死亡爆炸
            if (Model !=null) Model.SetActive(false);
            switch (MonsterType)  //關閉怪物AI 腳本
            {
                case 0:
                    boss01_AI.enabled = false;
                    break;
                case 1:
                    boss02_AI.enabled = false;
                    boss02_AI.MG_Turret[0].GetComponent<MG_Turret_AI>().StartAttack = false;
                    boss02_AI.MG_Turret[1].GetComponent<MG_Turret_AI>().StartAttack = false;
                    boss02_AI.MG_Turret[2].GetComponent<MG_Turret_AI>().StartAttack = false;
                    PlayerResurrection.Fail = true;
                    print("T");
                    break;
            }           
            //ani.SetTrigger("Die");           
        }
        //ReHpUI = true;
        RefreshLifebar(); // 更新血條
    }

    public void RefreshLifebar() // 更新血條 UI
    {
        switch (MonsterType)
        {
            case 0:
                i_HP_W[0].fillAmount = hp_W[0] / hpFull[0]; //顯示血球
                i_HP_R[0].fillAmount = hp_R[0] / hpFull[0]; //顯示血球
                if (hp_R[0] != hp_W[0])
                {
                    UiTime[0] += 2 * Time.deltaTime;
                    if (UiTime[0] >= 2f)
                    {
                        UiTime[0] = 2;
                        hp_R[0] -= 20f * Time.deltaTime;
                    }
                }
                if (hp_R[0] <= hp_W[0])
                {
                    hp_R[0] = hp_W[0];
                    UiTime[0] = 0;
                }
                break;
            case 1:
                i_HP_W = Save_Across_Scene.ps_HP_W;
                i_HP_R = Save_Across_Scene.ps_HP_R;
                i_HP_W[Hp_Type + 1].fillAmount = Weakness_Hp[Hp_Type] / Weakness_Hp_Max[Hp_Type]; //顯示血球
                i_HP_R[Hp_Type + 1].fillAmount = Weakness_Hp_R[Hp_Type] / Weakness_Hp_Max[Hp_Type]; //顯示血球
                i_HP_W[0].fillAmount = hp_W[1] / hpFull[1]; //顯示血球
                i_HP_R[0].fillAmount = hp_R[1] / hpFull[1]; //顯示血球
                if (Weakness_Hp_R[Hp_Type] != Weakness_Hp[Hp_Type])
                {
                    UiTime[Hp_Type+1] += 8 * Time.deltaTime;
                    if (UiTime[Hp_Type+1] >= 2f)
                    {
                        UiTime[Hp_Type+1] = 2;
                        Weakness_Hp_R[Hp_Type] -= 4f * Time.deltaTime;
                    }
                }
                if (Weakness_Hp_R[Hp_Type] <= Weakness_Hp[Hp_Type])
                {
                    Weakness_Hp_R[Hp_Type] = Weakness_Hp[Hp_Type];
                    UiTime[Hp_Type+1] = 0;
                    //ReHpUI = false;
                }
                if (hp_R[1] != hp_W[1])
                {
                    UiTime[0] += 2 * Time.deltaTime;
                    if (UiTime[0] >= 2f)
                    {
                        UiTime[0] = 2;
                        hp_R[1] -= 20f * Time.deltaTime;
                    }
                }
                if (hp_R[1] <= hp_W[1])
                {
                    hp_R[1] = hp_W[1];
                    UiTime[0] = 0;
                }
                break;
        }
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
        hpFull = new float[] { 300, 250 };
        hp_W = new float[] { 230, 164 }; //補滿血量
        hp_R = new float[] { 230, 164 }; //補滿血量
        Weakness_Hp_Max = new float[] { 20, 20, 20, 24, 80 };
        Weakness_Hp_R = new float[] { 20, 20, 20, 24, 80 };
        Weakness_Hp = new float[] { 20, 20, 20, 24, 80 };
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
