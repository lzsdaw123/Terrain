using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
    static AudioManager current;
    public Settings Settings;
    public Image AsI, PsI;

    public GameObject AudioSourceUI;

    public AudioClip RainCilp;  //下雨音效

    public AudioClip[] WalkClip;  //走路音效
    public AudioClip[] GunshotsClip;  //走路音效

    AudioSource AmbientSource;  //環境音源
    AudioSource PlayerSource;  //玩家音源
    AudioSource GunSource;  //槍枝音源

    public Scrollbar[] Scrollbar;
    public Button[] MuteButton;  //BSE靜音按鈕
    public Text[] Nub;  //BSE
    public bool[] muteState;   

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);  //切換場景時保留
        current = this;
        AudioSourceUI.SetActive(true);

        //生成聲音控制器
        AmbientSource = gameObject.AddComponent<AudioSource>();
        PlayerSource = gameObject.AddComponent<AudioSource>();
        GunSource = gameObject.AddComponent<AudioSource>();

        muteState[1] = AmbientSource.mute;
        muteState[2] = PlayerSource.mute;
        AsI.color = new Color(0.298f, 0.298f, 0.298f, 1f);
        StartLevelAudio();
    }
    void Update()
    {
        AmbientSource.volume = Scrollbar[1].value;
        PlayerSource.volume = Scrollbar[2].value;
        GunSource.volume = Scrollbar[2].value;

        for (int i =0; i< Nub.Length ; i++)
        {          
            float ScrV = Scrollbar[i].value * 100;
            int _Nub = (int)ScrV;
            Nub[i].text = _Nub + " %";
        }

    }
    public void AudioSetUI()  //點開聲音設定
    {
        AsI.color = new Color(0.298f, 0.298f, 0.298f, 1f);
        PsI.color = new Color(0.643f, 0.643f, 0.643f, 1f);
        AudioSourceUI.SetActive(true);
        Settings.PictureSetUI.SetActive(false);
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
        GunSource.mute = muteState[2];

    }

    void StartLevelAudio()  //背景音效
    {
        current.AmbientSource.clip = current.RainCilp;
        current.AmbientSource.loop = true;
        current.AmbientSource.Play();
    }

    public static void PlayFootstepAudio()
    {
        int index = Random.Range(0, current.WalkClip.Length);

        current.PlayerSource.clip = current.WalkClip[index];
        current.PlayerSource.Play();
    }
    public static void PlayGunshotsAudio(int B)
    {
        current.GunSource.clip = current.GunshotsClip[B];
        current.GunSource.volume = 0.6f;
        current.GunSource.pitch = 1.3f;
        if (B == 0)
        {
            current.GunSource.volume = 1.2f;
            current.GunSource.pitch = 1f;
        }
        current.GunSource.Play();
    }
}
