using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public int Type;
    public Animator Animator;
    public bool DownUp=true;
    public LayerMask LayerMask;
    GameObject play;
    bool SourcePause;
    public AudioManager audioManager;
    public AudioSource AudioS;
    bool PlayAudio = false;
    bool Playing = false;
    bool start=true;
    bool StopPlayA;
    public BoxCollider boxCollider;

    void Awake()
    {
    }
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        StopPlayA = false;
        boxCollider.enabled = true;

    }

    void Update()
    {      
        if(AudioS != null && Playing)
        {
            SourcePause = AudioManager.SourcePause;
            if (SourcePause)  //暫停
            {
                AudioS.Pause();
                PlayAudio = true;
                //("Play_暫停" + Play);
            }
            else
            {
                if (PlayAudio)
                {
                    PlayAudio = false;
                    AudioS.Play();
                    //print("暫停後Play_" + Play);
                }
            }
        }
        if (StopPlayA)
        {
            audioManager.StopPlayAudio();
        }
    }
    void Up()  //動畫
    {
        DownUp = true;
    }
    void Down()
    {
        DownUp = false;
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            switch (Type)
            {
                case 0:
                    if (start)
                    {
                        start = false;
                        play = collider.gameObject;
                        if (DownUp)
                        {
                            Animator.SetTrigger("Down");
                            AudioManager.MechanicalAudio(gameObject, 0);
                            AudioS = GetComponent<AudioSource>();
                        }
                        //else
                        //{
                        //    Animator.SetTrigger("Up");
                        //    AudioManager.MechanicalAudio(gameObject, 0);
                        //}
                        EnterEV();
                    }
                    break;
                case 1:
                    play = collider.gameObject;
                    if (DownUp)
                    {
                        Animator.SetTrigger("Down");
                        AudioManager.MechanicalAudio(gameObject, 0);
                        AudioS = GetComponent<AudioSource>();
                        StopPlayA = true;
                    }
                    //else
                    //{
                    //    Animator.SetTrigger("Up");
                    //    AudioManager.MechanicalAudio(gameObject, 0);
                    //}
                    EnterEV();
                    break;
            }
            
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            //play = collider.gameObject;
            //ExitEV();
        }
    }
    void EnterEV()
    {
        if (!Playing)
        {
            PlayAudio = Playing = true;
        }
        //XX物件變成子物件
        play.transform.parent = gameObject.transform;
        play.GetComponent<PlayerMove>().enabled = false;

    }
    void ExitEV()
    {
        boxCollider.enabled = false;
        StopPlayA = false;
        Playing = false;
        //子物件脫離父物件
        play.transform.parent = null;
        play.GetComponent<PlayerMove>().enabled = true;
    }
}
