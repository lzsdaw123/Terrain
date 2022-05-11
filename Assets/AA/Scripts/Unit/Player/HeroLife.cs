using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroLife : MonoBehaviour
{
    public Camera Camera;
    public static int HpLv;
    [SerializeField] int SF_HpLv;
    public static float fullHp, hp, hp_R;  //滿血時數值, 實際, 紅血
    [SerializeField] float SF_fullHp, SF_hp;  //滿血時數值, 實際, 紅血
    public Image HP_W, HP_R; //血球的UI物件
    public GameObject[] Hit_Direction_UI;
    public GameObject[] BloodpackUI=new GameObject[3];  //補包UI
    public static int BloodpackNub; //補包數量
    public int SaveBpN;  //保存補包數量
    public static bool GetBP;  //取得補包
    public float BpTime;  //補包使用冷卻
    public static bool Dead;
    public float UiTime=0;
    bool Invincible=false;
    GameObject DeBugT;
    public int WeaponType; //武器類型
    public int HitType;
    public Animator[] PlayerAni;
    public Animator BossHit_Ani;
    public ParticleSystem[] Hit_Player;  //玩家受傷特效
    public float HP_R_speed =0.8f;
    bool playing ;
    public bool Crystal_Infection;
    public int Level;  //水晶感染等級
    public int LV_Level;  //水晶感染等級
    public int InfectionValue;  //感染值
    public int[] InfectionValueUp=new int[4];  //感染次數
    public float DeadTime;
    public static bool PlayerRebirth;  //玩家重生
    public float LiftTime; //解除倒數
    public GameObject InfectionUI;
    public Image Infection_Image;
    public bool InfectionSW;
    public float dot;
    public float Fdot;
    public float Rdot;
    public bool 無敵;

    void Awake()
    {
        DeBugT = GameObject.Find("DeBugT").gameObject;  //開發模式文字
        for (int Bp = 0; Bp < BloodpackUI.Length; Bp++)
        {
            BloodpackUI[Bp].SetActive(false);
        }
        BloodpackNub = 0;
    }

    void Start()
    {
        無敵 = false;
        Level = InfectionValue = 0;
        DeadTime = 0;
        LiftTime = 0;
        InfectionValueUp = new int[] { 1, 2, 3, 3 };
        Crystal_Infection = InfectionSW = false;
        HpLv = 1;
        hp = hp_R = fullHp = 20 * HpLv; //遊戲一開始時先填滿血
        GetBP = false;
        BpTime = -1;
        Dead = false;

        for (int i=0; i< Hit_Player.Length; i++)  //命中玩家特效
        {
            if (Hit_Player[i] != null)
            {
                Hit_Player[i].Stop();
                Hit_Player[i].gameObject.SetActive(false);
            }
        }
        InfectionUI.SetActive(false);
        DeBugT.SetActive(false);
    }
    public void Damage(float Power) // 接受傷害
    {
        if (BpTime == -1)
        {
            hp -= Power; // 扣血
        }
    }
    public void DamageEffects(int hitType)  //受傷特效
    {
        HitType = hitType;
        switch (HitType)
        {
            case 0:  //水晶 尖
                if (InfectionValue >= 0)
                {
                    InfectionValue++;
                    LiftTime = 0;
                }
                break;
            case 1:  //水晶 長
                break;
            case 2:  //水晶 方
                break;
            case 3:  //蠍子
                break;
        }
        Hit_Player[HitType].gameObject.SetActive(true);
        Hit_Player[HitType].Play();
        playing = true;
    }
    public void closeDamageEffects()  //關閉受傷特效
    {
        for (int i = 0; i < Hit_Player.Length; i++)  //受傷特效
        {
            if (Hit_Player[i] != null)
            {
                Hit_Player[i].Stop();
                Hit_Player[i].gameObject.SetActive(false);
            }
        }
    }
    public void hit_Direction(Transform Hit_transform)  //命中方向
    {
        Vector3 dirForward = (Hit_transform.transform.position - transform.position).normalized;
        dot = Vector3.Dot(transform.forward, dirForward);     //判斷物體是否在相機前面
        Vector3 Bdir = Hit_transform.transform.position - Camera.transform.position; //位置差，方向  
        Rdot = Vector3.Dot(Camera.transform.right, Bdir.normalized);//點乘判斷左右： Rdot >0在右，<0在左
        Fdot = Vector3.Dot(Camera.transform.forward, Bdir.normalized);//點乘判斷前後：Fdot >0在前，<0在後
        if (dot > 0)
        {
            if (Fdot > 0.86)  //擊中正面
            {
                Hit_Direction_UI[0].SetActive(true);
            }
            else if (Fdot < -0.35) //後面
            {
                Hit_Direction_UI[1].SetActive(true);
            }
            if (Rdot > 0.5)  //R 右邊
            {
                Hit_Direction_UI[2].SetActive(true);

            }
            else if (Rdot < -0.5)  //L 左邊
            {
                Hit_Direction_UI[3].SetActive(true);
            }
        }
        else if(dot<0)  //擊中背面
        {
            Hit_Direction_UI[1].SetActive(true);

            if (Rdot > 0.1)  //R 右邊
            {
                Hit_Direction_UI[2].SetActive(true);

            }
            else if (Rdot < -0.1)  //L 左邊
            {
                Hit_Direction_UI[3].SetActive(true);
            }
        }
    }
    public static void DownDamage(int Dps)  //摔落傷害
    {
        hp -= Dps;
    }

    void Update()
    {
        SF_HpLv = HpLv;
        SF_fullHp = fullHp;
        SF_hp = hp;
        if (PlayerRebirth)  //玩家重生
        {
            BloodpackNub = SaveBpN;  //回復補包數量
            for (int Bp = 0; Bp < SaveBpN; Bp++)
            {
                BloodpackUI[Bp].SetActive(true);
                SaveBpN = 0;
            }
            PlayerAni[0].SetTrigger("InfectionLift");
            PlayerAni[1].SetTrigger("InfectionLift");
            BossHit_Ani.SetTrigger("Lift");
            InfectionUI.SetActive(false);
            LiftTime = 0;
            Level = 0;
            PlayerRebirth = false;
            Start();
        }
        if (Level < 4)
        {
            if (InfectionValue >= InfectionValueUp[Level])
            {
                InfectionValue = 0;
                Level++;
                Crystal_Infection = true;
            }
        }
        else if(Level>=4)
        {
            LiftTime = 0;
        }
        if (Level > 0)
        {
            Infection_Image.fillAmount = LiftTime / 12; //顯示血球
            InfectionUI.SetActive(true);
            LiftTime += Time.deltaTime;
            if (LiftTime >= 12)  //感染解除時間
            {
                LiftTime = 0;
                Level = 0;
                InfectionValue = 0;
                Hit_Player[HitType].gameObject.SetActive(true);
                Hit_Player[0].Play();
                PlayerAni[0].SetInteger("InfectionLevel", 0);
                PlayerAni[1].SetInteger("InfectionLevel", 0);
                BossHit_Ani.SetInteger("Level", 0);
                PlayerAni[0].SetTrigger("InfectionLift");
                PlayerAni[1].SetTrigger("InfectionLift");
                BossHit_Ani.SetTrigger("Lift");
                InfectionUI.SetActive(false);
            }
        }
        if (Crystal_Infection)
        {
            PlayerAni[0].SetInteger("InfectionLevel", Level);
            PlayerAni[1].SetInteger("InfectionLevel", Level);
            BossHit_Ani.SetInteger("Level", Level);
        }
        if (InfectionSW)
        {
            PlayerAni[0].SetTrigger("InfectionSw");
            PlayerAni[1].SetTrigger("InfectionSw");
            PlayerAni[0].SetInteger("InfectionLevel", Level);
            PlayerAni[1].SetInteger("InfectionLevel", Level);
        }

        WeaponType = Shooting.WeaponType;

        if (Level >= 4)  //水晶感染Lv4 狂扣血
        {
            DeadTime += Time.deltaTime;
            if (DeadTime >= 2)
            {
                DeadTime = 2;
                hp -= 1 *HpLv;
            }
        }
        if (無敵)
        {
            hp = fullHp;  //補滿血量
            HP_W.color = new Color(0, 0.85f, 0.70f, 1);
        }
        if(!無敵 && BpTime ==-1)
        {
            HP_W.color = new Color(0, 0.57f, 0.85f, 1);
        }
        if (hp <= 0)  //玩家死亡
        {
            hp = 0; // 不要扣到負值
            Hit_Player[HitType].Stop();
            Hit_Player[HitType].gameObject.SetActive(false);
            DeadTime=0;
            SaveBpN = BloodpackNub;         
            HP_W.color = new Color(0, 0.57f, 0.85f, 1);
            Dead = true;
        }
        if (Hit_Player[HitType] != null)
        {
            if (Hit_Player[HitType].isStopped && playing)  //粒子結束時關掉
            {
                playing = false;
                Hit_Player[HitType].gameObject.SetActive(false);
            }
        }
        HP_W.fillAmount = hp / fullHp; //顯示血球
        HP_R.fillAmount = hp_R / fullHp; //顯示血球
        if (hp != hp_R)
        {
            UiTime +=4* Time.deltaTime;
            if (UiTime >= 2f)
            {
                UiTime = 2;
                hp_R -= HP_R_speed * Time.deltaTime;
            }
        }
        if(hp_R <=hp)
        {
            hp_R = hp;
            UiTime = 0;
        }
        if (GetBP)  //取得補包
        {
            GetBP = false;
            BloodpackNub++;
            BloodpackUI[BloodpackNub-1].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Q) && BloodpackNub>0) //使用補包
        {
            if(hp< fullHp && BpTime==-1)
            {
                HP_W.color = new Color(0, 0.85f, 0.70f, 1);
                BpTime = 0;
                LiftTime = 12;
                hp = fullHp;
                BloodpackNub--;
                if (BloodpackNub <= 0) BloodpackNub = 0;
                 BloodpackUI[BloodpackNub].SetActive(false);
                AudioManager.PickUp(1);
            }
        }
        if (BpTime >= 0)  //補包冷卻時間
        {
            BpTime += Time.deltaTime;
            if (BpTime >= 2.866f)
            {
                BpTime = -1;
                HP_W.color = new Color(0, 0.57f, 0.85f, 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.L)) //開發者模式
        {
            Damage(5);
            DamageEffects(0);
        }
        if (hp >= fullHp)
        {
            hp = fullHp;
        }
        if (Input.GetKeyDown(KeyCode.K))  //開發者模式
        {         
            if (Invincible)
            {
                DeBugT.SetActive(false);
                Invincible = false;
            }
            else
            {
                DeBugT.SetActive(true);
                Invincible = true;                
            }
        }
        if(Invincible) hp = fullHp;
    }
    public static void PlayerRe()  //重生
    {
        hp = hp_R = fullHp = 20 * HpLv; //遊戲一開始時先填滿血
        Dead = false;
        PlayerRebirth = true;
    }
    public static void AddHp()
    {
        hp += 1.6f * Time.smoothDeltaTime;
    }
    public static void HpUp()
    {
        fullHp = 20 + Shop.HpLv * 5;
        hp= hp_R = fullHp;
    }
    public static void GetBloodpack()  //取得補包
    {
        GetBP = true;
    }
}
