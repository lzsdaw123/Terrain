using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammunition : MonoBehaviour
{
    public int ammunition, Total_ammunition;
    public int WeaponType; //武器類型
    public Text text;
    static Color Color;
    public Animator animator;
    public Image[] Images;
    public RawImage[] RawImages;
    public Texture2D[] Texture2D = new Texture2D[3];
    static bool Weapswitch;
    static int Type;
    [SerializeField] int[] weapon_of_Pos;
    int[] Equipment;

    void Start()
    {
        Color = text.color;
        Color.a = 0;
        RawImages[0].gameObject.SetActive(false);
        RawImages[1].gameObject.SetActive(false);
        animator.SetInteger("WeapSW", -1);

    }

    void Update()
    {
        WeaponType = Shooting.WeaponType;
        ammunition = Shooting.Weapons[WeaponType].WeapAm;
        Total_ammunition = Shooting.Weapons[WeaponType].T_WeapAm;
        text.color = Color;
        text.text = ammunition+"/"+ Total_ammunition;

        if (Shooting.FirstWeapon)
        {
            if (Weapswitch)
            {
                animator.SetTrigger("Start");
                switch (Type)
                {
                    case 0:
                        animator.SetInteger("WeapSW", 0);
                        break;
                    case 1:
                        animator.SetInteger("WeapSW", 1);
                        break;
                    case 2:
                        animator.SetInteger("WeapSW", 0);
                        break;
                }
                Weapswitch = false;
            }
        }

        weapon_of_Pos = Shooting.Weapon_of_Pos;
        Equipment = Shooting.Equipment;
        if (weapon_of_Pos[0] == 0)
        {
            RawImages[0].gameObject.SetActive(true);
            RawImages[0].GetComponent<RawImage>().texture = Texture2D[0];
        }
        if (weapon_of_Pos[1] == 1)
        {
            RawImages[1].gameObject.SetActive(true);
            RawImages[1].GetComponent<RawImage>().texture = Texture2D[1];
        }
        if (weapon_of_Pos[0] == 2)
        {
            RawImages[0].gameObject.SetActive(true);
            RawImages[0].GetComponent<RawImage>().texture = Texture2D[2];
        }
    }
    public static void showUI()
    {
        Color.a = 1;
    }
    public static void Switch(int type)
    {
        Weapswitch = true;
        Type = type;
    }
}
