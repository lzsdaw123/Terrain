using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammunition : MonoBehaviour
{
    public int ammunition, Total_ammunition;
    public int WeaponType; //武器類型

    void Start()
    {
    }

    void Update()
    {
        WeaponType = Shooting.WeaponType;
        ammunition = Shooting.Weapons[WeaponType].WeapAm;
        Total_ammunition = Shooting.Weapons[WeaponType].T_WeapAm;

        GetComponent<Text>().text = ammunition+"/"+ Total_ammunition;  
    }
}
