using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{
    static AudioManager current;
    public Settings SettingsCanvas;
    public Image AsI, PsI;

    public GameObject AudioSourceUI;

    public static bool SourcePause = false;
    public static bool walkPlay=true;

    //BGM

    //BGS
    public AudioClip[] BgsCilp;  //背景音效
    //SE
    public AudioClip[] WalkClip;  //走路音效
    //public AudioClip[] MetalWalkClip;  //走路音效
    public AudioClip[] JumpClip;  //落地音效
    public AudioClip[] GunshotsClip;  //開槍音效
    public AudioClip[] HitClip;  //擊中音效
    public AudioClip ElevatorCilp;  //電梯音效
    public AudioClip ExplodeCilp;  //爆炸音效
    public AudioClip ButtonCilp;  //按鈕音效
    public AudioClip[] ActionCilp;  //動作音效
    public AudioClip[] WarnCilp;  //提示音效


    AudioSource AmbientSource;  //環境音源
    AudioSource PlayerSource;  //玩家音源
    AudioSource GunSource;  //槍枝音源
    AudioSource HitSource;  //擊中音源
    static AudioSource ElevatorSource;  //電梯音源
    AudioSource EffectsSource;  //特效音源
    AudioSource ButtonSource;  //按鈕音源
    AudioSource ActionSource;  //動作音源
    AudioSource WarnSource;  //提示音源

    public Slider[] Slider;
    public Button[] MuteButton;  //BSE靜音按鈕
    public Text[] Nub;  //BSE
    public bool[] muteState;

    int SceneNub;  //當前場景編號
    int OriSceneNub;  //當前場景編號

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);  //切換場景時保留
        current = this;
        AudioSourceUI.SetActive(true);
        SettingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Settings>();

        //生成聲音控制器
        AmbientSource = gameObject.AddComponent<AudioSource>();
        PlayerSource = gameObject.AddComponent<AudioSource>();
        GunSource = gameObject.AddComponent<AudioSource>();
        HitSource = gameObject.AddComponent<AudioSource>();
        EffectsSource = gameObject.AddComponent<AudioSource>();
        ButtonSource = gameObject.AddComponent<AudioSource>();
        ActionSource = gameObject.AddComponent<AudioSource>();
        WarnSource = gameObject.AddComponent<AudioSource>();

        muteState[1] = AmbientSource.mute;
        muteState[2] = PlayerSource.mute;
        AsI.color = new Color(0.298f, 0.298f, 0.298f, 1f);
        SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        OriSceneNub = SceneNub;
        StartLevelAudio();

    }
    void Update()
    {
        SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        if (SceneNub != OriSceneNub)
        {
            OriSceneNub = SceneNub;
            StartLevelAudio();
        }

        AmbientSource.volume = Slider[1].value;
        PlayerSource.volume = Slider[2].value;
        GunSource.volume = Slider[2].value;
        HitSource.volume = Slider[2].value;
        EffectsSource.volume = Slider[2].value;
        ButtonSource.volume = Slider[2].value;
        ActionSource.volume = Slider[2].value;
        WarnSource.volume = Slider[2].value;
        if (ElevatorSource !=null)
        {
            ElevatorSource.volume = Slider[2].value;
        }

        for (int i =0; i< Nub.Length ; i++)
        {          
            float ScrV = Slider[i].value * 100;
            int _Nub = (int)ScrV;
            Nub[i].text = _Nub + " %";
        }
        if (SettingsCanvas.transform.GetChild(0).gameObject.activeSelf)  //遊戲是否暫停
        {
            SourcePause = true;
            AmbientSource.Pause();
            PlayerSource.Pause();
            ActionSource.Pause();
            GunSource.Pause();
            HitSource.Pause();
            EffectsSource.Pause();
            WarnSource.Pause();
        }
        else
        {
            SourcePause = false;
            AmbientSource.UnPause();
            PlayerSource.UnPause();
            ActionSource.UnPause();
            GunSource.UnPause();
            HitSource.UnPause();
            EffectsSource.UnPause();
            WarnSource.UnPause();
        }
        if (ElevatorSource != null)
        {
            ElevatorSource.mute = muteState[2];
        }
        
    }
    public void AudioSetUI()  //點開聲音設定UI
    {
        AudioManager.Button();
        AsI.color = new Color(0.298f, 0.298f, 0.298f, 1f);
        PsI.color = new Color(0.643f, 0.643f, 0.643f, 1f);
        AudioSourceUI.SetActive(true);
        SettingsCanvas.PictureSetUI.SetActive(false);
    }
    public void MuteState(int N)  //靜音
    {
        ColorBlock cb = new ColorBlock();
    
        if (!muteState[N])
        {
            cb.normalColor = Color.grey;
            cb.selectedColor = Color.grey;
            muteState[N] = true;
        }
        else
        {
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            muteState[N] = false;
        }
        cb.colorMultiplier = 1;
        
        MuteButton[N].colors = cb;      

        AmbientSource.mute = muteState[1];
        PlayerSource.mute = muteState[2];
        ActionSource.mute = muteState[2];
        GunSource.mute = muteState[2];
        HitSource.mute = muteState[2];
        EffectsSource.mute = muteState[2];
        ButtonSource.mute = muteState[2];
        WarnSource.mute = muteState[2];

    }

    void StartLevelAudio()  //背景音效
    {
        if (SceneNub == 1)
        {
            current.AmbientSource.clip = current.BgsCilp[0];
            current.AmbientSource.volume = 0.8f;
        }
        else if(SceneNub == 2)
        {
            current.AmbientSource.clip = current.BgsCilp[1];
        }
        else
        {
            current.AmbientSource.clip = null;
        }
        current.AmbientSource.loop = true;
        current.AmbientSource.Play();
    }
    public static void ElevatorAudio(GameObject ElevatorA)  //電梯音效
    {
        if (ElevatorSource == null)
        {
            ElevatorSource = ElevatorA.AddComponent<AudioSource>();
            ElevatorSource.clip = current.ElevatorCilp;
            ElevatorSource.volume = 2f;         
        }
    }
    public static void PlayFootstepAudio()  //走路
    {
        if (PlayerMove.Metal==0) //是否走在金屬上 0=否
        {
            int index = Random.Range(0, current.WalkClip.Length);
            current.PlayerSource.clip = current.WalkClip[index];
            current.PlayerSource.pitch = 1.5f;
        }
        else
        {
            current.PlayerSource.clip = current.JumpClip[1];
            current.PlayerSource.pitch = 0.75f;
            if (PlayerMove.Speed <= 4)
            {
                current.PlayerSource.pitch = 0f;
            }
        }
        current.PlayerSource.Play();
    }
    public static void PlayJumpAudio(int Nub)  //跳躍落地
    {
        current.PlayerSource.clip = current.JumpClip[Nub];
        if (Nub == 1)
        {
            current.PlayerSource.pitch = 0.65f;
        }
        current.PlayerSource.Play();
    }
    public static void PlayGunshotsAudio(int Nub)  //開火
    {
        current.GunSource.clip = current.GunshotsClip[Nub];
        current.GunSource.volume = 0.6f;
        current.GunSource.pitch = 1.3f;
        if (Nub == 0)
        {
            current.GunSource.volume = 1.2f;
        }
        current.GunSource.Play();
    }
    public static void Reload(int Nub)  //換彈
    {
        if (Nub == 0)
        {
            current.GunSource.clip = current.GunshotsClip[2];
        }
        else
        {
            current.GunSource.clip = current.GunshotsClip[3];
        }
        current.GunSource.Play();
    }
    public static void explode()  //爆炸
    {
        current.EffectsSource.clip = current.ExplodeCilp;
        current.EffectsSource.Play();
    }
    public static void Button()  //按鈕
    {
        current.ButtonSource.clip = current.ButtonCilp;
        current.ButtonSource.loop = false;
        current.ButtonSource.Play();
    }
    public static void PickUp(int Nub)  //互動
    {
        if (Nub == -1)
        {
            current.ActionSource.Stop();
            return;
        }
        current.ActionSource.clip = current.ActionCilp[Nub];
        if (Nub == 0)
        {
            current.ActionSource.pitch = 2;
        }
        current.ActionSource.Play();     
    }
    public static void Warn(int Nub)  //警告 提示
    {
        if (Nub ==-1)
        {
            current.WarnSource.Stop();
            return;
        }
        if (Nub == 0)
        {
            current.WarnSource.clip = current.WarnCilp[Nub];
            current.WarnSource.loop = true;
        }
        if (Nub == 1)
        {
            current.WarnSource.clip = current.WarnCilp[Nub];
            current.WarnSource.loop = false;
        }     
        current.WarnSource.Play();
    }
    public static void Hit(int Nub)  //擊中
    {
        current.HitSource.clip = current.HitClip[Nub];
        current.HitSource.pitch=0.8f;
        current.HitSource.Play();
    }
}
