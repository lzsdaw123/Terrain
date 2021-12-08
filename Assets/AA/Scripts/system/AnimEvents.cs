using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public int UnitType;  //0=玩家 / 1=NPC
    public static bool DontShooting;
    public static int ammunition, Total_ammunition;  //彈藥量
    int[] WeaponAmm = new int[] { 30, 6 }; //武器可裝填彈藥量
    [SerializeField] int WeaponType; //武器類型

    public MonsterAI02 MonsterAI02;
    public MonsterAI03 MonsterAI03;
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
        //Move = animator.SetBool("Move", bool );
        h = PlayerMove.h;
        v = PlayerMove.v;
        WeaponType = Shooting.WeaponType;
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
            DontShooting = false;
        }    
    }
    void FireEnd()
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
    void ReLoad()
    {
        if (UnitType == 0)
        {
            ammunition = Shooting.WeapAm[WeaponType];
            Total_ammunition = Shooting.T_WeapAm[WeaponType];

            int R_ammunition = WeaponAmm[WeaponType] - ammunition;  //裝填量 = 武器可裝填量 - 武器當前數量    
            if (Total_ammunition < WeaponAmm[WeaponType])  //總數量<武器可裝填量 {當前數量+總數量}
            {
                ammunition += Total_ammunition;
            }
            else  //總數量>=武器可裝填量 {當前數量+裝填量}
            {
                ammunition += R_ammunition;
            }
            //當前數量 >= 武器可裝填量 {當前數量 = 武器可裝填量}
            if (ammunition >= WeaponAmm[WeaponType]) ammunition = WeaponAmm[WeaponType];
            Total_ammunition -= R_ammunition;  //總數量-裝填量
        }     
    }
    void ReLoadEnd()
    {      
        switch (UnitType)
        {
            case 0:
                Shooting.ReLoad_E();
                break;
            case 1:
                NPC_AI.ReLoad_E();
                break;
        }
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

    //音效用
    public void WalkAudio()
    {
        if (h != 0 || v != 0 )
        {
            AudioManager.PlayFootstepAudio();
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
    void MonsterAudio(int Nub)
    {
        MonsterSource.clip = MonsterClip[Nub];
        MonsterSource.volume = AudioManager.Slider[2].value;
        MonsterSource.mute = AudioManager.muteState[2];
        MonsterSource.Play();   
    }
    void ReloadAudio(int Nub)
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
