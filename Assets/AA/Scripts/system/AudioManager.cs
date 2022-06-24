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
    public AudioClip[] MechanicalCilp;  //機械音效  (電梯&開門)
    public AudioClip ExplodeCilp;  //爆炸音效
    public AudioClip ButtonCilp;  //按鈕音效
    public AudioClip[] ActionCilp;  //動作音效
    public AudioClip[] WarnCilp;  //提示音效

    public AudioSource 雨聲;
    public AudioSource 風聲;
    AudioSource AmbientSource;  //環境音源
    AudioSource PlayerSource;  //玩家音源
    AudioSource GunSource;  //槍枝音源
    AudioSource Gun2_Source;  //槍枝音源
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
    static bool Re;
    public static float[] SaveVolume =new float[9];  //保存預設音量

    public static int SceneNub;  //當前場景編號
    [SerializeField]int SF_SceneNub;  //當前場景編號
    int OriSceneNub;  //當前場景編號
    public static bool AudioStop;

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
        Gun2_Source = gameObject.AddComponent<AudioSource>();
        HitSource = gameObject.AddComponent<AudioSource>();
        EffectsSource = gameObject.AddComponent<AudioSource>();
        ButtonSource = gameObject.AddComponent<AudioSource>();
        ActionSource = gameObject.AddComponent<AudioSource>();
        WarnSource = gameObject.AddComponent<AudioSource>();

        muteState[1] = AmbientSource.mute;
        muteState[2] = PlayerSource.mute;
        AsI.color = new Color(0.55f, 0.82f, 1, 1f);
        SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        OriSceneNub = SceneNub;
        //StartLevelAudio(SceneNub);
        for (int i = 0; i < SaveVolume.Length; i++)
        {
            SaveVolume[i] = 0;
        }
    }
    void Start()
    {
        Slider[1].value = 0.6f;  //預設音量
        Slider[2].value = 0.4f;  //預設音量
        AudioStop = false;
    }
    void Update()
    {
        SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        if (SceneNub != OriSceneNub)
        {
            OriSceneNub = SceneNub;
            //StartLevelAudio(SceneNub);
        }
        if (AudioStop)
        {
            AmbientSource.Stop();
        }
        if (!AmbientSource.isPlaying && !AudioStop)
        {
            if (SceneNub == 1)
            {
                //SaveVolume[0] = 0.7f;
                StartLevelAudio(1);
                //風聲 = GameObject.Find("風聲").GetComponent<AudioSource>();
                //風聲.enabled = true;
                //if (!風聲.isPlaying)
                //{
                //    風聲.Play();
                //}
            }
            if (SceneNub == 2)
            {
                //SaveVolume[0] = 1f;
                StartLevelAudio(2);
                //雨聲 = GameObject.Find("雨聲").GetComponent<AudioSource>();
                //雨聲.enabled = true;
                //if (!雨聲.isPlaying)
                //{
                //    雨聲.Play();
                //}
            }
            if (SceneNub == 3)
            {
                AmbientSource.Stop();
            }
        }
        if (SceneNub == 3 && AmbientSource.isPlaying)
        {
            AmbientSource.Stop();
        }
        SF_SceneNub = SceneNub;
        if (Re)
        {
            Re = false;
            if(雨聲!=null) 雨聲.volume = Slider[1].value;
            if (風聲 != null) 風聲.volume = Slider[1].value;
            AmbientSource.volume = SaveVolume[0]* Slider[1].value;
            PlayerSource.volume = SaveVolume[1] * Slider[2].value;
            GunSource.volume = SaveVolume[2] * Slider[2].value;
            Gun2_Source.volume = SaveVolume[3] * Slider[2].value;
            HitSource.volume = Slider[2].value;
            EffectsSource.volume = Slider[2].value;
            ButtonSource.volume = Slider[2].value;
            ActionSource.volume = Slider[2].value;
            WarnSource.volume = Slider[2].value;
            if (ElevatorSource != null)
            {
                ElevatorSource.volume = 0.5f* Slider[2].value;
            }
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
            if (雨聲 != null) 雨聲.Pause();
            if (風聲 != null) 風聲.Pause();
            AmbientSource.Pause();
            PlayerSource.Pause();
            ActionSource.Pause();
            GunSource.Pause();
            Gun2_Source.Pause();
            HitSource.Pause();
            EffectsSource.Pause();
            WarnSource.Pause();
        }
        else
        {
            SourcePause = false;
            if (雨聲 != null) 雨聲.UnPause();
            if (風聲 != null) 風聲.UnPause();
            AmbientSource.UnPause();
            PlayerSource.UnPause();
            ActionSource.UnPause();
            GunSource.UnPause();
            Gun2_Source.UnPause();
            HitSource.UnPause();
            EffectsSource.UnPause();
            WarnSource.UnPause();
        }
        if (ElevatorSource != null)
        {
            ElevatorSource.mute = muteState[2];
        }
        
    }
    public static void OnClick()
    {
        Re = true;
    }

    public void AudioSetUI()  //點開聲音設定UI
    {
        AudioManager.Button();
        AsI.color = new Color(0.55f, 0.82f, 1, 1f);
        PsI.color = new Color(0.37f, 0.55f, 0.67f, 1f);
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

        if (雨聲 != null) 雨聲.mute = muteState[1];
        if (風聲 != null) 風聲.mute = muteState[1];
        AmbientSource.mute = muteState[1];
        PlayerSource.mute = muteState[2];
        ActionSource.mute = muteState[2];
        GunSource.mute = muteState[2];
        Gun2_Source.mute = muteState[2];
        HitSource.mute = muteState[2];
        EffectsSource.mute = muteState[2];
        ButtonSource.mute = muteState[2];
        WarnSource.mute = muteState[2];

    }

    public static void StartLevelAudio(int Type)  //背景音效
    {
        if (Type == 1)
        {
            current.AmbientSource.clip = current.BgsCilp[0];
            current.AmbientSource.volume = 0.2f;
            current.AmbientSource.pitch = 1;
        }
        if (Type == 2)
        {
            current.AmbientSource.clip = current.BgsCilp[1];
            current.AmbientSource.volume = 0.77f;
            current.AmbientSource.pitch = InteriorSpace.Pitch;
        }
        else
        {
            //current.AmbientSource.clip = null;
        }
        SaveVolume[0] = current.AmbientSource.volume;
        current.AmbientSource.loop = true;
        current.AmbientSource.Play();
        OnClick();
    }
    public static void MechanicalAudio(GameObject gameObject, int Type)  //機械音效 (電梯&開門)
    {
        if (ElevatorSource == null)
        {       
            ElevatorSource = gameObject.AddComponent<AudioSource>();
            ElevatorSource.clip = current.MechanicalCilp[0];
        }
    }
    public static void PlayFootstepAudio(int Type)  //走路
    {
        switch (Type)
        {
            case -1:
                current.PlayerSource.Stop();
                break;
            case 0:
                if (PlayerMove.Metal == 0) //是否走在金屬上 0=否
                {
                    int index = Random.Range(0, current.WalkClip.Length);
                    current.PlayerSource.clip = current.WalkClip[index];
                    current.PlayerSource.volume = 0.7f;
                    current.PlayerSource.pitch = 1.5f;
                }
                else  //走在金屬上
                {
                    current.PlayerSource.clip = current.JumpClip[1];
                    current.PlayerSource.volume = 0.42f;
                    current.PlayerSource.pitch = 0.75f;
                    if (PlayerMove.Speed <= 4)
                    {
                        current.PlayerSource.pitch = 0f;
                    }
                }
                SaveVolume[1] = current.PlayerSource.volume;
                current.PlayerSource.Play();
                OnClick();
                break;
        }
    }
    public static void PlayJumpAudio(int Nub)  //跳躍落地
    {
        current.PlayerSource.clip = current.JumpClip[Nub];
        switch (Nub)
        {
            case 0:  //土地
                current.PlayerSource.pitch = 1f;
                break;
            case 1:  //金屬地
                     //current.PlayerSource.pitch = 0.65f;
                current.PlayerSource.volume = 0.8f;
                current.PlayerSource.pitch = 0.8f;
                break;
        }
        SaveVolume[1] = current.PlayerSource.volume;
        current.PlayerSource.Play();
        OnClick();
    }
    public static void PlayGunshotsAudio(int Nub)  //開火
    {
        current.GunSource.clip = current.GunshotsClip[Nub];
        current.GunSource.pitch = 1.3f;
        current.GunSource.volume = 1f;
        switch (Nub)
        {
            case 0:
                current.GunSource.volume = 1.2f;
                break;
            case 1:
                current.GunSource.volume = 0.6f;
                break;
        }
        SaveVolume[2] = current.GunSource.volume;
        current.GunSource.Play();
        OnClick();
    }
    public static void Reload(int Nub)  //換彈
    {
        switch (Nub)
        {
            case 0:
                current.Gun2_Source.clip = current.GunshotsClip[2];
                break;
            case 1:
                current.Gun2_Source.clip = current.GunshotsClip[3];
                break;
            case 2:
                current.Gun2_Source.clip = current.GunshotsClip[5];
                break;
        }
        current.Gun2_Source.volume = 1;
        SaveVolume[3] = current.Gun2_Source.volume;
        current.Gun2_Source.Play();
        OnClick();
    }
    public static void explode()  //爆炸
    {
        current.EffectsSource.clip = current.ExplodeCilp;
        current.EffectsSource.Play();
        OnClick();
    }
    public static void Button()  //按鈕
    {
        current.ButtonSource.clip = current.ButtonCilp;
        current.ButtonSource.loop = false;
        current.ButtonSource.Play();
        OnClick();
    }
    public static void PickUp(int Nub)  //互動
    {
        if (Nub == -1)
        {
            current.ActionSource.Stop();
            return;
        }
        current.ActionSource.clip = current.ActionCilp[Nub];
        switch (Nub)
        {
            case 0:  //拾取彈藥
                current.ActionSource.pitch = 2;
                break;
            case 1:  //修理
                current.ActionSource.pitch = 1;
                break;
            case 2:  //拾取武器
                current.ActionSource.pitch = 1;
                break;
            case 3:  //拾取道具
                current.ActionSource.pitch = 1;
                break;
        }
        current.ActionSource.Play();
        OnClick();
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
        OnClick();
    }
    public static void Hit(int Nub)  //擊中
    {
        current.HitSource.clip = current.HitClip[Nub];
        //current.HitSource.pitch = 1f;
        switch (Nub)
        {
            case 0:
                current.HitSource.pitch = 0.8f;
                break;
            case 4:
                current.HitSource.Pause();
                current.HitSource.pitch = 1f;
                break;
        }
        current.HitSource.Play();
        OnClick();
    }
    public void StopPlayAudio()
    {
        PlayFootstepAudio(-1);
    }
}
