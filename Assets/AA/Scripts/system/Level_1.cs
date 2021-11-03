using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_1 : MonoBehaviour
{
    public bool Level_1_Start=false;
    GameObject SpawnRay;
    [SerializeField] GameObject explode;
    [SerializeField] ParticleSystem PSexplode;
    float time = 0;
    bool Lv1=false;

    void Start()
    {
        SpawnRay= GameObject.Find("SpawnRay");
        SpawnRay.SetActive(false);
        explode.SetActive(false);
    }

    void Update()
    {
        if (!Lv1)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                AudioManager.explode();
                Level_1_Start = true;
                explode.SetActive(true);
                Force.開始破門();
                SpawnRay.SetActive(true);
                Lv1 = true;
            }
        }
        if (Lv1)
        {
            time += Time.deltaTime;
            if (time >= 15f)
            {
                var main = PSexplode.main;
                main.loop = false;
            }
            if (time >=25f)
            {
                explode.SetActive(false);
            }
        }

    }
}
