using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public int UnitType;  //0=玩家 / 1=NPC
    public int MonsterType;  //0=玩家 / 1=NPC
    public int BossType;  //0=玩家 / 1=NPC
    public static bool DontShooting;
    public static int ammunition, Total_ammunition, Magazine;  //彈藥量
    [SerializeField] int SF_ammunition, SF_Total_ammunition, SF_Magazine;  //彈藥量
    [SerializeField] int[] WeaponAmm = new int[] { 30, 6, 5 }; //武器可裝填彈藥量
    public int R_ammunition;
    [SerializeField] int WeaponType; //武器類型

    public MonsterAI02 MonsterAI02;
    public MonsterAI03 MonsterAI03;
    public Boss01_AI boss01_AI;
    public Boss02_AI boss02_AI;
    public B1_BulletLife b1_BulletLife;
    public B1_BulletHole b1_BulletHole;
    public NPC_Life NPC_Life;
    public HeroLife heroLife;
    public MG_Turret_AI mg_Turret_AI;
    public CameraMove cameraMove;
    public float h, v;


    public AudioManager AudioManager;
    public AudioClip[] NPC_Clip;  //NPC守衛音效
    public AudioClip[] MonsterClip;  //怪物音校
    public float SaveVolume;

    public AudioSource NPC_Source;  //NPC守衛音源
    public AudioSource MonsterSource;  ///怪物攻擊音源
    bool MonsterA=false;

    void Start()
    {
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();  //聲音控制器
        Magazine = Shooting.Weapons[WeaponType].Magazine;
    }
    void Update()
    {
        SF_ammunition = Shooting.Weapons[WeaponType].WeapAm;
        SF_Total_ammunition = Shooting.Weapons[WeaponType].T_WeapAm;
        SF_Magazine = Shooting.Weapons[WeaponType].Magazine;
        //Move = animator.SetBool("Move", bool );
        if (UnitType == 0)
        {
            h = PlayerMove.Player_h;
            v = PlayerMove.Player_v;
            WeaponType = Shooting.WeaponType;
        }

    }

    //玩家&NPC用
    void NoShooting()
    {
        if (UnitType == 0)
        {
            DontShooting = true;
        }    
    }
    void YesShooting()
    {
        if (UnitType == 0)
        {
            Shooting.FireButtle = 1;
            Shooting.Reload = false;
            DontShooting = false;
        }    
    }
    void FireEnd()  //開火結束
    {
        switch (UnitType)
        {
            case 0:
                Shooting.Loaded();
                break;
            case 1:
                NPC_AI.Loaded();
                break;
        }   
    }
    void Bumb()  //投彈
    {
        Shooting.Bumb();
    }
    void BumbEnd()  //投彈結束
    {
        Shooting.BumbEnd();
    }
    void ReLoad()  //換彈
    {
        if (UnitType == 0)
        {
            ammunition = Shooting.Weapons[WeaponType].WeapAm;
            Total_ammunition = Shooting.Weapons[WeaponType].T_WeapAm;
            Magazine = Shooting.Save_Magazine[WeaponType];
            WeaponAmm[WeaponType] = Magazine; //武器可裝填量 = 彈匣容量
            R_ammunition = WeaponAmm[WeaponType] - ammunition;  //裝填量 = 武器可裝填量 - 武器當前子彈數    
            if (Total_ammunition < R_ammunition)  //總數量< 裝填量 {當前數量+總數量}
            {
                if (Total_ammunition <= 0)
                {
                    Total_ammunition = 0;  // 總數量=0
                }
                else
                {
                    switch (Shooting.WeaponType)
                    {
                        case 0:
                        case 1:
                            ammunition += Total_ammunition;  //彈藥 + 總數量
                            Total_ammunition = 0;  //總數量=0
                            break;
                        case 2:  //霰彈槍
                            ammunition += 1;
                            Total_ammunition -= 1;
                            break;
                    }
                }
            }
            else  //總數量>=武器可裝填量 {當前數量+裝填量}
            {
                switch (Shooting.WeaponType)
                {
                    case 0:
                    case 1:
                        ammunition += R_ammunition;  //彈藥 + 裝填量
                        Total_ammunition -= R_ammunition;  //總數量 - 裝填量
                        break;
                    case 2:
                        if (R_ammunition != 0)
                        {
                            ammunition += 1;
                            Total_ammunition -= 1;
                        }
                        break;
                }
            }
            //當前數量 >= 武器可裝填量 {當前數量 = 武器可裝填量}
            //if (ammunition >= WeaponAmm[WeaponType]) ammunition = WeaponAmm[WeaponType];         
        }     
    }
    void ReLoadEnd()
    {      
        switch (UnitType)
        {
            case 0:
                switch (Shooting.WeaponType)
                {
                    case 0:
                    case 1:
                        Shooting.ReReLoad(false);
                        break;
                    case 2:
                        if (ammunition < 5 && Total_ammunition >0)
                        {
                            Shooting.ReReLoad(true);
                        }
                        else
                        {
                            Shooting.ReReLoad(false);
                        }
                        break;
                }
                if (Total_ammunition <= 0) Total_ammunition = 0;
                Shooting.ReLoad_E(ammunition, Total_ammunition);
                break;
            case 1:
                NPC_AI.ReLoad_E();
                break;
        }
    }
    void NPC_Dead(int N)
    {
        NPC_Life.DeadExp(N);
    }
    void InfectionEnd()  //感染生成結束
    {
        heroLife.Crystal_Infection = false;
    }
    void InfectionSW()  //感染生成切換
    {
        heroLife.InfectionSW = false;
    }

    //敵人用
    void M1_Attack()
    {
        MonsterAI02.AttackAning(true, 1);
    }
    void M2_AttackEnd()
    {
        MonsterAI03.AttackAning(true, 1);
    }
    void Boss1_Exp()  //水晶Boss出場
    {
        boss01_AI.ani.SetBool("Start", true);
        boss01_AI.Scenes_ani.SetBool("Start", true);
        boss01_AI.AttackStatus = true;
        cameraMove.CameraMoveEnd(2);
        print("0");
    }
    void B1_gEnd()
    {
        b1_BulletLife.StartAttack = true;
    }
    void B1_BH_gEnd(int Type)  //彈孔生成後
    {
        b1_BulletHole.Generate(Type);
    }
    void Boss2_Start() //機械Boss甦醒
    {
        GameObject.Find("CameraMove").GetComponent<CameraMove>().CameraMoveEnd(1);
        boss02_AI.StartTime = 0;
    }
    void Boss2_Attack(int Type)  //機械Boss攻擊
    {
        boss02_AI.AttackBullet(Type);
        switch (Type)
        {
            case 1:
                MonsterSource.clip = MonsterClip[1];
                MonsterSource.volume = 0.5f;
                break;
            case 2:
                MonsterSource.clip = MonsterClip[2];
                MonsterSource.volume = 0.9f;
                break;
        }
        SaveVolume = MonsterSource.volume;
        MonsterSource.volume = SaveVolume * AudioManager.Slider[2].value;
        MonsterSource.mute = AudioManager.muteState[2];
        MonsterSource.Play();
    }
    void Boss2_AttackEnd(int Type)  //機械Boss攻擊結束
    {
        boss02_AI.AttackAning(true, 1);
    }
    void MG_Turret_Attack(int Type)  //機槍塔攻擊
    {
        switch (Type)
        {
            case 0: //攻擊結束
                mg_Turret_AI.AttackAning(true, 1);
                break;
            case 1:  //射出第一發子彈
                mg_Turret_AI.FlyStart[0] = true;
                mg_Turret_AI.HitTarget[0] = false;
                break;
            case 2:  //射出第二發子彈
                mg_Turret_AI.FlyStart[1] = true;
                mg_Turret_AI.HitTarget[1] = false;
                break;
        }
    }

    //音效用
    public void WalkAudio(int Type)
    {
        if (Type == 0)
        {
            if (h != 0 || v != 0)
            {
                AudioManager.PlayFootstepAudio(0);
            }
        }
    }
    public void JumpAudio()
    {
        //AudioManager.PlayJumpAudio();
    }
    void WalkCilpRight()
    {

    }
    public void NPC_Audio(int Nub)
    {
        NPC_Source.clip = NPC_Clip[Nub];
        NPC_Source.volume = AudioManager.Slider[2].value;
        NPC_Source.mute = AudioManager.muteState[2];
        NPC_Source.pitch = 1.3f;
        NPC_Source.Play();
    }
    public void MonsterAudio(int Nub)  //怪物音效
    {
        if(Nub==0 || Nub == 1)
        {
            MonsterSource.pitch = 2;
        }
        if (Nub == 2)
        {
            MonsterSource.pitch = 1;
        }
        MonsterSource.clip = MonsterClip[Nub];
        MonsterSource.volume = AudioManager.Slider[2].value;
        MonsterSource.mute = AudioManager.muteState[2];
        MonsterSource.Play();   
    }
    public void BossAudio(int Nub)  //怪物音效
    {
        switch (BossType)
        {
            case 0:
                break;
            case 1:
                switch (Nub)
                {
                    case 0:
                        MonsterSource.clip = MonsterClip[0];
                        MonsterSource.volume = 0.7f;
                        break;
                }
                break;
        }
        SaveVolume = MonsterSource.volume;
        MonsterSource.volume = SaveVolume * AudioManager.Slider[2].value;
        MonsterSource.mute = AudioManager.muteState[2];
        MonsterSource.Play();
    }
    void ReloadAudio(int Nub)  //換彈音效
    {
        switch (UnitType)
        {
            case 0:
                AudioManager.Reload(Nub);
                break;
            case 1:
                NPC_Audio(Nub);
                break;
        }
    }
}
