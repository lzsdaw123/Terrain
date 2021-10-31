using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Animator Animator;
    public bool DownUp=true;
    public LayerMask LayerMask;
    GameObject play;
    bool SourcePause;
    public AudioSource AudioS;
    bool Play=false;
    bool Playing = false;
    void Start()
    {
        
    }

    void Update()
    {      
        if(AudioS != null && Playing)
        {
            SourcePause = AudioManager.SourcePause;
            if (SourcePause)
            {
                AudioS.Pause();
                Play = true;
                //("Play_暫停" + Play);
            }
            else
            {
                if (Play)
                {
                    Play = false;
                    AudioS.Play();
                    //print("暫停後Play_" + Play);
                }
            }
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
            play = collider.gameObject;
            EnterEV();
            if (DownUp)
            {
                Animator.SetTrigger("Down");
                AudioManager.ElevatorAudio(gameObject);
                AudioS = GetComponent<AudioSource>();
            }
            else
            {
                Animator.SetTrigger("Up");
                AudioManager.ElevatorAudio(gameObject);
            }
        }
    }void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            play = collider.gameObject;
            ExitEV();
        }
    }
    void EnterEV()
    {
        Play = Playing = true;
        //XX物件變成子物件
        play.transform.parent = gameObject.transform;
        play.GetComponent<PlayerMove>().enabled = false;
        //print(Playing);

    }
    void ExitEV()
    {
        Playing = false;
        //子物件脫離父物件
        play.transform.parent = null;
        play.GetComponent<PlayerMove>().enabled = true;
        //print(Playing);
    }
}
