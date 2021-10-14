using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static AudioManager current;


    public AudioClip RainCilp;  //下雨音效

    public AudioClip[] WalkClip;  //走路音效
    public AudioClip[] GunshotsClip;  //走路音效


    AudioSource AmbientSource;  //環境音源
    AudioSource PlayerSource;  //玩家音源
    AudioSource GunSource;  //槍枝音源


    private void Awake()
    {
        current = this;

        DontDestroyOnLoad(gameObject);  //切換場景時保留

        AmbientSource = gameObject.AddComponent<AudioSource>();
        PlayerSource = gameObject.AddComponent<AudioSource>();
        GunSource = gameObject.AddComponent<AudioSource>();

        StartLevelAudio();
    }
    void StartLevelAudio()
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
    public static void PlayGunshotsAudio()
    {
        current.GunSource.clip = current.GunshotsClip[0];
        current.GunSource.volume = 0.6f;
        current.GunSource.pitch = 1.3f;
        current.GunSource.Play();
    }
}
