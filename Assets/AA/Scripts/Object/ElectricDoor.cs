using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectricDoor : MonoBehaviour
{
    public GameObject TextG;
    public bool Botton;
    public int Type;
    public Vector3[] pos;
    public GameObject[] Door;
    bool OpenDoor;
    [SerializeField] private float time;
    float speed;

    void Start()
    {
        TextG = GameObject.Find("ObjectText");
        Botton = false;
        OpenDoor = false;
        time = 0;
        speed = 5;
    }

    void Update()
    {

        for(int i=0; i< pos.Length; i++)
        {
            pos[i] = Door[i].transform.localPosition;
        }

        switch (Type)
        {
            case 0:
                if (OpenDoor)
                {
                    pos[Type].y -= speed * Time.deltaTime;
                    if (pos[Type].y <= -0.58)
                    {
                        pos[Type].y = -0.58f;
                        Botton = false;
                    }
                }
                else
                {
                    pos[Type].y += speed * Time.deltaTime;
                    if (pos[Type].y >= 1.29)
                    {
                        pos[Type].y = 1.29f;
                        Botton = false;
                    }
                }
                Door[Type].transform.localPosition = pos[Type];
                break;
            case 1:
                if (OpenDoor)
                {
                    //97.51 / 80.9 -/- 110.7 /67.71  13.19
                    pos[0].x += speed * Time.deltaTime;
                    pos[1].x -= speed * Time.deltaTime;
                    if (pos[0].x >= 110.7 && pos[1].x <= 67.71)
                    {
                        pos[0].x = 110.7f;
                        pos[1].x = 67.71f;
                        Botton = false;
                    }
                }
                else
                {
                    pos[0].x -= speed * Time.deltaTime;
                    pos[1].x += speed * Time.deltaTime;
                    if (pos[0].x <= 97.51 && pos[1].x >= 80.9)
                    {
                        pos[0].x = 97.51f;
                        pos[1].x = 80.9f;
                        Botton = false;
                    }
                }
                for (int i = 0; i < pos.Length; i++)
                {
                    Door[i].transform.localPosition = pos[i];
                }
                break;
        }
        if (OpenDoor && !Botton)
        {
            time += Time.deltaTime;
            if (time>=2.6)
            {
                time = 0;
                Botton = true;
                OpenDoor = false;
            }
        }
    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        if (OpenDoor)  //門開的
        {
            TextG.GetComponent<Text>().text = "按「E」關門\n";
        }
        else
        {
            TextG.GetComponent<Text>().text = "按「E」開門\n";
        }

        if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
        {
            if (!Botton)
            {
                Botton = true;
                time = 0;
                if (OpenDoor) //門開的
                {
                    OpenDoor =false;
                }
                else
                {
                    OpenDoor = true;
                }              
            }
        }
    }
}
