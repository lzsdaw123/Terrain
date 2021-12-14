using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammunition : MonoBehaviour
{
    public int ammunition, Total_ammunition;
    public int WeaponType; //武器類型
    Text text;
    static Color Color;

    void Start()
    {
        text = GetComponent<Text>();
        Color = text.color;
        Color.a = 0;      
    }

    void Update()
    {
        WeaponType = Shooting.WeaponType;
        ammunition = Shooting.Weapons[WeaponType].WeapAm;
        Total_ammunition = Shooting.Weapons[WeaponType].T_WeapAm;
        text.color = Color;
        GetComponent<Text>().text = ammunition+"/"+ Total_ammunition;  
    }
    public static void showUI()
    {
        Color.a = 1;
    }
}
