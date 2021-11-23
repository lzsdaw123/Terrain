using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public static bool DontShooting;
    public static int ammunition, Total_ammunition;
    public static int N_ammunition, N_Total_ammunition;  //彈藥量

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

    }

    void NoShooting()
    {
        DontShooting = true;
    }
    void YesShooting()
    {
        DontShooting = false;
    }
    void ReLoad()
    {    
        ammunition = Shooting.ammunition;
        Total_ammunition = Shooting.Total_ammunition;   

        int R_ammunition = 30 - ammunition;
        if (Total_ammunition < 30)
        {
            ammunition += Total_ammunition;
        }
        else
        {
            ammunition +=R_ammunition ;
        }
        if (ammunition >= 30) ammunition = 30;
        Total_ammunition -= R_ammunition;

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
    void Reload(int Nub)
    {
        AudioManager.Reload(Nub);
    }

}
