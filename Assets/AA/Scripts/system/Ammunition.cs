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
        ammunition = Shooting.WeapAm[WeaponType];
        Total_ammunition = Shooting.T_WeapAm[WeaponType];

        GetComponent<Text>().text = ammunition+"/"+ Total_ammunition;  
    }
}
