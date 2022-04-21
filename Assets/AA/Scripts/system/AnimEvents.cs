using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public int UnitType;  //0=玩家 / 1=NPC
    public static bool DontShooting;
    public static int ammunition, Total_ammunition;  //彈藥量
    [SerializeField] int SF_ammunition, SF_Total_ammunition;  //彈藥量
    [SerializeField] int[] WeaponAmm = new int[] { 30, 6, 5 }; //武器可裝填彈藥量
    public int R_ammunition;
    [SerializeField] int WeaponType; //武器類型

    public MonsterAI02 MonsterAI02;
    public MonsterAI03 MonsterAI03;
    public Boss01_AI boss01_AI;
    public B1_BulletLife b1_BulletLife;
    public B1_BulletHole b1_BulletHole;
    public NPC_Life NPC_Life;
    public HeroLife heroLife;
    public CameraMove cameraMove;
    public float h, v;


    public AudioManager AudioManager;
    public AudioClip[] NPC_Clip;  //NPC守衛音效
    public AudioClip[] MonsterClip;  //怪物音校

    public AudioSource NPC_Source;  //NPC守衛音源
    public AudioSource MonsterSource;  ///怪物攻擊音源
    bool MonsterA=false;

    void Start()
    {
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();  //聲音控制器
    }
    void Update()
    {
        SF_ammunition = Shooting.Weapons[WeaponType].WeapAm;
        SF_Total_ammunition = Shooting.Weapons[WeaponType].T_WeapAm;
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
    void BumbEnd()
    {
        Shooting.BumbEnd();
    }
    void ReLoad()
    {
        if (UnitType == 0)
        {
            ammunition = Shooting.Weapons[WeaponType].WeapAm;
            Total_ammunition = Shooting.Weapons[WeaponType].T_WeapAm;

            R_ammunition = WeaponAmm[WeaponType] - ammunition;  //裝填量 = 武器可裝填量 - 武器當前數量    
            if (Total_ammunition < R_ammunition)  //總數量<武器可裝填量 {當前數量+總數量}
            {
                if (Total_ammunition <= 0)
                {
                    Total_ammunition = 0;  // 總數量=0
                }
                else
                {
                    ammunition +=  Total_ammunition;  //彈藥 + 總數量
                    Total_ammunition = 0;  //總數量=0
                }
            }
            else  //總數量>=武器可裝填量 {當前數量+裝填量}
            {
                ammunition += R_ammunition;  //彈藥 + 裝填量
                Total_ammunition -= R_ammunition;  //總數量 - 裝填量
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

    //怪物用
    void M1_Attack()
    {
        MonsterAI02.AttackAning(true, 1);
    }
    void M2_AttackEnd()
    {
        MonsterAI03.AttackAning(true, 1);
    }
    void Boss1_Exp()
    {
        boss01_AI.ani.SetBool("Start", true);
        boss01_AI.Scenes_ani.SetBool("Start", true);
        boss01_AI.AttackStatus = true;
        cameraMove.StartBoos1();
    }
    void B1_gEnd()
    {
        b1_BulletLife.StartAttack = true;
    }
    void B1_BH_gEnd(int Type)  //彈孔生成後
    {
        b1_BulletHole.Generate(Type);
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
