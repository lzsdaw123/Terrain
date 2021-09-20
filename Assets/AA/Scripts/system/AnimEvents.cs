using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public static bool DontShooting;
    public static int ammunition, Total_ammunition;
    public static int N_ammunition, N_Total_ammunition;  //彈藥量

    public static bool attacking = false;
    public static int buttleAttack=0;


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
        ammunition = 30;
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
    }
    void AttackiBEnd()
    {
        buttleAttack = 0;
    }

    void AttackCompletion()
    {
        attacking = false;
    }
    public void WalkAudio()
    {
        AudioManager.PlayFootstepAudio();
    }
    void WalkCilpRight()
    {

    }
}
