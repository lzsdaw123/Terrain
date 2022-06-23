using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{
    public int Type;
    public Animator Animator;
    public bool DownUp=true;
    public LayerMask LayerMask;
    GameObject play;
    public AudioManager audioManager;
    bool SourcePause;
    [SerializeField] bool PlayAudio;
    public AudioSource AudioS;
    public AudioClip[] MechanicalCilp;
    bool Playing = false;
    bool start=true;
    bool StopPlayA;  //關閉玩家音效
    public BoxCollider boxCollider;
    public bool running;
    public Vector3 v3;
    public Vector3 Lv3;
    public Vector3 Pv3;
    public Vector3 PLv3;
    public GameObject Rp;
    [SerializeField] bool end;

    public bool 遊戲開始;

    void Awake()
    {
    }
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        StopPlayA = false;
        boxCollider.enabled = true;
        running = false;
        end = false;
        遊戲開始 = true;;
    }

    void Update()
    {
        
        SourcePause = AudioManager.SourcePause;
        if (SourcePause)  //暫停
        {
            PlayAudio = true;
            if (AudioS.isPlaying)
            {
                AudioS.Pause();
            }
        }
        else
        {
            if (!end)
            {
                if (PlayAudio)
                {
                    PlayAudio = false;
                    AudioS.Play();
                }
            }
        }
        //if (AudioS != null && Playing)
        //{
        //    SourcePause = AudioManager.SourcePause;
        //    if (SourcePause)  //暫停
        //    {
        //        AudioS.Pause();
        //        PlayAudio = true;
        //        //("Play_暫停" + Play);
        //    }
        //    else
        //    {
        //        if (PlayAudio)
        //        {
        //            PlayAudio = false;
        //            AudioS.Play();
        //            //print("暫停後Play_" + Play);
        //        }
        //    }
        //}
        if (StopPlayA)
        {
            audioManager.StopPlayAudio();
        }
        if (running)
        {
            v3 = Rp.transform.position;
            Lv3 = Rp.transform.localPosition;
            Pv3 = play.transform.position;
            PLv3 = play.transform.localPosition;
            Vector3 Vv3 = play.transform.localPosition;
            Vv3.z = Rp.transform.localPosition.z;
            play.transform.localPosition = Vv3;
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
                            PlayingAudio(0, false);
                        }
                        //else
                        //{
                        //    Animator.SetTrigger("Up");
                        //    AudioManager.MechanicalAudio(gameObject, 0);
                        //}
                        if (Level_1.LevelA_>1)
                        {
                            Shooting.JumpDown = 1;
                        }
                        EnterEV();
                    }
                    break;
                case 1:
                    play = collider.gameObject;
                    if (DownUp)
                    {
                        Animator.SetTrigger("Down");
                        AudioS.clip = MechanicalCilp[0];
                        AudioS.loop = false;
                        AudioS.Play();
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
    void PlayingAudio(int Type, bool loop)
    {
        AudioS.clip = MechanicalCilp[Type];
        AudioS.loop = loop;
        AudioS.Play();
    }
    void EnterEV()
    {
        if (!Playing)
        {
            PlayAudio = Playing = true;
        }
        //XX物件變成子物件
        play.transform.parent = gameObject.transform;
        play.GetComponent<PlayerMove>().enabled = false; //關移動
        //play.GetComponent<PlayerMove>().isVehicle = true;
        PlayerMove.Speed = 0;
        //running = true;
        if (遊戲開始)
        {
            遊戲開始 = false;
            print("遊戲開始 ");
        }
    }
    void ExitEV()
    {
        PlayingAudio(2, false);
        end = true;
        boxCollider.enabled = false;
        StopPlayA = false;
        Playing = false;
        //子物件脫離父物件
        play.transform.parent = null;
        play.GetComponent<PlayerMove>().enabled = true;
        //play.GetComponent<PlayerMove>().isVehicle = false;
        //running = false;
        if (Level_1.LevelA_ > 7)
        {
            Shooting.JumpDown = 6;
        }
    }
    void middle()
    {
        PlayingAudio(1, true);
    }
}
