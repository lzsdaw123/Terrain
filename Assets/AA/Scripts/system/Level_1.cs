using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_1 : MonoBehaviour
{
    public bool Level_1_Start=false;
    GameObject SpawnRay;

    void Start()
    {
        SpawnRay= GameObject.Find("SpawnRay");
        SpawnRay.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Level_1_Start = true;
            Force.開始破門();
            SpawnRay.SetActive(true);
        }
    }
}
