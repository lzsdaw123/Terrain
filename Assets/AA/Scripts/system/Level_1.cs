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
    public LayerMask LayerMask;
    bool start=false;
    public GameObject MissonUI,warnUI;

    void Start()
    {
        SpawnRay= GameObject.Find("SpawnRay");
        SpawnRay.SetActive(false);
        explode.SetActive(false);
        MissonUI.SetActive(false);
        warnUI.SetActive(false);
        warnUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 373, 0);
    }

    void Update()
    {
        if (!Lv1)
        {
            if (Input.GetKeyDown(KeyCode.F1) || start)
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
            if (time >= 7f)
            {
                MissonUI.SetActive(true);
                warnUI.SetActive(true);
                //AudioManager.Warn(0);

            }
            if (time >= 15f)
            {
                var main = PSexplode.main;
                main.loop = false;
                warnUI.SetActive(false);
            }
            if (time >=25f)
            {
                explode.SetActive(false);
            }
        }

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                start = true;
            }
        }
    }
}
