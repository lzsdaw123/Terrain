using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSourceUI : MonoBehaviour
{
    public GameObject AudioManager;

    AudioSource AmbientSource;  //環境音源
    AudioSource PlayerSource;  //玩家音源
    AudioSource GunSource;  //槍枝音源

    void Awake()
    {
        AmbientSource = AudioManager.AddComponent<AudioSource>();
        PlayerSource = AudioManager.AddComponent<AudioSource>();
        GunSource = AudioManager.AddComponent<AudioSource>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
