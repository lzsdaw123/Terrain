using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroLife : MonoBehaviour
{
    public static float fullHp,hp, hp_R;  //滿血時數值, 實際, 紅血
    public Image HP_W, HP_R; //血球的UI物件
    public static bool Dead;
    public float UiTime=0;
    bool Invincible=false;
    GameObject DeBugT;
    public int WeaponType; //武器類型
    public int HitType;
    public Animator[] PlayerAni;
    public Animator BossHit_Ani;
    public ParticleSystem[] Hit_Player;
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

    void Start()
    {
        Level = InfectionValue = 0;
        DeadTime = 0;
        LiftTime = 0;
        InfectionValueUp = new int[] { 1, 2, 3, 3 };
        Crystal_Infection = false;
        hp = hp_R = fullHp = 20; //遊戲一開始時先填滿血
        Dead = false;
        DeBugT = GameObject.Find("DeBugT").gameObject;  //開發模式文字
        DeBugT.SetActive(false);
        for (int i=0; i< Hit_Player.Length; i++)
        {
            if (Hit_Player[i] != null)
            {
                Hit_Player[i].Stop();
                Hit_Player[i].gameObject.SetActive(false);
            }
        }
        InfectionUI.SetActive(false);
    }
    public void Damage(float Power) // 接受傷害
    {
        hp -= Power; // 扣血
    }
    public void DamageEffects(int hitType)
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
    public static void DownDamage(int Dps)  //摔落傷害
    {
        hp -= Dps;
    }

    void Update()
    {
        if (PlayerRebirth)  //玩家重生
        {
            PlayerAni[0].SetTrigger("InfectionDead");
            PlayerAni[1].SetTrigger("InfectionDead");
            BossHit_Ani.SetTrigger("Dead");
            InfectionUI.SetActive(false);
            LiftTime = 0;
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
        if (Level > 0)
        {
            Infection_Image.fillAmount = LiftTime / 12; //顯示血球
            InfectionUI.SetActive(true);
            LiftTime += Time.deltaTime;
            if (LiftTime >= 12)
            {
                LiftTime = 0;
                Level = 0;
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

        WeaponType = Shooting.WeaponType;

        if (Level >= 4)
        {
            DeadTime += Time.deltaTime;
            if (DeadTime >= 2)
            {
                DeadTime = 2;
                hp -= 1;
            }
        }
        if (hp <= 0)
        {
            Hit_Player[HitType].Stop();
            Hit_Player[HitType].gameObject.SetActive(false);
            DeadTime=0;
            hp = 0; // 不要扣到負值
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
        hp = hp_R = fullHp; //遊戲一開始時先填滿血        
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
}
