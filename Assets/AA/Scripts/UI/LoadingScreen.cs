using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public RawImage rawImage;
    public Texture2D[] Texture2Ds;
    public int Ro;
    public int SaveRo;
    public float time;
 
    void Start()
    {
        SaveRo = -1;
        Ro = Random.Range(0, Texture2Ds.Length);
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= 3)
        {
            Ro = Random.Range(0, Texture2Ds.Length);
            if(Ro== SaveRo)
            {
                return;
            }
            rawImage.texture = Texture2Ds[Ro];
            SaveRo = Ro;
            time = 0;
        }
    }
}
