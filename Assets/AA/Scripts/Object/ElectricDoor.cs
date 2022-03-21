using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectricDoor : MonoBehaviour
{
    public GameObject TextG;
    public bool Botton;
    public Animator Animator;
    public int Type;  //門類型
    public Vector3[] pos;
    public GameObject[] Door;  //哪扇門
    [SerializeField] bool OpenDoor = false;
    public bool OriDoor;
    [SerializeField] private float time;
    float speed;
    bool SourcePause;
    [SerializeField] bool PlayAudio;
    public AudioSource AudioS;
    public bool AutoDoor; 

    void Start()
    {
        TextG = GameObject.Find("ObjectText");
        Botton = false;
        time = 0;
        speed = 5;
        Animator.SetInteger("Type", Type);
    }

    void Update()
    {

        for(int i=0; i< pos.Length; i++)
        {
            pos[i] = Door[i].transform.localPosition;
        }
        if (Type == 2 && OriDoor == true)
        {
            OriDoor = false;
            pos[0].y = -0.444f;
        }

        switch (Type)
        {
            case 0:
                if (OpenDoor)
                {
                    if (Botton)
                    {
                        Animator.SetBool("Open", true);
                    }
                }
                else
                {
                    if (Botton)
                    {
                        Animator.SetBool("Open", false);
                    }
                }
                break;
            case 1:
                if (OpenDoor) //進行開門
                {
                    if (Botton)
                    {
                        Animator.SetBool("Open", true);
                    }
                }
                else  //進行關門
                {
                    if (Botton)
                    {
                        Animator.SetBool("Open", false);
                    }
                }
                break;
            case 2:
                if (OpenDoor)
                {
                    if (Botton)
                    {
                        Animator.SetBool("Open", true);
                    }
                }
                else
                {
                    if (Botton)
                    {
                        Animator.SetBool("Open", false);
                    }
                }
                break;
        }
        if (AutoDoor)
        {
            if (OpenDoor && !Botton)  //時間到自動關門
            {
                time += Time.deltaTime;
                if (time >= 2.6)
                {
                    time = 0;
                    Botton = true;
                    PlayAudio = true;
                    OpenDoor = false;
                }
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
    void end()
    {
        Botton = false;
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
