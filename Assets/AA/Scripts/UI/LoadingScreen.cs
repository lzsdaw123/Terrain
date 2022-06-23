using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public RawImage rawImage;
    public Texture2D[] Lv1_Texture2Ds;
    public Texture2D[] Lv2_Texture2Ds;
    public GameObject[] content;
    public int Level;
    public int Ro;
    public int SaveRo;
    public float time;
    public int[] Nub;
    public bool[] Ran;
 
    void Start()
    {
        content[0].SetActive(false);
        content[1].SetActive(false);
        Level = Settings.GameLevel;
        SaveRo = -1;
        Nub =new int[] {0,0 };
        Ran =new bool[] {false,false };
        switch (Settings.GameLevel)
        {
            case 1:
                Ro = Random.Range(0, Lv1_Texture2Ds.Length);
                rawImage.texture = Lv1_Texture2Ds[0];
                content[0].SetActive(true);
                content[1].SetActive(false);
                break;
            case 2:
                Ro = Random.Range(0, Lv2_Texture2Ds.Length);
                rawImage.texture = Lv2_Texture2Ds[0];
                content[1].SetActive(true);
                content[0].SetActive(false);
                break;
        }
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Level = Settings.GameLevel;
        time += Time.deltaTime;
        if (time >= 2.5f)
        {
            switch (Settings.GameLevel)
            {
                case 1:
                     if (Ran[0])
                    {
                        Ro = Random.Range(0, Lv1_Texture2Ds.Length);
                    }
                    else
                    {
                        if (Nub[0] >= Lv1_Texture2Ds.Length)
                        {
                            Nub[0] = 0;
                            Ran[0] = true;
                            Ro = Random.Range(0, Lv1_Texture2Ds.Length);
                        }
                        else
                        {
                            Ro = Nub[0];
                            Nub[0]++;
                        }
                    }
                    break;
                case 2:
                    if (Ran[1])
                    {
                        Ro = Random.Range(0, Lv2_Texture2Ds.Length);
                    }
                    else
                    {
                        if (Nub[1] >= Lv2_Texture2Ds.Length)
                        {
                            Nub[1] = 0;
                            Ran[1] = true;
                            //Ro = Random.Range(0, Lv2_Texture2Ds.Length);
                        }
                        else
                        {
                            Ro = Nub[1];
                            Nub[1]++;
                        }
                    }
                    break;
            }
            if(Ro== SaveRo)
            {
                return;
            }
            switch (Settings.GameLevel)
            {
                case 1:
                    rawImage.texture = Lv1_Texture2Ds[Ro];
                    content[0].SetActive(true);
                    content[1].SetActive(false);
                    break;
                case 2:
                    rawImage.texture = Lv2_Texture2Ds[Ro];
                    content[1].SetActive(true);
                    content[0].SetActive(false);
                    break;
            }
            SaveRo = Ro;
            time = 0;
        }
    }
}
