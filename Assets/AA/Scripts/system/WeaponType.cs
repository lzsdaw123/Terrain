using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponType
{

}

[Serializable]
public class WeaponValue
{
    public int WeaponPos; //武器位置
    public float power;  //威力
    public float distance;  //射程
    public int WeapAm;  //武器彈藥量
    public int T_WeapAm; //武器總彈藥量

    public WeaponValue(int _WeaponPos, float Power, float Distance, int _WeapAm, int _T_WeapAm)
    {
        WeaponPos = _WeaponPos;
        power = Power;
        distance = Distance;
        WeapAm = _WeapAm;
        T_WeapAm = _T_WeapAm;
    }
}
