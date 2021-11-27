using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public static bool DontShooting;
    public static int ammunition, Total_ammunition;  //彈藥量
    int[] WeaponAmm = new int[] { 30, 6 }; //武器可裝填彈藥量
    [SerializeField] int WeaponType; //武器類型

    public MonsterAI02 MonsterAI02;
    public MonsterAI03 MonsterAI03;
    [SerializeField] bool attacking = false;
    public int buttleAttack=0;
    public float h, v;

    public AudioManager AudioManager;
    public AudioClip[] MonsterClip;
    public AudioSource MonsterSource;  ///怪物攻擊音源
    bool MonsterA=false;

    void Start()
    {
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    void Update()
    {
        //Move = animator.SetBool("Move", bool );
        h = PlayerMove.h;
        v = PlayerMove.v;
        WeaponType = Shooting.WeaponType;
    }

    void NoShooting()
    {
        DontShooting = true;
    }
    void YesShooting()
    {
        DontShooting = false;
    }
    void FireEnd()
    {
        Shooting.Loaded();
    }
    void ReLoad()
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
            ammunition +=R_ammunition ;
        }
        //當前數量 >= 武器可裝填量 {當前數量 = 武器可裝填量}
        if (ammunition >= WeaponAmm[WeaponType]) ammunition = WeaponAmm[WeaponType];
        Total_ammunition -= R_ammunition;  //總數量-裝填量

    }
    void ReLoadEnd()
    {
        Shooting.ReLoad_E();
    }
    void Attacking()
    {
        attacking = true;
        buttleAttack = 1;
        MonsterAI02.AttackAning(attacking, buttleAttack);
    }
    void M2_Attacking()
    {
        attacking = true;
        buttleAttack = 1;
        MonsterAI03.AttackAning(attacking, buttleAttack);
    }
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
    void MonsterAudio(int Nub)
    {
        MonsterSource.clip = MonsterClip[Nub];
        MonsterSource.volume = AudioManager.Slider[2].value;
        MonsterSource.mute = AudioManager.muteState[2];
        MonsterSource.Play();
        //if (Nub == 0)
        //{
            
        //}      
    }
    void ReloadAudio(int Nub)
    {
        AudioManager.Reload(Nub);
    }

}
