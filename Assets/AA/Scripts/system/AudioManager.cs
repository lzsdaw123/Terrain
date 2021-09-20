using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static AudioManager current;


    public AudioClip RainCilp;

    public AudioClip[] WalkClip;  //走路音效


    AudioSource AmbientSource;
    AudioSource PlayerSource;


    private void Awake()
    {
        current = this;

        DontDestroyOnLoad(gameObject);

        AmbientSource = gameObject.AddComponent<AudioSource>();
        PlayerSource = gameObject.AddComponent<AudioSource>();

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
}
