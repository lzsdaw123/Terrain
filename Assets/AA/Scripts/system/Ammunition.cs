using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammunition : MonoBehaviour
{
    public int ammunition, Total_ammunition;

    void Start()
    {
    }

    void Update()
    {
        ammunition = Shooting.ammunition;
        Total_ammunition = Shooting.Total_ammunition;

        GetComponent<Text>().text = ammunition+"/"+ Total_ammunition;  
    }
}
