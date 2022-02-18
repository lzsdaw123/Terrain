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
    [SerializeField] bool OpenDoor;
    [SerializeField] private float time;
    float speed;
    bool SourcePause;
    [SerializeField] bool PlayAudio;
    public AudioSource AudioS;

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
                    if (Botton)
                    {
                        pos[Type].y -= speed * Time.deltaTime;
                    }
                    if (pos[Type].y <= -0.58)
                    {
                        pos[Type].y = -0.58f;
                        Botton = false;
                    }
                }
                else
                {
                    if (Botton)
                    {
                        pos[Type].y += speed * Time.deltaTime;
                    }
                    if (pos[Type].y >= 1.29)
                    {
                        pos[Type].y = 1.29f;
                        Botton = false;
                    }
                }
                Door[Type].transform.localPosition = pos[Type];
                break;
            case 1:
                if (OpenDoor) //進行開門
                {
                    //關門 115.71 /  97.8 -/- 開門 102.71 / 110.8  打開距離13
                    if (Botton)
                    {
                        pos[0].x -= speed * Time.deltaTime;
                        pos[1].x += speed * Time.deltaTime;
                    }
                    if (pos[0].x <= 102.71 && pos[1].x >= 110.8)
                    {
                        pos[0].x = 102.71f;
                        pos[1].x = 110.8f;
                        Botton = false;
                    }
                }
                else  //進行關門
                {
                    if (Botton)
                    {
                        pos[0].x += speed * Time.deltaTime;
                        pos[1].x -= speed * Time.deltaTime;
                    }
                    if(pos[0].x >= 115.71 && pos[1].x <= 97.8)
                    {
                        pos[0].x = 115.71f;
                        pos[1].x = 97.8f;
                        Botton = false;
                    }
                }
                for (int i = 0; i < pos.Length; i++)
                {
                    Door[i].transform.localPosition = pos[i];
                }
                break;
        }
        if (OpenDoor && !Botton)  //時間到自動關門
        {
            time += Time.deltaTime;
            if (time>=2.6)
            {
                time = 0;
                Botton = true;
                PlayAudio = true;
                OpenDoor = false;
            }
        }
        SourcePause = AudioManager.SourcePause;
        if (Botton)
        {
            if (SourcePause)  //暫停
            {
                PlayAudio = true;
                AudioS.Pause();
            }
            else
            {
                if (PlayAudio)
                {
                    PlayAudio = false;
                    AudioS.Play();
                }
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
                PlayAudio = true;
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
